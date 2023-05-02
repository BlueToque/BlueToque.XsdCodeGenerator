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
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CodeGeneration.ImporterExtensions
{
    /// <summary>
    /// Helper class for the CodeDom
    /// </summary>
    internal class CodeDomHelper
    {
        //const string FacetViolationException = "Cannot set value to '{{0}}'. Constraining facet violation: {0}='{1}'.";

        internal static CodeTypeDeclaration CreateClassDeclaration(XmlSchemaType type)
        {
            string className = CodeIdentifier.MakeValid(type.QualifiedName.Name);
            CodeTypeDeclaration codeClass = new CodeTypeDeclaration(className);
            codeClass.TypeAttributes |= TypeAttributes.Public;

            return codeClass;
        }

        internal static void GenerateXmlTypeAttribute(CodeTypeDeclaration codeClass, string name, string ns)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration(typeof(XmlTypeAttribute).FullName);
            if (name == null || name.Length == 0)
            {
                attribute.Arguments.Add(new CodeAttributeArgument("AnonymousType", new CodePrimitiveExpression(true)));
            }
            else
            {
                if (codeClass.Name != name)
                    attribute.Arguments.Add(new CodeAttributeArgument("TypeName", new CodePrimitiveExpression(name)));

                if (ns != null && ns.Length != 0)
                    attribute.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(ns)));
            }

            if (attribute.Arguments.Count > 0)
                codeClass.CustomAttributes.Add(attribute);
        }

        /// <summary>
        /// Add a text attribute
        /// </summary>
        /// <param name="metadata"></param>
        internal static void AddTextAttribute(CodeAttributeDeclarationCollection metadata) => 
            metadata.Add(new CodeAttributeDeclaration(typeof(XmlTextAttribute).FullName));

        /// <summary>
        /// Add a field to a codeTypeDeclaration
        /// </summary>
        /// <param name="codeClass"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static CodeMemberField AddField(CodeTypeDeclaration codeClass, string name, Type type) =>
            AddField(codeClass, name, type.FullName);

        /// <summary>
        /// Add a field to a CodeTypeDeclaration
        /// </summary>
        /// <param name="codeClass"></param>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal static CodeMemberField AddField(CodeTypeDeclaration codeClass, string name, string typeName)
        {
            CodeMemberField field = new CodeMemberField(typeName, name)
            {
                Attributes = MemberAttributes.Private
            };

            codeClass.Members.Add(field);

            return field;
        }

        /// <summary>
        /// Add a property to a CodeTypeDeclaration
        /// </summary>
        /// <param name="codeClass"></param>
        /// <param name="field"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static CodeMemberProperty AddPropertyDeclaration(CodeTypeDeclaration codeClass, CodeMemberField field, string name, Type type) =>
            AddPropertyDeclaration(codeClass, field, name, type.FullName);

        /// <summary>
        /// Add a Property to a CodeTypeDeclaration
        /// </summary>
        /// <param name="codeClass"></param>
        /// <param name="field"></param>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal static CodeMemberProperty AddPropertyDeclaration(CodeTypeDeclaration codeClass, CodeMemberField field, string name, string typeName)
        {
            CodeMemberProperty prop = CreatePropertyDeclaration(field, name, typeName);
            codeClass.Members.Add(prop);
            return prop;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal static CodeMemberProperty CreatePropertyDeclaration(CodeMemberField field, string name, string typeName)
        {
            CodeMemberProperty prop = new CodeMemberProperty
            {
                Type = new CodeTypeReference(typeName),
                Name = name,
            };
            prop.Attributes = (prop.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;

            CodeMethodReturnStatement ret = new CodeMethodReturnStatement
            {
                Expression = new CodeFieldReferenceExpression(This(), field.Name)
            };
            prop.GetStatements.Add(ret);

            AddInitializationStatement(prop.SetStatements, field, CodeDomHelper.Value());

            return prop;
        }

        /// <summary>
        /// Add a constructor to a CodeTypeDeclaration
        /// </summary>
        /// <param name="codeClass"></param>
        /// <returns></returns>
        internal static CodeConstructor AddCtor(CodeTypeDeclaration codeClass)
        {
            CodeConstructor ctor = new CodeConstructor();
            ctor.Attributes = (ctor.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;
            codeClass.Members.Add(ctor);
            return ctor;
        }

        /// <summary>
        /// Add a constructor to a CodeTypeDeclaration
        /// </summary>
        /// <param name="codeClass"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        internal static CodeConstructor AddCtor(CodeTypeDeclaration codeClass, CodeMemberProperty prop)
        {
            CodeConstructor ctor = AddCtor(codeClass);
            ctor.Parameters.Add(ParameterDecl(prop.Type.BaseType, "initValue"));
            AddInitializationStatement(ctor.Statements, prop, CodeDomHelper.Argument("initValue"));
            return ctor;
        }

        internal static CodeStatement ThrowFacetViolation(string facet, object value)
        {
            //"Cannot set value to '{0}'. Constraining facet violation: {1}='{3}'.";
            // generate string.Format call
            string exception = string.Format("Cannot set value to '{0}'. Constraining facet violation: {0}='{1}'.", facet, value);
            CodeExpression format = MethodCall(TypeExpr(typeof(string)), "Format", new CodeExpression[] { Primitive(exception), CodeDomHelper.Value() });
            return Throw(typeof(ArgumentException), new CodeExpression[] { format });
        }

        /// <summary>
        /// Add an initialization statement, usually to a constructor
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="field"></param>
        /// <param name="init"></param>
        internal static void AddInitializationStatement(CodeStatementCollection statements, CodeMemberField field, CodeExpression init) => 
            statements.Add(new CodeAssignStatement
            {
                Left = new CodeFieldReferenceExpression(This(), field.Name),
                Right = init
            });

        /// <summary>
        /// Add an initialization statement, usually to a constructor
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="prop"></param>
        /// <param name="init"></param>
        internal static void AddInitializationStatement(CodeStatementCollection statements, CodeMemberProperty prop, CodeExpression init) => 
            statements.Add(new CodeAssignStatement
            {
                Left = CodeDomHelper.Property(This(), prop.Name),
                Right = init
            });

        internal static CodeStatement Throw(Type exception, CodeExpression[] parameters) =>
            new CodeThrowExceptionStatement(New(exception, parameters));

        internal static CodeExpression MethodCall(CodeExpression targetObject, string methodName, CodeExpression[] parameters) =>
            new CodeMethodInvokeExpression(targetObject, methodName, parameters);

        internal static CodeExpression Argument(string argument) => new CodeArgumentReferenceExpression(argument);

        internal static CodeMemberMethod Method(CodeTypeReference type, string name, MemberAttributes attributes) => 
            new CodeMemberMethod
            {
                ReturnType = type,
                Name = name,
                Attributes = attributes
            };

        internal static CodeMemberMethod MethodDecl(Type type, string name, MemberAttributes attributes) => Method(Type(type), name, attributes);

        internal static CodeMemberMethod MethodDecl(string type, string name, MemberAttributes attributes) => Method(Type(type), name, attributes);

        internal static CodeExpression New(string type, CodeExpression[] parameters) => new CodeObjectCreateExpression(type, parameters);

        internal static CodeExpression New(Type type, CodeExpression[] parameters) => new CodeObjectCreateExpression(type, parameters);

        internal static CodeParameterDeclarationExpression ParameterDecl(string type, string name) => new CodeParameterDeclarationExpression(type, name);

        internal static CodeExpression Primitive(object primitive) => new CodePrimitiveExpression(primitive);

        internal static CodeExpression Property(string property) => new CodePropertyReferenceExpression(This(), property);

        internal static CodeExpression Property(CodeExpression exp, string property) => new CodePropertyReferenceExpression(exp, property);

        internal static CodeStatement Return(CodeExpression expr) => new CodeMethodReturnStatement(expr);

        internal static CodeExpression This() { return new CodeThisReferenceExpression(); }

        internal static CodeTypeReference Type(string type) => new CodeTypeReference(type);

        internal static CodeTypeReference Type(Type type) => new CodeTypeReference(type);

        internal static CodeTypeReferenceExpression TypeExpr(Type type) => new CodeTypeReferenceExpression(type);

        internal static CodeExpression Value() { return new CodePropertySetValueReferenceExpression(); }

        internal static CodeExpression Variable(string variable) => new CodeVariableReferenceExpression(variable);
    }

}
