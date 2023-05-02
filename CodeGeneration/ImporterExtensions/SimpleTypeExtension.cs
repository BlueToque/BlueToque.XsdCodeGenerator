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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;

namespace CodeGeneration.ImporterExtensions
{
    /// <summary>
    /// When you import an XSD you can extend the default importer with extensions.
    /// This extension attempts to add some utility to the SimpleTypes
    /// If there's a constriction facet, we can try to limit the length of a string, or the 
    /// value of an int.
    /// </summary>
    public class SimpleTypeExtension : SchemaImporterExtension
    {
        readonly Dictionary<XmlSchemaType, string> m_generatedTypes = new Dictionary<XmlSchemaType, string>();
        readonly Hashtable m_clrTypes = new Hashtable();

        public override string ImportSchemaType(
            string name, 
            string ns, 
            XmlSchemaObject context, 
            XmlSchemas schemas, 
            XmlSchemaImporter importer,
            CodeCompileUnit compileUnit, 
            CodeNamespace mainNamespace, 
            CodeGenerationOptions options, 
            CodeDomProvider codeProvider)
        {

            XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) schemas.Find(new XmlQualifiedName(name, ns), typeof(XmlSchemaSimpleType));
            return ImportSchemaType(
                simpleType, 
                context, 
                schemas, 
                importer, 
                compileUnit, 
                mainNamespace, 
                options, 
                codeProvider);
        }

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

            if (!(type is XmlSchemaSimpleType simpleType))
                return null;

            if (m_generatedTypes.TryGetValue(simpleType, out string typeName))
                return typeName;

            if (!(simpleType.Content is XmlSchemaSimpleTypeRestriction restriction))
                return null;

            // genetate type only for xs:string restrictions
            if (restriction.BaseTypeName.Name != "string" || restriction.BaseTypeName.Namespace != XmlSchema.Namespace)
                return null;

            // does not generate custom type if it has any enumeration facets
            foreach (object o in restriction.Facets)
            {
                if (o is XmlSchemaEnumerationFacet)
                    return null;
            }

            typeName = GenerateSimpleType(simpleType, compileUnit, mainNamespace, options, codeProvider);

            // add generated type information to the cache to avoid generating type al the time
            if (typeName != null)
            {
                m_generatedTypes[simpleType] = typeName;
                m_clrTypes.Add(typeName, typeName);
            }

            return typeName;
        }

        public override CodeExpression ImportDefaultValue(string value, string clrTypeName) =>
            m_clrTypes[clrTypeName] != null
                ? CodeDomHelper.New(clrTypeName, new CodeExpression[] { CodeDomHelper.Primitive(value) })
                : null;

        internal string GenerateSimpleType(
            XmlSchemaSimpleType type, 
            CodeCompileUnit compileUnit, 
            CodeNamespace mainNamespace, 
            CodeGenerationOptions options, 
            CodeDomProvider codeProvider)
        {
            CodeTypeDeclaration codeClass = CodeDomHelper.CreateClassDeclaration(type);
            mainNamespace.Types.Add(codeClass);

            CodeDomHelper.GenerateXmlTypeAttribute(codeClass, type.QualifiedName.Name, type.QualifiedName.Namespace);

            XmlSchemaSimpleTypeRestriction restriction = type.Content as XmlSchemaSimpleTypeRestriction;

            Type baseType = XsdToClrPrimitive(restriction.BaseTypeName);
            CodeMemberField field = CodeDomHelper.AddField(codeClass, "value", baseType);

            CodeMemberProperty prop = CodeDomHelper.AddPropertyDeclaration(codeClass, field, "Value", baseType);
            CodeDomHelper.AddTextAttribute(prop.CustomAttributes);

            CodeDomHelper.AddCtor(codeClass);
            CodeDomHelper.AddCtor(codeClass, prop);

            // for each facet we support, add validation to the setter
            foreach (object facet in restriction.Facets)
            {
                if (facet is XmlSchemaLengthFacet length)
                {
                    int? value = ToInt32(length.Value);
                    if (value.HasValue)
                    {
                        CodeExpression valueLength = CodeDomHelper.Property(CodeDomHelper.Value(), "Length");
                        CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(valueLength, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(value));
                        prop.SetStatements.Add(new CodeConditionStatement(condition, new CodeStatement[] { CodeDomHelper.ThrowFacetViolation("length", value) }, new CodeStatement[0]));
                    }
                    continue;
                }

                if (facet is XmlSchemaPatternFacet pattern)
                {
                    // TODO: might want ot validate the pattern value here to make sure that it is a valid Regex.
                    if (!string.IsNullOrEmpty(pattern.Value))
                    {
                        CodeExpression patternMatch =
                            CodeDomHelper.MethodCall(
                                CodeDomHelper.TypeExpr(typeof(Regex)), "IsMatch", new CodeExpression[] { CodeDomHelper.Value(), CodeDomHelper.Primitive(pattern.Value) });

                        CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(patternMatch, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(true));
                        prop.SetStatements.Add(new CodeConditionStatement(condition, new CodeStatement[] { CodeDomHelper.ThrowFacetViolation("pattern", pattern.Value) }, new CodeStatement[0]));
                    }
                    continue;
                }

                if (facet is XmlSchemaMinLengthFacet minLength)
                {
                    int? value = ToInt32(minLength.Value);
                    if (value.HasValue)
                    {
                        CodeExpression valueLength = CodeDomHelper.Property(CodeDomHelper.Value(), "Length");
                        CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(valueLength, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(value));
                        prop.SetStatements.Add(new CodeConditionStatement(condition, new CodeStatement[] { CodeDomHelper.ThrowFacetViolation("minLength", value) }, new CodeStatement[0]));
                    }
                    continue;
                }

                if (facet is XmlSchemaMaxLengthFacet maxLength)
                {
                    int? value = ToInt32(maxLength.Value);
                    if (value.HasValue)
                    {
                        CodeExpression valueLength = CodeDomHelper.Property(CodeDomHelper.Value(), "Length");
                        CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(valueLength, CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(value));
                        prop.SetStatements.Add(new CodeConditionStatement(condition, new CodeStatement[] { CodeDomHelper.ThrowFacetViolation("maxLength", value) }, new CodeStatement[0]));
                    }
                    continue;
                }
            }

            //add ToSrting() Overload and implicit and explicit Cast operators for compatibilty woth previously generated code

            CodeMemberMethod toString = CodeDomHelper.MethodDecl(typeof(string), "ToString", MemberAttributes.Public | MemberAttributes.Override);
            toString.Statements.Add(CodeDomHelper.Return(CodeDomHelper.Property("value")));
            codeClass.Members.Add(toString);

            // Unfortunately CodeDom does not support operators, so we have to use CodeSnippet to generate the Cast operators
            // CodeSnippet is not language aware, so we have to use different snippets for different providers
            // this version only support c# syntax
            if (codeProvider is Microsoft.CSharp.CSharpCodeProvider)
            {
                string implicitCast = string.Format("    public static implicit operator {0}({1} x) {{ return new {0}(x); }}", codeClass.Name, baseType.FullName);
                CodeSnippetTypeMember implicitOp = new CodeSnippetTypeMember(implicitCast);
                codeClass.Members.Add(implicitOp);

                string explicitCast = string.Format("    public static explicit operator {1}({0} x) {{ return x.Value; }}", codeClass.Name, baseType.FullName);
                CodeSnippetTypeMember explicitOp = new CodeSnippetTypeMember(explicitCast);
                codeClass.Members.Add(explicitOp);
            }

            return codeClass.Name;
        }

        Type XsdToClrPrimitive(XmlQualifiedName qname)
        {
            if (qname.Namespace == XmlSchema.Namespace)
            {
                switch (qname.Name)
                {
                    case "string": return typeof(string);
                    case "int": return typeof(int);
                    case "integer": return typeof(int);
                    case "byte": return typeof(byte);
                    case "boolean": return typeof(bool);
                    case "decimal": return typeof(decimal);
                    case "double": return typeof(double);

                    case "float": return typeof(float);
                    case "long": return typeof(long);
                    

                    // TODO: Add all xsd primitives
                }
            }

            throw new InvalidOperationException($"Cannot map XSD type '{qname}' to CLR primitive type.");
        }

        int? ToInt32(string value)
        {
            int? i;
            try
            {
                i = XmlConvert.ToInt32(value);
            }
            catch
            {
                return null;
            }
            return i;
        }
    }


}