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
    /// Add XML comments to every class and method
    /// </summary>
    class AddDocComments : BaseCodeModifier
    {
        public AddDocComments() : base("Add Document Comments") { }

        /// <summary>
        /// Loop over all members that don't have a triple-slash comment and 
        /// add one. We do this primarily so that the generated code won't barf
        /// with a "missing XML comment" when we turn the warning level up to 4
        /// and set "warn as error" to true. 
        /// </summary>
        /// <param name="codeNamespace">The CodeDOM object that represents the
        /// generated proxy code</param>
        public override void Execute(CodeNamespace codeNamespace)
        {
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                foreach (CodeTypeMember member in type.Members)
                {
                    if (member.Comments.Count == 0)
                        member.Comments.Add(new CodeCommentStatement("<summary />", true));
                }
            }
        }
    }
}
