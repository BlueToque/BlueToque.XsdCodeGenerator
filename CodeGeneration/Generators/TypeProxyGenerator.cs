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
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Web.Services.Description;

namespace CodeGeneration.Generators
{
    /// <summary>
    /// This generates a Web Service Proxy directly from a type that contains a web service
    /// </summary>
    public class TypeProxyGenerator : CodeGeneratorBase
    {
        public TypeProxyGenerator(Type type, string uri, string path)
        {
            m_type = type;
            m_uri = uri;
        }

        readonly Type m_type;
        readonly string m_uri;
        private readonly ServiceDescriptionImportStyle m_style = ServiceDescriptionImportStyle.Client;

        /// <summary>
        /// This is the workhorse method of the program. Given a web service type, 
        /// it generates a proxy class for it, strips out any excess types, and then 
        /// adds a few using statments to it. 
        /// </summary>
        /// <param name="type">The web service type</param>
        /// <param name="uri">The URL for the service that will be set in the constructor</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private bool Compile()
        {
            try
            {
                // These next two lines do the generate the WSDL based on the web service class
                ServiceDescriptionReflector reflector = new ServiceDescriptionReflector();
                reflector.Reflect(m_type, m_uri);

                if (reflector.ServiceDescriptions.Count > 1)
                    throw new Exception($"Don't know how to deal with multiple service descriptions in {m_type}");

                // Now we take the WSDL service description and turn it into a proxy in CodeDOM form
                ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
                importer.AddServiceDescription(reflector.ServiceDescriptions[0], null, null);
                importer.Style = m_style;

                // Probably a good idea to make the namespace a command-line parameter, but hardcode it for now
                //CodeNamespace codeNamespace = new CodeNamespace("Integic.ePower.Psd.WebServices.Common.Proxies");
                CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
                codeCompileUnit.Namespaces.Add(CodeNamespace);

                ServiceDescriptionImportWarnings warnings = importer.Import(CodeNamespace, codeCompileUnit);

                // TODO: explicitly handle all of the warnings generated by the ServiceImporter
                if (warnings != 0)
                {
                    Trace.TraceError("Warnings: {0}", warnings);

                    switch (warnings)
                    {
                        case ServiceDescriptionImportWarnings.NoMethodsGenerated:
                            throw new ApplicationException($"Error: Web service at {m_type.FullName} does not contain any web methods");
                        case ServiceDescriptionImportWarnings.NoCodeGenerated:
                            throw new ApplicationException($"Error: No code was generated for web service at {m_type.FullName}");
                        default:
                            throw new ApplicationException($"Error: Unhandled error while generating code for web service {m_type.FullName} : {warnings}");
                    }
                }

                if (!GenerateCode())
                    return false;

            }
            catch (Exception ex)
            {
                Errors.Add(new CompilerError(string.Empty, 0, 0, string.Empty, ex.Message));
                Trace.TraceError(ex.ToString());
                return false;
            }

            return true;

        }
    }
}
