//=============================================================================
//
// Copyright (C) 2013 Michael Coyle, Blue Toque
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
using BlueToque.XmlLibrary.CodeModifiers.Schemas;
using CodeGeneration.CodeModifiers;
using CodeGeneration.Internal;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BlueToque.XmlLibrary.CodeModifiers
{
    /// <summary>
    /// Add a category property with the given value
    /// </summary>
    class PropertyGridProperties : BaseCodeModifier
    {
        public PropertyGridProperties() : base("Property Grid Properties") { }

        public PropertyGridOptions Options { get; set; } = new PropertyGridOptions();

        public override XmlElement XmlOptions
        {
            get => Serializer.SerializeToElement(Options);
            set { Options = Serializer.Deserialize<PropertyGridOptions>(value); }
        }

        #region ICodeModifier Members
        public override void Execute(CodeNamespace codeNamespace)
        {
            if (Options == null || Options.Property == null || Options.Property.Count == 0)
                return;

            // foreach datatype in the codeNamespace
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                // get a list of the propertytypes that start with the class name
                List<PropertyGridType> categoryTypes = Options.Property.FindAll(x => x.QualifiedName.StartsWith(type.Name));
                if (categoryTypes == null || categoryTypes.Count == 0)
                    continue;

                foreach (PropertyGridType categoryType in categoryTypes)
                {
                    CodeMemberProperty member = type.Members
                        .OfType<CodeMemberProperty>()
                        .FirstOrDefault(x => categoryType.QualifiedName.EndsWith(x.Name) || categoryType.QualifiedName.EndsWith("*"));
                    
                    if (member == null)
                        continue;

                    // Add the DisplayName attribute
                    if (!string.IsNullOrEmpty(categoryType.DisplayName))
                    {
                        CodeAttributeDeclaration attr = new CodeAttributeDeclaration("System.ComponentModel.DisplayName");
                        attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(categoryType.DisplayName)));
                        member.CustomAttributes.Add(attr);
                    }

                    // Add the Category attribute
                    if (!string.IsNullOrEmpty(categoryType.Category))
                    {
                        CodeAttributeDeclaration attr = new CodeAttributeDeclaration("System.ComponentModel.Category");
                        attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(categoryType.Category)));
                        member.CustomAttributes.Add(attr);
                    }

                    // Add the Description attribute
                    if (!string.IsNullOrEmpty(categoryType.Description))
                    {
                        // add the custom type editor attribute
                        CodeAttributeDeclaration attr = new CodeAttributeDeclaration("System.ComponentModel.Description");
                        attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(categoryType.Description)));
                        member.CustomAttributes.Add(attr);
                    }

                    // Add the Editor attribute
                    if (!string.IsNullOrEmpty(categoryType.Editor))
                    {
                        string[] p = categoryType.Editor.Split(',');
                        if (p.Length == 2)
                        {
                            CodeAttributeDeclaration attr = new CodeAttributeDeclaration("System.ComponentModel.Editor");
                            attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(p[0])));
                            attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(p[1])));
                            member.CustomAttributes.Add(attr);
                        }
                    }


                    // Add the Browsable attribute
                    if (categoryType.BrowsableSpecified && !categoryType.Browsable)
                    {
                        CodeAttributeDeclaration attr = new CodeAttributeDeclaration("System.ComponentModel.Browsable");
                        attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(false)));
                        member.CustomAttributes.Add(attr);
                    }

                    // Add the ReadOnly attribute
                    if (categoryType.ReadOnlySpecified && categoryType.ReadOnly)
                    {
                        CodeAttributeDeclaration attr = new CodeAttributeDeclaration("System.ComponentModel.ReadOnly");
                        attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(true)));
                        member.CustomAttributes.Add(attr);
                    }
                }

            }
        }

        #endregion

    }
}
