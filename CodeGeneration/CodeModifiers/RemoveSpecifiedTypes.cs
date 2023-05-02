//=============================================================================
//
// Copyright (C) 2007 Michael Coyle, Blue Toque
// Copyright (C) 2023 Michael Coyle, Blue Toque Software
// http://www.BlueToque.ca/Products/XmlGridControl2.html
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
using System.CodeDom;
using System.Xml;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// The collection type converter give a slightly more useable view of a collection
    /// in the property grid
    /// </summary>
    public class RemoveSpecifiedTypes : BaseCodeModifier
    {
        public RemoveSpecifiedTypes() : base("Remove Specified Types") { }

        public RemoveSpecifiedTypesOptions Options { get; set; } = new RemoveSpecifiedTypesOptions();

        public override XmlElement XmlOptions
        {
            get => Serializer.SerializeToElement(Options);
            set { Options = Serializer.Deserialize<RemoveSpecifiedTypesOptions>(value); }
        }

        #region ICodeModifier Members

        public override void Execute(CodeNamespace codeNamespace)
        {
            CodeTypeDeclarationCollection toRemove = new CodeTypeDeclarationCollection();

            // foreach datatype in the codeNamespace
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (Options.Type.ContainsName(codeNamespace.Name+"."+type.Name) || Options.Type.ContainsName(type.Name))
                    toRemove.Add(type);
            }

            codeNamespace.Types.RemoveAll(toRemove);
        }

        #endregion
    }
}
