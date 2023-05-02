using System.CodeDom;
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

namespace CodeGeneration.CodeModifiers
{
    public class StripProxyTypesCodeModifier : BaseCodeModifier
    {
        public StripProxyTypesCodeModifier() : base("Strip Proxy Types") { }

        /// <summary>
        /// This method walks over the generated code, removing any code that isn't the proxy
        /// class itself. This is so we can (if we like) include the type definitions in another file
        /// Dont run this code modifer if you don't understand it.
        /// 
        /// You will need to add "using" statements to the code so that the types that are in 
        /// external assemblies are referened properly.
        /// 
        /// The purpose of this is so that the client and the 
        /// server code can share an assembly that contains the types.
        /// </summary>
        /// <param name="codeCompileUnit">The generated proxy code, in CodeDOM form. This 
        /// object will be modified by this method.</param>
        public override void Execute(CodeNamespace codeNamespace)
        {
            // Remove anything that isn't the proxy itself
            CodeTypeDeclarationCollection typesToRemove = new CodeTypeDeclarationCollection();

            foreach (CodeTypeDeclaration codeType in codeNamespace.Types)
            {
                bool webDerived = false;
                foreach (CodeTypeReference baseType in codeType.BaseTypes)
                {
                    if (baseType.BaseType == "System.Web.Services.Protocols.SoapHttpClientProtocol")
                    {
                        webDerived = true;
                        break;
                    }
                }

                // We can't remove elements from a collection while we're iterating over it...
                if (!webDerived)
                    typesToRemove.Add(codeType);
            }

            // ...so we remove them later
            codeNamespace.Types.RemoveAll(typesToRemove);
            //foreach (CodeTypeDeclaration codeType in typesToRemove)
            //    codeNamespace.Types.Remove(codeType);

            // Add the missing using statements. Should probably allow these to be specified
            // on the command line, but hardcode them for now
            //codeNamespace.Imports.Add(new CodeNamespaceImport("Integic.ePower.Psd.Common.Framework"));
            //codeNamespace.Imports.Add(new CodeNamespaceImport("Integic.ePower.Psd.Common.Framework.Audit"));
            //codeNamespace.Imports.Add(new CodeNamespaceImport("Integic.ePower.Psd.Common.Utilities.Ticketing"));

        }
    }
}
