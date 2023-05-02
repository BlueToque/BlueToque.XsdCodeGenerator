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
using System.Xml;
using BlueToque.XmlLibrary.CodeModifiers.Schemas;
using CodeGeneration.CodeModifiers;
using CodeGeneration.Internal;

namespace BlueToque.XmlLibrary.CodeModifiers
{
    /// <summary>
    /// Make a property in a class override
    /// </summary>
    public class OverrideProperty : BaseCodeModifier
    {
        public OverrideProperty() : base("Override Property") { }

        public OverridePropertyOptions Options { get; set; } = new OverridePropertyOptions();

        public override XmlElement XmlOptions
        {
            get => Serializer.SerializeToElement(Options);
            set { Options = Serializer.Deserialize<OverridePropertyOptions>(value); }
        }

        #region ICodeModifier Members
        public override void Execute(CodeNamespace codeNamespace)
        {
            if (Options == null || Options.Property == null || Options.Property.Count == 0)
                return;

            // foreach datatype in the codeNamespace
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {

                // if the qualified name doesn't start with the name of the class, continue.
                PropertyType propertyType = Options.Property.Find(x => x.QualifiedName.StartsWith(type.Name));
                if (propertyType == null)
                    continue;

                // for each property in the type
                foreach (CodeTypeMember member in type.Members)
                {
                    if (!(member is CodeMemberProperty))
                        continue;

                    CodeMemberProperty property = (member as CodeMemberProperty);
                    if (propertyType.QualifiedName.EndsWith(property.Name) ||
                        propertyType.QualifiedName.EndsWith("*"))
                        property.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                }
            }
        }

        #endregion
    }
}
