//=============================================================================
//
// Copyright (C) 2007 Michael Coyle, Blue Toque
// Copyright (C) 2023 Michael Coyle, Blue Toque Software
// http://www.BlueToque.ca/Products/CodeGeneration.html
// Michael.Coyle@BlueToque.ca
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
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Generators
{
    public partial class XsdClassGeneratorOptions
    {
        public XsdClassGeneratorOptions()
        {
            CompileAssembly = false;
            GenerateObjects = false;
            GenerateComplexTypes = false;
            GenerateCodeString = false;
        }

    }
}
