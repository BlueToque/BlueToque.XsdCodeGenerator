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
    /// The purpose of this code modifier is to solve the problem when an XML Schema
    /// imports another XML Schema that has already had code generated for it.
    /// The default behaviour is for the XsdToClasses tool to generate all of the code
    /// for all of the complex types in the base schema and all of the imported schemas.
    /// This is not desired when an external library has already generated to require code.
    /// 
    /// The solution is as follows:
    /// This code modifier takes as it's options a file that looks like follows:
    /// 
    ///   <ImportFixerOptions xmlns="http://BlueToque.ca/CodeGeneration/CodeModifiers/ImportFixerOptions.Options">
    ///     <Namespace XmlNamespace="http://BlueToque.ca/TrueNorth/Geographic" CodeNameSpace="TrueNorth.Geographic" />
    ///   </ImportFixerOptions>
    ///   
    /// This sets up a correspondance between the xml namespace "http://BlueToque.ca/TrueNorth/Geographic" 
    /// and the code namespace "TrueNorth.Geographic"
    /// 
    /// WHat the codemodifier does with this is it will remove all types from the generated code that come from the 
    /// Xml namespace, and include the a using statement for the Code namespace to the library where the types have already been defined.
    /// It is up to the programmer to be sure the proper library is included in the references of the project.
    /// 
    /// These classes are identified by the following attribute: 
    /// [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/TrueNorth/Geographic")]
    /// </summary>
    public class ImportFixer : BaseCodeModifier
    {
        public ImportFixer() : base("Fix Imports") { }

        public ImportFixerOptions Options { get; set; } = new ImportFixerOptions();

        public override XmlElement XmlOptions
        {
            get => Serializer.SerializeToElement(Options);
            set { Options = Serializer.Deserialize<ImportFixerOptions>(value); }
        }

        #region ICodeModifier Members

        public override void Execute(CodeNamespace codeNamespace)
        {
            
            #region find the values that have the namespace
            CodeTypeDeclarationCollection typesToRemove = new CodeTypeDeclarationCollection();
            // foreach datatype in the codeNamespace
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                string ns = Utility.GetXmlNamespace(type);
                
                if (string.IsNullOrEmpty(ns))
                    continue;

                if (Options.Namespace.ContainsXmlName(ns))
                    typesToRemove.Add(type);
            }
            #endregion

            // remove the types you decided to remove
            codeNamespace.Types.RemoveAll(typesToRemove);

            foreach (NamespaceType ns in Options.Namespace.Where(ns=> !string.IsNullOrEmpty(ns.CodeNamespace)))
                codeNamespace.Imports.Add(new CodeNamespaceImport(ns.CodeNamespace));
        }

        #endregion

    }
}
