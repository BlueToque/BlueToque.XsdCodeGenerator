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
using CodeGeneration.CodeModifiers;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;

namespace CodeGeneration.Generators
{
    /// <summary>
    /// ICodegenerator interface defines an object that generates code
    /// </summary>
    public interface ICodeGenerator
    {
        /// <summary>
        /// Compiled assembly
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// A list of code modifiers that are applied to the DOM after it is generated
        /// </summary>
        CodeModifierCollection CodeModifiers { get; }
        
        /// <summary>
        /// The code namespace, used to generate the code
        /// </summary>
        CodeNamespace CodeNamespace { get; }
        
        /// <summary>
        /// String representing the code namespace
        /// </summary>
        string CodeNamespaceString { get; set; }
        
        /// <summary>
        /// Code provider, default "CSharp"
        /// </summary>
        string CodeProvider { get; set; }

        /// <summary>
        /// The denerated code string
        /// </summary>
        string CodeString { get; }
        
        /// <summary>
        /// Parameters when you need to compile the generated code
        /// </summary>
        CompilerParameters CompilerParameters { get; set; }

        /// <summary>
        /// Code generation options for this class
        /// </summary>
        BaseCodeGeneratorOptions CodeGeneratorOptions { get; set; }

        /// <summary>
        /// Compiler errors
        /// </summary>
        CompilerErrorCollection Errors { get; }

        /// <summary>
        /// A list of referenced assemblies
        /// </summary>
        StringCollection ReferencedAssemblies { get; }

        /// <summary>
        /// Does the class have Compiler Errors
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Has the code been compiled
        /// </summary>
        bool IsCompiled { get; }

        /// <summary>
        /// Should this class throw exceptions when it is generating or compiling code
        /// </summary>
        bool ThrowExceptions { get; set; }

        /// <summary>
        /// Source document - the Schema loaded into an XmlDocument
        /// </summary>
        XmlDocument SourceDocument { get; set; }
    }
}
