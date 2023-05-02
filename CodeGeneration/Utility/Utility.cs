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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Schema;

namespace CodeGeneration
{
    /// <summary>
    /// Helper methods
    /// </summary>
    public static class Utility
    {
        public static void RemoveAll(this CodeTypeDeclarationCollection collection, CodeTypeDeclarationCollection toRemove)
        {
            foreach (CodeTypeDeclaration type in toRemove)
                collection.Remove(type);
        }

        /// <summary>
        /// Compile an XmlSchema
        /// </summary>
        /// <param name="schema"></param>
        internal static void CompileSchema(XmlSchema schema)
        {
            if (schema.IsCompiled) return;
            Trace.TraceInformation("Compiling schema");
            XmlSchemaSet set = new XmlSchemaSet();
            set.Add(schema);
            set.CompilationSettings.EnableUpaCheck = false;
            set.Compile();
        }

        /// <summary>
        /// Is this type a primitive type?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsPrimitive(Type type) => type.IsPrimitive || IsPrimitive(type.FullName);

        /// <summary>
        /// Is the type represented by this string a primitive type?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsPrimitive(string type)
        {
            switch (type)
            {
                case "System.Char":
                case "System.Char&":

                case "System.Byte":
                case "System.Byte&":

                case "System.Int16":
                case "System.Int16&":

                case "System.Int32":
                case "System.Int32&":

                case "System.Int64":
                case "System.Int64&":

                case "System.UInt16":
                case "System.UInt16&":

                case "System.UInt32":
                case "System.UInt32&":

                case "System.UInt64":
                case "System.UInt64&":

                case "System.Boolean":
                case "System.Boolean&":

                case "System.Decimal":
                case "System.Decimal&":

                case "System.Single":
                case "System.Single&":

                case "System.Double":
                case "System.Double&":

                case "System.String":
                case "System.String&":

                case "System.DateTime":
                case "System.DateTime&":

                case "System.TimeSpan":
                case "System.TimeSpan&":

                case "System.Xml.XmlElement":
                case "System.Xml.XmlAttribute":

                case "System.Object":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Takes a candidate c# identifier and makes it valid by removing whitespace, 
        /// checking collision with c# keywords, and ensuring the composing characters 
        /// are valid identifier characters.
        /// </summary>
        /// <param name="candidate">the candidate identifier</param>
        /// <returns>a valid c# identifier</returns>
        public static string GetValidCSharpIdentifier(string candidate)
        {
            // return CodeIdentifier.MakeValid(candidate);

            const char underscore = '_';
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            //if the candidate is null, set it to a single underscore.
            if (candidate == null)
                candidate = underscore.ToString();

            //remove any spaces
            candidate = candidate.Replace(" ", string.Empty);
            StringBuilder attempt;
            try
            {
                //the CreateValidIdentifier() call only checks collisions with reserved keywords
                attempt = new StringBuilder(provider.CreateValidIdentifier(candidate));
                if (attempt.Length < 1)
                    attempt = new StringBuilder(new string(underscore, 1));
            }
            catch (Exception ex)
            {
                attempt = new StringBuilder(new string(underscore, 1));
                Trace.TraceWarning(ex.ToString());
            }

            //loop through the string, ensuring that each character is a letter, digit or underscore
            for (int index = 0; index < attempt.Length; index++)
            {
                if (!char.IsLetterOrDigit(attempt[index]) && attempt[index] != underscore)
                    attempt[index] = underscore;
            }

            //finally check that the first digit is a letter or underscore
            if (char.IsDigit(attempt[0]))
                attempt.Insert(0, underscore);
            return attempt.ToString();
        }

        /// <summary>
        /// Used in the method <see cref="InstantiateAllMembers"/>
        /// </summary>
        static readonly Stack<Type> m_instantiatedTypes = new Stack<Type>();

        /// <summary>
        /// Create instances of all of the types in the given object
        /// This is useful for non-value types when you're creating a GUI.
        /// </summary>
        /// <param name="obj"></param>
        public static void InstantiateAllMembers(object obj)
        {
            if (obj == null) return;

            if (obj.GetType().IsClass)
                m_instantiatedTypes.Push(obj.GetType());

            Type type = obj.GetType();

            #region loops through the properies ad use recusion to instantiate them
            foreach (PropertyInfo property in type.GetProperties())
            {
                // if we've already instantiated an object of this type don't bother
                if (m_instantiatedTypes.Contains(property.PropertyType))
                    continue;

                try
                {
                    // this property is an array
                    if (property.GetIndexParameters().Length > 0)
                        continue;

                    // if there's already a value then we don't create a new one
                    object value = property.GetValue(obj, null);
                    if (value != null)
                        continue;
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation(ex.ToString());
                }

                Type propertyType = property.PropertyType;

                // check to see if we have a parameterless constructor
                if (propertyType.GetConstructor(new Type[0] { }) == null)
                    continue;

                // the magic happens here
                object propertyObject = Activator.CreateInstance(propertyType);

                property.SetValue(obj, propertyObject, null);

                InstantiateAllMembers(propertyObject);

            }
            #endregion

            if (obj.GetType().IsClass)
                m_instantiatedTypes.Pop();

        }

        /// <summary>
        /// Return true if a type is a nullable type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsNullable(Type type) => (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));

        /// <summary>
        /// return true if a type string is a nullable type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsNullable(string type) => (type == "System.Nullable`1");

        /// <summary>
        /// Get the string that represents the element type.
        /// If the element type is nullable, get the string that represents the contained type
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        internal static string GetCollectionType(CodeTypeReference elementType) =>
            (IsNullable(elementType.BaseType)) ? GetNullableTypeName(elementType) + "?" : elementType.BaseType;

        /// <summary>
        /// Get the type contained by a collection
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        internal static string GetCollectionContainedType(CodeTypeReference elementType) =>
            (IsNullable(elementType.BaseType)) ? GetNullableTypeName(elementType) : elementType.BaseType;

        /// <summary>
        /// Get a collection name from a element type that is an array type, 
        /// including of the element type is a nullable type
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        internal static string GetCollectionName(CodeTypeReference elementType) =>
            Utility.GetValidCSharpIdentifier(GetCollectionContainedType(elementType) + "Collection");

        /// <summary>
        /// If an element type is a nullable type return the contained type
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        internal static string GetNullableTypeName(CodeTypeReference elementType) =>
            elementType.TypeArguments[0].BaseType;

        /// <summary>
        /// Get the Xml Namespace declaration from the XmlTypeAttribute
        /// [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/TrueNorth/Geographic")]
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string GetXmlNamespace(CodeTypeDeclaration type)
        {
            // for each attribute
            foreach (CodeAttributeDeclaration cad in type.CustomAttributes)
            {
                if (cad.Name != "System.Xml.Serialization.XmlTypeAttribute")
                    continue;

                // for each argument
                foreach (CodeAttributeArgument caa in cad.Arguments)
                {
                    if (caa.Name != "Namespace")
                        continue;

                    // get the code primitive
                    CodePrimitiveExpression cpe = (caa.Value as CodePrimitiveExpression);
                    if (cpe == null)
                        continue;

                    if (!(cpe.Value is string))
                        continue;

                    return cpe.Value as string;
                }

            }
            return null;
        }

        /// <summary>
        /// Find the declaration of a type from it's reference
        /// </summary>
        /// <param name="codeNamespace"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal static CodeTypeDeclaration FindDeclaration(CodeNamespace codeNamespace, CodeTypeReference reference)
        {
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (type.Name == reference.BaseType)
                    return type;
            }
            return null;
        }
    }
}
