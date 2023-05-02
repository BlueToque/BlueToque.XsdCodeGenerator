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
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace CodeGeneration.Generators
{
    /// <summary>
    /// The base class implementation of a code generator
    /// contains utility classes for generating code
    /// </summary>
    public class CodeGeneratorBase : ICodeGenerator
    {

        #region ICodeGenerator Members

        /// <summary>
        /// The source document being used to generated this code
        /// </summary>
        public XmlDocument SourceDocument { get; set; } = new XmlDocument();

        /// <summary>
        /// Get and set the code generator options
        /// </summary>
        public BaseCodeGeneratorOptions CodeGeneratorOptions { get; set; } = new BaseCodeGeneratorOptions();

        /// <summary>
        /// Returns the course code as a string if the code was generated successfully
        /// </summary>
        public string CodeString { get; internal set; }

        /// <summary>
        /// Returns true if there are any errors
        /// </summary>
        public bool HasErrors => Errors.Count > 0;

        /// <summary>
        /// returns true if the resulting assembly has been compiled, false otherwise
        /// </summary>
        public bool IsCompiled => Assembly != null;

        /// <summary>
        /// A collection of error strings resulting from the compile
        /// </summary>
        public CompilerErrorCollection Errors { get; } = new CompilerErrorCollection();

        /// <summary>
        /// A list of code modifiers to apply to the code dom after compilation
        /// </summary>
        public CodeModifierCollection CodeModifiers { get; } = new CodeModifierCollection();

        /// <summary>
        /// the code dom namespace, this is the structure that represents the code
        /// </summary>
        public CodeNamespace CodeNamespace { get; private set; }

        /// <summary>
        /// A list of referenced assemblies
        /// </summary>
        public StringCollection ReferencedAssemblies => CompilerParameters.ReferencedAssemblies;

        /// <summary>
        /// A pointer to the in-memory assembly that is the result of compilation
        /// </summary>
        public Assembly Assembly { get; private set; } = null;

        /// <summary>
        /// The logging level to use for the run of this code generator (for testing)
        /// </summary>
        public static SourceLevels Logging
        {
            get { return Trace.Switch.Level; }
            set { Trace.Switch.Level = value; }
        }

        /// <summary>
        /// The code namespace to use for code generation
        /// </summary>
        public string CodeNamespaceString { get; set; }

        /// <summary>
        /// Parameters to pass to the compiler controling where the code generator puts code etc.
        /// </summary>
        public CompilerParameters CompilerParameters { get; set; } = new CompilerParameters();

        /// <summary>
        /// The code provider, or what language to generate the code in
        /// </summary>
        public string CodeProvider { get; set; } = "CSharp";

        /// <summary>
        /// Set this to TRUE and instead of returning FALSE, the Compile method
        /// will throw exceptions. Default is FALSE
        /// </summary>
        public bool ThrowExceptions { get; set; } = false;

        #endregion

        #region protected

        /// <summary>
        /// Create the code namespace
        /// </summary>
        protected void CreateCodeNamespace()
        {
            if (CodeNamespace != null)
                return;
            CodeNamespace = string.IsNullOrEmpty(CodeNamespaceString) ? new CodeNamespace() : new CodeNamespace(CodeNamespaceString);
        }

        /// <summary>
        /// Method to get an CodeDomProvider with which this class can create code.
        /// </summary>
        /// <returns></returns>
        protected virtual CodeDomProvider GetCodeWriter() => CodeDomProvider.CreateProvider(CodeProvider);

        /// <summary>
        /// Generate the code
        /// </summary>
        /// <returns></returns>
        protected bool GenerateCode()
        {
            Assembly = null;

            #region Generate the code
            if (CodeGeneratorOptions.GenerateCodeString)
            {
                //CodeCompileUnit compileUnit = new CodeCompileUnit();
                //compileUnit.Namespaces.Add(m_codeNamespace);
                //CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                //provider.GenerateCodeFromCompileUnit(compileUnit, sw, options);

                using (StringWriter sw = new StringWriter())
                {
                    var codeWriter = GetCodeWriter();
                    var compilerInfo = CodeDomProvider.GetAllCompilerInfo();

                    codeWriter.GenerateCodeFromNamespace(CodeNamespace, sw, new CodeGeneratorOptions { VerbatimOrder = true });
                    CodeString = sw.ToString();
                }
            }
            #endregion

            AddReferencedAssemblies();

            // compile the generated code
            if (CodeGeneratorOptions.CompileAssembly)
                return CompileAssembly();
 
            return true;
        }

        /// <summary>
        /// Compile the generated code
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private bool CompileAssembly()
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(CodeNamespace);

            CodeDomProvider provider = GetCodeWriter();
            CompilerResults compilerResults = provider.CompileAssemblyFromDom(CompilerParameters, new CodeCompileUnit[] { compileUnit });

            // handle the errors if there are any
            if (compilerResults.Errors.HasErrors)
            {
                Errors.AddRange(compilerResults.Errors);

                // check to see if there are fatal errors
                bool fatalErrors = false;
                foreach (CompilerError error in compilerResults.Errors)
                {
                    if (!error.IsWarning)
                        fatalErrors = true;
                }

                // trace the errors at this point
                if (Trace.Switch.ShouldTrace(TraceEventType.Error))
                    Trace.TraceError(BuildCompileErrorString(compilerResults));

                // throw exception if we've ben told to
                if (ThrowExceptions && fatalErrors)
                    throw new ApplicationException(BuildCompileErrorString(compilerResults));

                return !fatalErrors;
            }

            if (compilerResults != null)
                Assembly = compilerResults.CompiledAssembly;
            return Assembly != null;
        }

        /// <summary>
        /// Build a string from compilation errors
        /// </summary>
        /// <param name="compilerResults"></param>
        /// <returns></returns>
        protected static string BuildCompileErrorString(CompilerResults compilerResults)
        {
            StringBuilder sb = new StringBuilder().AppendLine("Error compiling assembly:");

            foreach (CompilerError error in compilerResults.Errors)
                sb.AppendFormat("{0} {1} ({2}): {3}, {4}\r\n",
                    error.IsWarning ? "Warning" : "Error",
                    error.ErrorText,
                    error.ErrorNumber,
                    error.Line,
                    error.Column);

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// this method is meant to be overridden in a derived class
        /// </summary>
        protected virtual void AddReferencedAssemblies() { }

    }
}
