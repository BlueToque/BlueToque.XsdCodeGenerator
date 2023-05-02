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
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;

namespace CodeGeneration.ImporterExtensions
{

    /// <summary>
    /// This class is an attempt to import documentation for each type and member
    /// in the schema
    /// </summary>
    public class AnnotationTypeExtension: SchemaImporterExtension
    {
        //public override string ImportSchemaType(
        //    string name, 
        //    string ns, 
        //    XmlSchemaObject context, 
        //    XmlSchemas schemas, 
        //    XmlSchemaImporter importer,
        //    CodeCompileUnit compileUnit, 
        //    CodeNamespace mainNamespace, 
        //    CodeGenerationOptions options, 
        //    CodeDomProvider codeProvider)
        //{

        //    XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) schemas.Find(new XmlQualifiedName(name, ns), typeof(XmlSchemaSimpleType));
        //    return ImportSchemaType(
        //        simpleType, 
        //        context, 
        //        schemas, 
        //        importer, 
        //        compileUnit, 
        //        mainNamespace, 
        //        options, 
        //        codeProvider);
        //}

        public override string ImportSchemaType(
            XmlSchemaType type, 
            XmlSchemaObject context, 
            XmlSchemas schemas, 
            XmlSchemaImporter importer,
            CodeCompileUnit compileUnit, 
            CodeNamespace mainNamespace, 
            CodeGenerationOptions options, 
            CodeDomProvider codeProvider)
        {

            if (!(type is XmlSchemaAnnotated annotatedType))
                return null;

            if (annotatedType.Annotation == null)
                return null;

            // create the comments and add them to the hash table under the namespace of the object
            CreateComments(annotatedType);

            //mainNamespace.Types.

            return null;

        }

        private void CreateComments(XmlSchemaAnnotated annotatedType) => throw new Exception("The method or operation is not implemented.");

        //public override CodeExpression ImportDefaultValue(string value, string clrTypeName)
        //{
        //    if (clrTypes[clrTypeName] != null)
        //    {
        //        return CodeDomHelper.New(clrTypeName, new CodeExpression[] { CodeDomHelper.Primitive(value) });
        //    }
        //    return null;
        //}


    }
}
