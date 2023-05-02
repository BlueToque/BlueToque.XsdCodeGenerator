//=============================================================================
//
// Copyright (C) 2007 Michael Coyle, Blue Toque
// Copyright (C) 2023 Michael Coyle, Blue Toque Software
// http://www.BlueToque.ca/Products/CodeGeneration.html
// michael.coyle@BlueToque.ca
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// http://www.gnu.org/licenses/gpl.txt
//
//=============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CodeGeneration.Generators
{
    public delegate string NameGeneratorDelegate(int index);

    /// <summary>
    /// Generate an XSD (Xml Schema Document) from a classes
    /// </summary>
    public class ClassXsdGenerator
    {
        public ClassXsdGenerator() { }

        /// <summary>
        /// Generate a set of schemas from the given type.
        /// 
        /// Every serializable .NET class can be represented by a schema. 
        /// If that class inherits from another class, then this method will generate more than one schema.
        /// The number of schemas is determines by .NET's own magic formula for when a new schema is needed
        /// but this is probably determined by the namespace attributes added (in the form of XmlRoot and 
        /// XmlType attribute) to the class.
        /// 
        /// We assume that the first schema in the array of generated schemas contains the schema
        /// of the type we requested.
        /// 
        /// </summary>
        /// <param name="type">A type to generate schemas from</param>
        /// <returns>An array of schemas</returns>
        public IList<XmlSchema> GenerateSchema(Type type)
        {
            Type typeToGenerate = type.IsByRef ? type.GetElementType() : type;

            #region generate the schema from the type

            XmlReflectionImporter reflectionImporter = new XmlReflectionImporter();

            XmlSchemas schemas = new XmlSchemas();

            XmlSchemaExporter exporter = new XmlSchemaExporter(schemas);

            XmlTypeMapping map = reflectionImporter.ImportTypeMapping(typeToGenerate);

            exporter.ExportTypeMapping(map);
            #endregion

            #region compile the schemas to make sure they were generated correctly
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            try
            {
                foreach (XmlSchema schema in schemas)
                    schemaSet.Add(schema);

                // some generated classes contain xs:any elements
                // disabling this check allows us to avoid a compilation error
                schemaSet.CompilationSettings.EnableUpaCheck = false;

                // we don't want to resolve any outside schemas
                schemaSet.XmlResolver = null;                 
                schemaSet.ValidationEventHandler += new ValidationEventHandler(SchemaSet_ValidationEventHandler);
                schemaSet.Compile();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw;
            }
            #endregion

            #region this is a special case for certain types of generated classes
            // when you use the System.Reflection.Emit namespace to generate a type, 
            // and then try to generate schemas from that type, the schemas don't
            // contain "imports" for types in other namespaces.
            // This code block adds those imports.

            // if the number of schemas generated is greater than 1 (meaning there are 
            // potentially types in other namespaces (and hence other schemas)
            // AND if the first schema generated does not include any other schemas
            // we know we're generating a schema that has the charateristics we're
            // expecting
            if (schemas.Count > 1 && schemas[0].Includes.Count == 0)
            {
                // create a list of schemas to process
                IList<XmlSchema> schemaArray = XmlSchemasToArray(schemas);

                // since the number of schemas is greater than one we know that there 
                // are at least 2 schemas, so we can safely index from 1
                for (int i = 1; i < schemaArray.Count; i++)
                {
                    // create a new schema import, and set the namespace
                    XmlSchemaImport import = new XmlSchemaImport
                    {
                        Namespace = schemaArray[i].TargetNamespace
                    };

                    // import it into the first schema
                    schemaArray[0].Includes.Add(import);
                }

                schemaSet.Compile();
            }

            #endregion

            #region "fix" the pointers to the included schemas for the generated schemas
            ResolveImportedSchemas(schemas, XmlConvert.EncodeName(typeToGenerate.Name));
            #endregion

            // convert the generated schemas to an array of schemas
            return XmlSchemasToArray(schemas);
        }

        /// <summary>
        /// Local schema validation event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SchemaSet_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                Trace.TraceError(e.Message);
                throw e.Exception;
            }

            if (e.Severity == XmlSeverityType.Warning)
                Trace.TraceWarning(e.Message);
        }

        /// <summary>
        /// Generate a set of schemas from the given types
        /// </summary>
        /// <param name="types">Array of types to generate schemas from</param>
        /// <returns>An array of schemas</returns>
        public IList<XmlSchema> GenerateSchemas(Type[] types)
        {
            Trace.Assert(types != null);
            if (types.Length == 0) return null;

            #region generate the schema from the type
            XmlReflectionImporter reflectionImporter = new XmlReflectionImporter();

            XmlSchemas schemas = new XmlSchemas();

            XmlSchemaExporter exporter = new XmlSchemaExporter(schemas);

            foreach (Type type in types)
            {
                // we can provide the default namespace as a parameter here
                XmlTypeMapping map = reflectionImporter.ImportTypeMapping(type);

                exporter.ExportTypeMapping(map);
            }
            #endregion

            // compile the schemas to make sure they were generated correctly
            schemas.Compile(null, true);

            ResolveImportedSchemas(schemas, types[0].Name);

            // convert the generated schemas to an array of schemas
            return XmlSchemasToArray(schemas);
        }

        public event NameGeneratorDelegate NameGenerator;

        #region utility
        private void ResolveImportedSchemas(XmlSchemas schemas, string baseName)
        {
            // "fix" the pointers to the included schemas for the generated schemas
            //NameGenerator.Reset(baseName + "GeneratedSchema");
            foreach (XmlSchema schema in schemas)
                ResolveImportedSchemas(schema, schemas);
        }

        int m_schemaIndex = 0;

        private string GenerateName() => NameGenerator != null ? NameGenerator(m_schemaIndex++) : "parameter" + m_schemaIndex++.ToString();

        /// <summary>
        /// 
        /// Resolve the pointers to included schemas.
        /// 
        /// When we generate the schemas from types, and more than one schema is generated, 
        /// the "master" schema that contains the schema for the type that is the parameter
        /// for this method will "include" the other schemas that define the 
        /// types it inherits from or the types that it returns in it's properties.
        /// If we read this schema off the disk, then the XmlSchemaExternal class's Schema property
        /// would contain a pointer to the included schema. This is not the case for generated 
        /// schemas.
        /// 
        /// What we do in this method is recurse through the generated schemas, and look for 
        /// included schemas who's pointers are null. Then we find out what namespoace those 
        /// include schemas are in, and look for that in the generated schemas collection. 
        /// 
        /// If we find it, we "fix" the pointer by setting it to the proper generated schema.
        /// 
        /// This method modified the schemas in the schema collection
        /// </summary>
        /// <param name="xmlSchema">The master schema</param>
        /// <param name="schemas">the collection of generated schemas</param>
        private void ResolveImportedSchemas(XmlSchema xmlSchema, XmlSchemas schemas)
        {
            string baseSchemaName = GenerateName();
            if (string.IsNullOrEmpty(xmlSchema.SourceUri))
            {
                xmlSchema.Id = Path.GetFileNameWithoutExtension(baseSchemaName);
                xmlSchema.SourceUri = baseSchemaName;
            }

            foreach (XmlSchemaObject externalSchemaObj in xmlSchema.Includes)
            {
                if (!(externalSchemaObj is XmlSchemaExternal externalSchema)) continue;

                // if the external schema is not null, we have nothing to worry about
                if (externalSchema.Schema != null) continue;

                // if the external schema is null, find it in the schema set
                if (externalSchemaObj is XmlSchemaImport importSchema)
                {
                    if (schemas.Contains(importSchema.Namespace))
                    {
                        XmlSchema referencedSchema = schemas[importSchema.Namespace];
                        importSchema.Schema = referencedSchema;
                        string name = GenerateName();
                        importSchema.Schema.Id = Path.GetFileNameWithoutExtension(name);
                        importSchema.Schema.SourceUri = name;
                        importSchema.SchemaLocation = MakeRelative(xmlSchema.SourceUri, name);
                    }
                }

                // resolve any included schemas in the external schema
                ResolveImportedSchemas(externalSchema.Schema, schemas);
            }

        }

        private string MakeRelative(string parentPath, string childPath)
        {
            if (!Uri.IsWellFormedUriString(parentPath, UriKind.RelativeOrAbsolute))
                return childPath;

            Uri parent = new Uri(parentPath, UriKind.RelativeOrAbsolute);
            if (!parent.IsAbsoluteUri)
                return childPath;

            Uri relative = parent.MakeRelativeUri(new Uri(childPath,  UriKind.RelativeOrAbsolute));

            return relative.OriginalString;
        }

        /// <summary>
        /// Convert a collection of schemas to an array of schemas
        /// </summary>
        /// <param name="schemas"></param>
        /// <returns></returns>
        private IList<XmlSchema> XmlSchemasToArray(XmlSchemas schemas)
        {
            List<XmlSchema> schemaCollection = new List<XmlSchema>();
            schemaCollection.AddRange(schemas);
            return schemaCollection;
        }

        #endregion


#if DEBUG
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static string SchemaToText(XmlSchema schema)
        {
            if (schema == null)
                return string.Empty;

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xtw = new XmlTextWriter(sw))
                {
                    xtw.Formatting = Formatting.Indented;
                    schema.Write(xtw);
                }

                return sw.ToString();
            }
        }

#endif

    }

}
