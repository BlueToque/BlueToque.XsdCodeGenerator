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
using CodeGeneration.Internal;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Xml;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// A CodeModifier to convert arrays to collections.
    /// </summary>
    public class ConvertArraysToCollections : BaseCodeModifier
    {
        public ConvertArraysToCollections() : base("Convert Arrays To Collections") { }

        /// <summary>
        /// Options for converting arrays to collections
        /// Classes to include and classes to explude from conversion
        /// </summary>
        public ConvertArraysToCollectionsOptions Options { get; set; } = new ConvertArraysToCollectionsOptions();    

        public override XmlElement XmlOptions { 
            get => Serializer.SerializeToElement(Options); 
            set { Options = Serializer.Deserialize<ConvertArraysToCollectionsOptions>(value); } }

        #region ICodeModifier Members

        public override void Execute(CodeNamespace codeNamespace)
        {
            Dictionary<string, CodeTypeDeclaration> collections = new Dictionary<string, CodeTypeDeclaration>();

            // foreach datatype in the codeNamespace
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (type.IsEnum) continue;

                #region get a list of all of the fields in a class
                List<CodeMemberField> fields = new List<CodeMemberField>();
                List<CodeMemberProperty> properties = new List<CodeMemberProperty>();

                foreach (CodeTypeMember member in type.Members)
                {
                    if (member is CodeMemberField codeField)
                        fields.Add(codeField);

                    if (member is CodeMemberProperty codeProperty)
                        properties.Add(codeProperty);
                }
                #endregion

                #region foreach field in the class, convert array fields to collections
                foreach (CodeMemberField field in fields)
                {
                    // get the array element's type so we can make a collection
                    CodeTypeReference elementType = field.Type.ArrayElementType;

                    if (elementType == null) continue;

                    // retrieve the declaration of the array element type
                    // so we can find it's namespoace
                    CodeTypeDeclaration elementDecl = Utility.FindDeclaration(codeNamespace, elementType);

                    // if this a type that's already loaded?
                    // if it's a loaded type and it's a primitive type,
                    Type baseElementType = Type.GetType(elementType.BaseType);
                    if (baseElementType != null && Utility.IsPrimitive(baseElementType)) continue;

                    CodeTypeDeclaration decl = null;

                    // The basetype can sometimes be a type like System.Xml.XmlElement (in the case
                    // where a schema has an "##any" element, .NET maps this to XmlElement)
                    // so we have to make sure there are no "." in the name
                    string className = Utility.GetCollectionName(elementType);

                    // if the collection class has already been declared, retrieve the declaration
                    // otherqise build a code declaraion for it
                    if (collections.ContainsKey(className))
                    {
                        decl = collections[className];
                    }
                    else
                    {
                        CodeTypeDeclarationCollection decls =
                            CollectionBuilder(
                                className,
                                Utility.GetCollectionType(elementType),
                                Utility.GetXmlNamespace(elementDecl));

                        decl = decls[0];
                        decl.Name = className;
                        collections.Add(decl.Name, decl);
                    }

                    field.Type.ArrayElementType = null;
                    field.Type.ArrayRank = 0;
                    field.Type.BaseType = decl.Name;
                }
                #endregion

                #region foreach property in the class, convert properties to return the new collections

                // do the same thing as for the fields, except don't create
                // a new collection if one does not already exist.
                // We will only change the properties to collections
                // if the collections already exist.
                foreach (CodeMemberProperty property in properties)
                {
                    CodeTypeReference elementType = property.Type.ArrayElementType;
                    if (elementType == null) continue;

                    //Type baseElementType = Type.GetType(elementType.BaseType);

                    if (elementType == null) continue;

                    //if (baseElementType == null || Utility.IsPrimitive(baseElementType))
                    //{
                    string className = Utility.GetCollectionName(elementType);
                    //string className = Utility.GetValidCSharpIdentifier(elementType.BaseType + "Collection");

                    if (!collections.ContainsKey(className))
                        continue;

                    CodeTypeDeclaration decl = collections[className];

                    property.Type.ArrayElementType = null;
                    property.Type.ArrayRank = 0;
                    property.Type.BaseType = decl.Name;
                    //}

                }
                #endregion
            }

            foreach (CodeTypeDeclaration decl in collections.Values)
            {
                // this code does nothing
                string collectionType = decl.Name;
                string elementType = collectionType.Replace("Collection", "");
                string elementName = elementType;
                if (elementType.IndexOf("@") == 0)
                    elementName = elementName.Substring(1);

                codeNamespace.Types.Add(decl);
            }
        }

        #endregion

        private static CodeTypeDeclarationCollection CollectionBuilder(string className, string classType, string ns)
        {
            // Declare a generic class.
            CodeTypeDeclaration newClass = new CodeTypeDeclaration
            {
                Name = className,
                IsPartial = true
            };

            newClass.BaseTypes.Add(new CodeTypeReference("System.Collections.Generic.List",
                                 new CodeTypeReference[] { new CodeTypeReference(classType) }));

            // add the Serializable attribute to the class
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("System.SerializableAttribute");
            newClass.CustomAttributes.Add(attribute);

            // add the Serializable attribute to the class
            CodeAttributeDeclaration xmlTypeAttribute = new CodeAttributeDeclaration("System.Xml.Serialization.XmlTypeAttribute");
            xmlTypeAttribute.Arguments.Add(new CodeAttributeArgument()
            {
                Name = "Namespace",
                Value = new CodePrimitiveExpression(ns)
            });

            newClass.CustomAttributes.Add(xmlTypeAttribute);

            return new CodeTypeDeclarationCollection { newClass };
        }
    }
}
