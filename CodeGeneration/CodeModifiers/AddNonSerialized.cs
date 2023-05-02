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
using System.CodeDom;
using System.Xml;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// Add non serialized attribute to members that should not be 
    /// serialized using a binary formatter.
    /// </summary>
    class AddNonSerialized : BaseCodeModifier
    {
        public AddNonSerialized() : base("Add Non Serialized") { }

        #region ICodeModifier Members

        public override void Execute(CodeNamespace codeNamespace)
        {
            // foreach datatype in the codeNamespace
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (type.IsEnum) continue;

                foreach (CodeTypeMember member in type.Members)
                {
                    if (!(member is CodeMemberField codeField))
                        continue;

                    // check if the Field is XmlElement
                    //"[System.ComponentModel.TypeConverter(typeof(ByteTypeConverter))]";
                    // add the custom type editor attribute
                    if (codeField.Type.BaseType == typeof(XmlElement).ToString())
                        codeField.CustomAttributes.Add(new CodeAttributeDeclaration("System.NonSerialized"));
                }
            }
        }

        #endregion
    }
}
