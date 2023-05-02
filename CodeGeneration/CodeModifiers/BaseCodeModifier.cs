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
using CodeGeneration.Generators;
using System.CodeDom;
using System.Xml;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// A base class implementation of the ICodeModifier interface
    /// Contains basic utility methods.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class BaseCodeModifier : ICodeModifier
    {
        #region properties

        public string Name { get; }

        public virtual XmlElement XmlOptions { get; set; }

        public ICodeGenerator CodeGenerator { get; private set; }

        public CodeNamespace CodeNamespace { get; set; }

        #endregion

        public BaseCodeModifier(string name) => Name = name;
        
        /// <summary>
        /// Execute the code modifier on the given namespace
        /// </summary>
        /// <param name="codeNamespace"></param>
        public virtual void Execute(CodeNamespace codeNamespace) => Execute(codeNamespace, null);

        /// <summary>
        /// Execute the code modifier on the given namespace with the given code generator
        /// </summary>
        /// <param name="codeNamespace"></param>
        /// <param name="codeGenerator"></param>
        public virtual void Execute(CodeNamespace codeNamespace, ICodeGenerator codeGenerator)
        {
            CodeGenerator = codeGenerator;
            Execute(codeNamespace);
        }

    }
}
