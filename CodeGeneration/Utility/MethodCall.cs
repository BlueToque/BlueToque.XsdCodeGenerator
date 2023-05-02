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
using CodeGeneration.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CodeGeneration
{
    /// <summary>
    /// Call a method
    /// </summary>
    public class MethodCall : IMethodCall
    {
        /// <summary>
        /// Assumptions: 
        ///     assembly that methodInfo is in has been loaded
        ///     the class the methodinfo is in has a parameterless constructor        
        /// </summary>
        /// <param name="methodInfo"></param>
        public MethodCall(MethodInfo methodInfo, string identifier)
        {
            m_identifier = identifier;
            m_methodInfo = methodInfo;
            m_object = Activator.CreateInstance(m_methodInfo.DeclaringType);

            ClassXsdGenerator generator = new ClassXsdGenerator();
            generator.NameGenerator += new NameGeneratorDelegate(Generator_NameGenerator);
            m_baseName = "input";

            List<ParameterInfo> inputParameters = MethodCallTools.GetInputParameters(m_methodInfo);
            m_inputType = MethodCallTools.CreateTypeFromParameters(inputParameters, m_methodInfo);
            m_inputSchemas = generator.GenerateSchema(m_inputType);
            m_inputSchema = m_inputSchemas[0];

            generator = new ClassXsdGenerator();
            generator.NameGenerator += new NameGeneratorDelegate(Generator_NameGenerator);
            m_baseName = "output";

            List<ParameterInfo> outputParameters = MethodCallTools.GetOutputParameters(m_methodInfo);
            m_outputType = MethodCallTools.CreateTypeFromParameters(outputParameters, m_methodInfo);
            m_outputSchemas = generator.GenerateSchema(m_outputType);
            m_outputSchema = m_outputSchemas[0];
        }

        readonly string m_baseName = "parameter";

        string Generator_NameGenerator(int index) => string.Format(
                "G:\\{0}_{1}\\Generated\\{2}{3}.xsd",
                m_methodInfo.Name,
                m_identifier,
                m_baseName,
                index);

        /// <summary>
        /// Copy constructor, used by the aggregate web method node
        /// </summary>
        /// <param name="methodCall"></param>
        public MethodCall(IMethodCall methodCall)
        {
            m_methodInfo = methodCall.MethodInfo;
            m_object = Activator.CreateInstance(m_methodInfo.DeclaringType);

            m_inputType = methodCall.InputType;
            m_inputSchemas = methodCall.InputSchemas;
            m_inputSchema = m_inputSchemas[0];

            m_outputType = methodCall.OutputType;
            m_outputSchemas = methodCall.OutputSchemas;
            m_outputSchema = m_outputSchemas[0];
        }

        #region fields
        protected MethodInfo m_methodInfo;
        readonly string m_identifier;
        protected object m_object;

        protected Type m_inputType;
        protected XmlSchema m_inputSchema;
        readonly IList<XmlSchema> m_inputSchemas;

        protected Type m_outputType;
        protected XmlSchema m_outputSchema;
        readonly IList<XmlSchema> m_outputSchemas;

        protected object m_returnObject = null;

        #endregion

        #region properties
        public MethodInfo MethodInfo => m_methodInfo;

        public object Object => m_object;

        public XmlSchema InputSchema => m_inputSchema;

        public IList<XmlSchema> InputSchemas => m_inputSchemas;

        public Type InputType => m_inputType;

        public XmlSchema OutputSchema => m_outputSchema;

        public IList<XmlSchema> OutputSchemas => m_outputSchemas;

        public Type OutputType => m_outputType;

        public object ReturnObject => m_returnObject;

        #endregion

        /// <summary>
        /// Take an XMLDocument that conforms to the input schema and call the method
        /// return and XML Document that conforms to the output schema.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public virtual XmlDocument Execute(XmlDocument document)
        {
            // assign the XML data to the parameter objects
            object[] parameterObjects = MethodCallTools.XmlToMethodInput(m_methodInfo, m_inputType, document);

            // invoke the method
            object returnObject = m_methodInfo.Invoke(m_object, parameterObjects);

            List<object> returnObjects = new List<object>();
            if (parameterObjects != null)
                returnObjects.AddRange(parameterObjects);
            returnObjects.Add(returnObject);

            m_returnObject = MethodCallTools.MethodOutputToObject(m_methodInfo, returnObjects, m_outputType);
            string results = Serialize(m_outputType.IsByRef ? m_outputType.GetElementType() : m_outputType, m_returnObject);

            // make sure to create a NEW document here becuase otherwise callers from different
            // threads will get a modified document
            document = new XmlDocument();
            document.LoadXml(results);
            return document;
        }

        public static string Serialize(Type type, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

    }
}
