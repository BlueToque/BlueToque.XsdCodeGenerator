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
using System.Xml;
using CodeGeneration.Generators;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// The ICodeModifier interface defines an interface for modifying code
    /// The XmlOptions allows this interface to read XML options from an XmlElement
    /// so that the calling class can read these options without understanding them.
    /// 
    /// The Execute Method is used to modify the code and contains a reference to the calling
    /// ICodeGenerator
    /// </summary>
    public interface ICodeModifier
    {
        string Name { get; }
        XmlElement XmlOptions { get; set; }
        void Execute(CodeNamespace codeNamespace, ICodeGenerator codeGenerator);
    }
}
