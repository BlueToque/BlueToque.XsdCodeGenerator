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

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// Remove the DebuggerStepThroughAttribute from classes
    /// This attribute stops the debugger from stepping through classes
    /// that are so decorated. By removing it we can let the debugger
    /// step through the code.
    /// </summary>
    public class RemoveDebuggerAttribute : BaseCodeModifier
    {
        public RemoveDebuggerAttribute() : base("Remove Debugger Attribute") { }

        #region ICodeModifier Members

        public override void Execute(CodeNamespace codeNamespace)
        {
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (!type.IsClass) continue;

                CodeAttributeDeclaration declToRemove = null;
                foreach (CodeAttributeDeclaration decl in type.CustomAttributes)
                {
                    if (decl.Name == "System.Diagnostics.DebuggerStepThroughAttribute")
                    {
                        declToRemove = decl;
                        break;
                    }
                }

                if (declToRemove != null)
                    type.CustomAttributes.Remove(declToRemove);

            }
        }

        #endregion
    }
}
