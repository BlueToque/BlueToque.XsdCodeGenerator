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
using System.IO;
using System.Web.Services.Description;
using System.Xml;

namespace CodeGeneration.Generators
{
    /// <summary>
    /// Generates WSDL from a web service type directly
    /// </summary>
    class WsdlGenerator
    {
        public void Generate(Type type, string url, TextWriter writer)
        {
            ServiceDescriptionReflector reflector = new ServiceDescriptionReflector();
            reflector.Reflect(type, url);

            if (reflector.ServiceDescriptions.Count > 1)
                throw new Exception("Deal with multiple service descriptions later");

            using (XmlTextWriter wtr = new XmlTextWriter(writer))
            {
                wtr.Formatting = Formatting.Indented;
                reflector.ServiceDescriptions[0].Write(wtr);
                wtr.Close();
            }

        }
    }
}
