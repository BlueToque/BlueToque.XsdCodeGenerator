//=============================================================================
//
// Copyright (C) 2010 Michael Coyle, Blue Toque
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
using System.Collections.Generic;
using System.Xml;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// When you get the following error:
    /// Only XmlRoot attribute may be specified for the type Fusion.DataLib.HibernateConfigType. Please use XmlSchemaProviderAttribute to specify schema type
    /// Use this modifier to remove the XmlType
    /// </summary>
    public class RemoveXmlTypeAttribute : BaseCodeModifier
    {
        public RemoveXmlTypeAttribute() : base("Remove XmlType Attribute") { }

        public RemoveXmlTypeAttributeOptions Options { get; set; } = new RemoveXmlTypeAttributeOptions();

        public override XmlElement XmlOptions
        {
            get => Serializer.SerializeToElement(Options);
            set { Options = Serializer.Deserialize<RemoveXmlTypeAttributeOptions>(value); }
        }

        #region ICodeModifier Members

        public override void Execute(CodeNamespace codeNamespace)
        {
            CodeTypeDeclarationCollection typesToRemove = new CodeTypeDeclarationCollection();
            // foreach datatype in the codeNamespace
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (Options.Type.Find(x => x.Name == codeNamespace.Name + "." + type.Name) != null ||
                    Options.Type.Find(x => x.Name == type.Name) != null)
                    typesToRemove.Add(type);
            }

            foreach (CodeTypeDeclaration type in typesToRemove)
            {
                List<CodeAttributeDeclaration> toRemove = new List<CodeAttributeDeclaration>();
                foreach (CodeAttributeDeclaration decl in type.CustomAttributes)
                    if (decl.Name == "System.Xml.Serialization.XmlTypeAttribute")
                        toRemove.Add(decl);
                foreach (var x in toRemove)
                    type.CustomAttributes.Remove(x);
                //if (type.CustomAttributes.Contains(new CodeAttributeDeclaration("System.Xml.Serialization.XmlTypeAttribute")))
                //    type.CustomAttributes.Remove(new CodeAttributeDeclaration("System.Xml.Serialization.XmlTypeAttribute"));
            }
        }

        #endregion
    }
}
