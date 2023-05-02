//=============================================================================
//
// Copyright (C) 2013 Michael Coyle, Blue Toque
// Copyright (C) 2023 Michael Coyle, Blue Toque Software
// http://www.bluetoque.ca/products/xsdtoclasses/
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
using System.CodeDom;
using CodeGeneration.CodeModifiers;

namespace BlueToque.XmlLibrary.CodeModifiers
{
    /// <summary>
    /// Implement attributes for Google's Protcol buffers as implemented by protobuf.net
    /// http://code.google.com/p/protobuf-net/
    /// </summary>
    public class Protobuf : BaseCodeModifier
    {
        public Protobuf() : base("Protobuf") { }

        #region ICodeModifier Members
        public override void Execute(CodeNamespace codeNamespace)
        {
            // for each type
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (type.IsEnum) continue;

                // add the "ProtoContract" attribute to the class
                type.CustomAttributes.Add(new CodeAttributeDeclaration("ProtoContract"));

                int number = 1;

                // for each member
                foreach (CodeTypeMember member in type.Members)
                {
                    // if the member is a property
                    if (!(member is CodeMemberProperty codeProperty))
                        continue;

                    // if the member is XmlElement
                    if (codeProperty.Type.BaseType == "System.Xml.XmlElement")
                        return;

                    // add the custom type editor attribute
                    CodeAttributeDeclaration attr = new CodeAttributeDeclaration("ProtoMember");
                    attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(number)));
                    codeProperty.CustomAttributes.Add(attr);

                    number++;
                }
            }

            // add an include for "ProtoBuf" which is th namespace of the ProtoContract and ProtoMember attributes
            codeNamespace.Imports.Add(new CodeNamespaceImport("ProtoBuf"));
        }

        #endregion
    }
}
