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
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CodeGeneration
{
    /// <summary>
    /// Take a class object and create a schema from it
    /// Also includes methods for "wrapping" a schema that contains
    /// multiple top-level elements into a single complex-types top level 
    /// element.
    /// </summary>
    public static class MethodCallTools
    {
        #region generates schema
      
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
        public static IList<XmlSchema> GenerateSchema(Type type)
        {
            #region generate the schema from the type
            XmlReflectionImporter reflectionImporter = new XmlReflectionImporter();

            XmlSchemas schemas = new XmlSchemas();

            XmlSchemaExporter exporter = new XmlSchemaExporter(schemas);

            XmlTypeMapping map = reflectionImporter.ImportTypeMapping(type);

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
                schemaSet.XmlResolver = null; // we don't want to resolve any outside schemas
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
            ResolveImportedSchemas(schemas, XmlConvert.EncodeName(type.Name));
            #endregion

            // convert the generated schemas to an array of schemas
            return XmlSchemasToArray(schemas);
        }

        static void SchemaSet_ValidationEventHandler(object sender, ValidationEventArgs e)
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
        public static IList<XmlSchema> GenerateSchemas(Type[] types)
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

        #region utility

        private static void ResolveImportedSchemas(XmlSchemas schemas, string baseName)
        {
            // "fix" the pointers to the included schemas for the generated schemas
            NameGenerator.Reset(baseName + "GeneratedSchema");
            foreach (XmlSchema schema in schemas)
                ResolveImportedSchemas(schema, schemas);
        }

        /// <summary>
        /// NameGenerator class
        /// Generates names in a sequence from a base name.
        /// Reset the namegenerator with the base name you choose, this will set the counter to
        /// zero. After this point the names generated will have a form {baseName}{number} 
        /// where number increments every time you call GetName
        /// 
        /// There is a static interface for situations when you need to use this class and not
        /// have it on the stack
        /// </summary>
        public class NameGenerator
        {
            public NameGenerator(string baseName) => m_baseName = baseName;

            int m_number = 0;

            readonly string m_baseName;

            /// <summary>
            /// Gets the next name
            /// </summary>
            /// <returns></returns>
            public string GetName() => m_baseName + m_number++.ToString();

            #region static interface

            static NameGenerator m_nameGenerator;

            /// <summary>
            /// Reset sets the counter to zero and sets the name to a new name
            /// </summary>
            /// <param name="nameBase"></param>
            public static void Reset(string baseName) => m_nameGenerator = new NameGenerator(baseName);

            /// <summary>
            /// Generates a new name (static)
            /// Same effect as GetName (the non-static version)
            /// </summary>
            /// <returns></returns>
            public static string GenerateName() => m_nameGenerator.GetName();

            #endregion
        }

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
        private static void ResolveImportedSchemas(XmlSchema xmlSchema, XmlSchemas schemas)
        {
            string baseSchemaName = NameGenerator.GenerateName();
            if (string.IsNullOrEmpty(xmlSchema.SourceUri))
            {
                xmlSchema.Id = baseSchemaName;
                xmlSchema.SourceUri = "file:///" + baseSchemaName + ".xsd";
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
                        string name = NameGenerator.GenerateName();
                        importSchema.Schema.Id = name;
                        importSchema.Schema.SourceUri = "file:///" + name + ".xsd";
                        importSchema.SchemaLocation = name + ".xsd";

                    }
                }

                // resolve any included schemas in the external schema
                ResolveImportedSchemas(externalSchema.Schema, schemas);
            }

        }

        /// <summary>
        /// Convert a collection of schemas to an array of schemas
        /// </summary>
        /// <param name="schemas"></param>
        /// <returns></returns>
        private static IList<XmlSchema> XmlSchemasToArray(XmlSchemas schemas)
        {
            List<XmlSchema> schemaCollection = new List<XmlSchema>();
            schemaCollection.AddRange(schemas);
            return schemaCollection;
        }

        #endregion

        #endregion

        #region calling methods
        private static Type GetParameterType(ParameterInfo info)
        {
            return (info.ParameterType.IsByRef) ? info.ParameterType.GetElementType() : info.ParameterType;
        }

        public static object MethodOutputToObject(MethodInfo methodInfo, IList<object> returnObjects, Type type)
        {
            if (type == null) return null;

            List<ParameterInfo> parameters = GetOutputParameters(methodInfo);
            returnObjects = GetReturnObjects(methodInfo, returnObjects);

            object obj;// = Activator.CreateInstance(type);

            if (parameters.Count == 1 && !Utility.IsPrimitive(parameters[0].ParameterType))
            {
                obj = returnObjects[0];
            }
            else
            {
                obj = Activator.CreateInstance(type);

                FieldInfo[] fields = type.GetFields();

                Trace.Assert(fields.Length == returnObjects.Count);

                int i = 0;
                foreach (object fieldValue in returnObjects)
                {
                    FieldInfo field = fields[i];
                    field.SetValue(obj, fieldValue);
                    i++;
                }

            }

            return obj;
        }

        /// <summary>
        /// Create the output document from the output parameters
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="returnObjects"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XmlDocument MethodOutputToXml(MethodInfo methodInfo, IList<object> returnObjects, Type type)
        {
            object obj = MethodOutputToObject(methodInfo, returnObjects, type);

            using (StringWriter sw = new StringWriter())
            {
                new XmlSerializer(type).Serialize(sw, obj);

                XmlDocument document = new XmlDocument();
                document.LoadXml(sw.ToString());
                return document;
            }
        }

        /// <summary>
        /// Take the Xml Input document and assign it to the parameters
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="type"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static object[] XmlToMethodInput(MethodInfo methodInfo, Type type, XmlDocument document)
        {
            if (methodInfo == null) return null;

            // copy array of parameters
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
                parameters.Add(parameter);

            int numParameters = methodInfo.GetParameters().Length;

            // check boundary conditions
            if (numParameters == 0) return null;
            if (document == null) return new object[numParameters];

            // create parameter object array
            object[] parameterObjects = new object[numParameters];

            // deserialize the XmlDocument into the generated type
            XmlSerializer deserializer = new XmlSerializer(type.IsByRef?type.GetElementType():type);

            object obj;
            using(StringReader sr = new StringReader(document.OuterXml))
                obj = deserializer.Deserialize(sr);

            // get non-output parameters
            List<ParameterInfo> nonOutputParameters = GetNonOutputParameters(methodInfo);

            if (nonOutputParameters.Count == 1 &&
                !Utility.IsPrimitive(parameters[0].ParameterType) &&
                !parameters[0].IsOut)
            {
                parameterObjects[0] =  obj;
            }
            else
            {
                int index = 0;
                foreach (FieldInfo field in type.GetFields())
                {
                    parameterObjects[index] = field.GetValue(obj);
                    index++;
                }
            }

            return parameterObjects;
        }

        /// <summary>
        /// Get all parameters except output only parameters for the given method
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private static List<ParameterInfo> GetNonOutputParameters(MethodInfo methodInfo)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();

            // count how many output parameter there are
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                // if this parameter is of type void then it has no value
                if (parameter.ParameterType == typeof(void))
                    continue;

                if (!parameter.IsOut)
                    parameters.Add(parameter);
            }

            return parameters;
        }

        /// <summary>
        /// Get the output parameters from a method.
        /// The outputs are the return value of the method plus any paramters that
        /// are passed by reference.
        /// </summary>
        /// <param name="methodInfo">MethodInfo for method to process</param>
        /// <returns>List of paramters that are outputs of this method</returns>
        public static List<ParameterInfo> GetOutputParameters(MethodInfo methodInfo)
        {
            List<ParameterInfo> outputParameters = new List<ParameterInfo>();

            // count how many output parameter there are
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                // if this parameter is of type void then it has no value
                if (parameter.ParameterType == typeof(void))
                    continue;

                if (parameter.IsOut || parameter.ParameterType.IsByRef)
                    outputParameters.Add(parameter);
            }

            // return type is added LAST
            if (methodInfo.ReturnParameter.ParameterType != typeof(void))
                outputParameters.Add(methodInfo.ReturnParameter);

            return outputParameters;
        }

        /// <summary>
        /// Get the input parameters for a method
        /// </summary>
        /// <param name="methodInfo">The method to get the parameters for</param>
        /// <returns>List of the parameters</returns>
        public static List<ParameterInfo> GetInputParameters(MethodInfo methodInfo)
        {
            // create a list of parameters
            List<ParameterInfo> inputParameters = new List<ParameterInfo>();

            // only add input parameter to the list
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                // output parameters can't be used as input
                if (!parameter.IsOut)
                    inputParameters.Add(parameter);
            }

            return inputParameters;
        }

        /// <summary>
        /// Convert a list of parameters to a schema that describes the parameters.
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <param name="methodInfo">Method </param>
        /// <returns></returns>
        public static XmlSchema ConvertParametersToSchema(List<ParameterInfo> parameters, MethodInfo methodInfo)
        {
            if (parameters.Count == 0)
                return null;

            Type parameterType = MethodCallTools.CreateTypeFromParameters(parameters, methodInfo);

            IList<XmlSchema> schemas = MethodCallTools.GenerateSchema(parameterType);

            XmlSchema wrappedSchema = schemas[0];

            return wrappedSchema;
        }

        /// <summary>
        /// Create a type from an array of parameterInfo
        /// This creates a simple class that contains the public fields, one for each parameter in
        /// the array of parameterInfo
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static Type CreateTypeFromParameters(List<ParameterInfo> parameters, MethodInfo methodInfo)
        {
            // if there is only one parameter, 
            // then just return that type
            if (parameters.Count == 1 && !Utility.IsPrimitive(parameters[0].ParameterType))
                return parameters[0].ParameterType;

            try
            {
                AppDomain myDomain = Thread.GetDomain();
                AssemblyName myAsmName = new AssemblyName
                {
                    Name = "TemporaryTypeAssembly"
                };

                AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndSave);

                ModuleBuilder IntVectorModule = myAsmBuilder.DefineDynamicModule("TemporaryTypeModule", "TemporaryType.dll");

                TypeBuilder typeBuilder = IntVectorModule.DefineType(
                    methodInfo.Name,
                    TypeAttributes.Public |
                    TypeAttributes.SequentialLayout |
                    TypeAttributes.Serializable);

                foreach (ParameterInfo info in parameters)
                {
                    Type paramType = GetParameterType(info);

                    string name = info.Name;

                    if (info.Position == -1) // this is the return value
                        name = "Return";

                    if (name == null)
                        name = "Parameter" + info.Position.ToString();

                    typeBuilder.DefineField(name, paramType, FieldAttributes.Public);
                }


                Type type = null;

                type = typeBuilder.CreateType();

                object obj = Activator.CreateInstance(type);

                return type;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                //ExceptionManager.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Convert a list of parameters to an array of schemas that describe the parameters
        /// </summary>
        /// <param name="parameters">Parameters to convert</param>
        /// <returns>Array of schemas</returns>
        public static IList<XmlSchema> ConvertParametersToSchemas(List<ParameterInfo> parameters)
        {
            // create an array of types
            Type[] types = new Type[parameters.Count];

            int i = 0;

            foreach (ParameterInfo parameter in parameters)
            {
                // if the type is by reference, then get it's base type
                types[i] = parameter.ParameterType.IsByRef ?
                    parameter.ParameterType.GetElementType() :
                    parameter.ParameterType;
                i++;
            }

            return MethodCallTools.GenerateSchemas(types);
        }

        /// <summary>
        /// Figure out which of the return objects is a "out" or "ref" parameter
        /// </summary>
        /// <param name="m_methodInfo"></param>
        /// <param name="parameterObjects"></param>
        /// <returns></returns>
        public static IList<object> GetReturnObjects(MethodInfo methodInfo, IList<object> parameterObjects)
        {
            List<object> returnObjects = new List<object>();
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.AddRange(methodInfo.GetParameters());
            parameters.Add(methodInfo.ReturnParameter);

            foreach (ParameterInfo parameterInfo in parameters)
            {
                // If this is not an "out" parameter (the result of a computation)
                // or it is not at parameter position -1 (indicating that it is the 
                // return value of a method) then we don;t care what the value is 
                // becuase it's not the output from this method
                if (!(parameterInfo.IsOut || parameterInfo.ParameterType.IsByRef) && parameterInfo.Position != -1)
                    continue;

                // if this parameter is of type void then it has no value
                if (parameterInfo.ParameterType == typeof(void))
                    continue;

                int index = parameters.IndexOf(parameterInfo);
                object obj = parameterObjects[index];
                returnObjects.Add(obj);
            }

            return returnObjects;
        }

        #endregion

#if DEBUG
        public static string SchemaToText(XmlSchema schema)
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
