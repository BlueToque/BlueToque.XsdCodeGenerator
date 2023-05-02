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
using System.Linq;
using System.Xml;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// Add or remove an include directive
    ///     <CodeModifier Type="CodeGeneration.CodeModifiers.ModifyImports" Assembly="CodeGeneration, Version=1.0.1.0, Culture=neutral, PublicKeyToken=46a422f5b652f20b">
    ///         <ModifyImportsOptions xmlns="http://BlueToque.ca/CodeGeneration/CodeModifiers/ModifyImports.Options">
    ///                 <AddImport Name="ProtoBuf" />
    ///         </ModifyImportsOptions>
    ///     </CodeModifier>
    /// </summary>
    public class ModifyImports : BaseCodeModifier
    {
        public ModifyImports() : base("Modify Imports") { }

        public ModifyImportsOptions Options { get; set; } = new ModifyImportsOptions();

        public override XmlElement XmlOptions
        {
            get => Serializer.SerializeToElement(Options);
            set { Options = Serializer.Deserialize<ModifyImportsOptions>(value); }
        }

        #region ICodeModifier Members

        public override void Execute(CodeNamespace codeNamespace)
        {            
            foreach (ImportType import in Options.AddImport.Where(x => !string.IsNullOrEmpty(x.Name)))
                codeNamespace.Imports.Add(new CodeNamespaceImport(import.Name));
        }

        #endregion
    }
}
