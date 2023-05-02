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
using CodeGeneration.CodeModifiers;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;

namespace CodeGeneration.Generators
{
    /// <summary>
    /// Generate a classes from and XSD (Xml Schema Document)
    /// </summary>
    public class XsdClassGenerator : CodeGeneratorBase, ICodeGenerator
    {
        /// <summary>
        /// Constructor:
        /// - initialize the schema
        /// - compile the schema
        /// - set some defaults
        /// </summary>
        /// <param name="schema"></param>
        public XsdClassGenerator(XmlSchema schema, CodeGenerationOptions options = CodeGenerationOptions.GenerateProperties)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema), "Xml Schema cannot be null");
            CodeGenerationOptions = options;

            #region save the schema to an XmlDocument
            using (StringWriter sw = new StringWriter())
            {
                Schema.Write(sw);
                SourceDocument.LoadXml(sw.ToString());
            }
            #endregion

            Utility.CompileSchema(schema);

            PreProcessSchemas();

            CompilerParameters.GenerateExecutable = false;
            CompilerParameters.GenerateInMemory = true;

            CodeModifiers
                .Add(new ConvertArraysToCollections())
                .Add(new AddDocComments())
                .Add(new RemoveObjectBase())
                .Add(new AddNonSerialized())
                .Add(new AddNonSerializedEvents())
                .Add(new RemoveSpecifiedTypes());
        }

        #region fields

        private readonly List<XmlSchema> m_schemaList = new List<XmlSchema>();

        private List<object> m_objects = new List<object>();

        private readonly SchemaImporterExtensionCollection m_schemaImporterExtensions = new SchemaImporterExtensionCollection();

        #endregion

        #region properties

        /// <summary> Options for this class generator </summary>
        public XsdClassGeneratorOptions ClassGeneratorOptions { get; set; } = new XsdClassGeneratorOptions();

        /// <summary> Options to pass to the Microsoft code generator </summary>
        public CodeGenerationOptions CodeGenerationOptions { get; set; } = CodeGenerationOptions.GenerateProperties;

        /// <summary> The Schema to generate code from </summary>
        public XmlSchema Schema { get; private set; }

        /// <summary> A list of instance objects from the compiled assembly </summary>
        public List<object> Objects => m_objects;

        /// <summary> A collection of extensions that help while importing schemas </summary>
        public SchemaImporterExtensionCollection SchemaImporterExtensions => m_schemaImporterExtensions;

        #endregion

        #region utility

        /// <summary>
        /// Generate code for all of the complex types in the schema
        /// </summary>
        /// <param name="xsd"></param>
        /// <param name="importer"></param>
        /// <param name="exporter"></param>
        private void GenerateForComplexTypes(XmlSchema xsd, XmlSchemaImporter importer, XmlCodeExporter exporter)
        {
            foreach(XmlSchemaComplexType ct in xsd.SchemaTypes.Values.OfType<XmlSchemaComplexType>())
            {
                Trace.TraceInformation("Generating for Complex Type: {0}", ct.Name);
                try
                {
                    exporter.ExportTypeMapping(importer.ImportSchemaType(ct.QualifiedName));
                }
                catch (Exception ex)
                {
                    Trace.TraceEvent(TraceEventType.Error, 10, ex.ToString());
                }
            }
        }

        /// <summary>
        /// Generate code for the elements in the Schema
        /// </summary>
        /// <param name="xsd"></param>
        /// <param name="importer"></param>
        /// <param name="exporter"></param>
        private void GenerateForElements(XmlSchema xsd, XmlSchemaImporter importer, XmlCodeExporter exporter)
        {
            foreach (XmlSchemaElement element in xsd.Elements.Values)
            {
                Trace.TraceInformation("Generating for element: {0}", element.Name);
                try
                {
                    exporter.ExportTypeMapping(importer.ImportTypeMapping(element.QualifiedName));
                }
                catch (Exception ex)
                {
                    Trace.TraceEvent(TraceEventType.Error, 10, ex.ToString());
                }
            }
        }

        /// <summary>
        /// Pre-process the loaded schemas
        /// </summary>
        private void PreProcessSchemas()
        {
            Trace.Assert(Schema != null);

            foreach (XmlSchemaExternal includedSchema in Schema.Includes)
            {
                if (includedSchema != null &&
                    includedSchema.Schema != null &&
                    !m_schemaList.Contains(includedSchema.Schema))
                    m_schemaList.Add(includedSchema.Schema);
            }
        }

        /// <summary>
        /// Add assembly references to the list of assemblies to 
        /// include when compiling the code
        /// </summary>
        protected override void AddReferencedAssemblies()
        {
            if (!ReferencedAssemblies.Contains("System.dll"))
                ReferencedAssemblies.Add("System.dll");
            if (!ReferencedAssemblies.Contains("mscorlib.dll"))
                ReferencedAssemblies.Add("mscorlib.dll");
            if (!ReferencedAssemblies.Contains("system.xml.dll"))
                ReferencedAssemblies.Add("system.xml.dll");
            if (!ReferencedAssemblies.Contains(Assembly.GetExecutingAssembly().Location))
                ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            if (!ReferencedAssemblies.Contains("System.Drawing.dll"))
                ReferencedAssemblies.Add("System.Drawing.dll");
        }

        #endregion

        /// <summary>
        /// Compile the code
        /// </summary>
        /// <returns></returns>
        public bool Compile()
        {
            CodeString = string.Empty;

            CreateCodeNamespace();

            #region Generate the CodeDom from the XSD

            XmlSchemas xmlSchemas = new XmlSchemas { Schema };
            foreach (XmlSchema s in m_schemaList)
                xmlSchemas.Add(s);

            XmlSchemaImporter schemaImporter = new XmlSchemaImporter(xmlSchemas);
            foreach (SchemaImporterExtension extension in m_schemaImporterExtensions)
                schemaImporter.Extensions.Add(extension);

            var ccu = new CodeCompileUnit();
            XmlCodeExporter codeExporter = new XmlCodeExporter(CodeNamespace, ccu, CodeGenerationOptions);

            try
            {
                GenerateForElements(Schema, schemaImporter, codeExporter);
                GenerateForComplexTypes(Schema, schemaImporter, codeExporter);
            }
            catch (Exception ex)
            {
                Trace.TraceEvent(TraceEventType.Error, 10, ex.ToString());
                throw new ApplicationException("Error Loading Schema: ", ex);
            }

            #endregion

            // apply the code modifiers
            CodeModifiers.ForEach(x => x.Execute(CodeNamespace, this));

            if (!GenerateCode())
                return false;

            #region Find the exported classes
            if (CodeGeneratorOptions.GenerateObjects && Assembly != null)
            {
                Type[] exportedTypes = Assembly.GetExportedTypes();

                // try to create an instance of the exported types
                m_objects = new List<object>();
                foreach (Type type in exportedTypes)
                {
                    if (type.IsAbstract)
                    {
                        Trace.TraceInformation("Type {0} is abstract, it is not created", type.ToString());
                        continue;
                    }

                    m_objects.Add(Activator.CreateInstance(type));
                }

            }
            #endregion

            return true;
        }

    }
}
