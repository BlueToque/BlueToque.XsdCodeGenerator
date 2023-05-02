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
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace CodeGeneration
{
    /// <summary>
    /// This interface shows us how to describe a method in such a way as
    /// to be able to 
    /// 1) describe is completely by XML Schemas
    /// 2) call it using XML Documents.
    /// 
    /// This is like SOAP lite.
    /// </summary>
    public interface IMethodCall
    {
        XmlDocument Execute(XmlDocument document);

        #region input

        /// <summary>
        /// The input schema for this Method
        /// </summary>
        XmlSchema InputSchema { get; }

        /// <summary>
        /// A list of schemas that includes the root input schema and all imported schemas
        /// </summary>
        IList<XmlSchema> InputSchemas { get; }
        
        /// <summary>
        /// The input type of this Method
        /// If this method only takes one parameter, then this is that type
        /// If this methods takes more than one parameter then this type is
        /// a wrapped parent type that contains all of the types that this 
        /// method takes as parameters.
        /// </summary>
        Type InputType { get; }

        #endregion

        /// <summary>
        /// The MethodInfo structure that describes this method
        /// </summary>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// The instance object (the class) that contains this method
        /// </summary>
        object Object { get; }

        #region output

        /// <summary>
        /// The output schema of this method
        /// </summary>
        XmlSchema OutputSchema { get; }

        /// <summary>
        /// A list of schemas that includes the root input schema and all imported schemas
        /// </summary>
        IList<XmlSchema> OutputSchemas { get; }

        /// <summary>
        /// The output type of this Method
        /// If this method only returns one parameter, then this is that type
        /// If this methods returns more than one parameter (for instance it 
        /// has "out" or "ref" parameters) then this type is a wrapped parent 
        /// type that contains all of the types that this method takes as 
        /// parameters.
        /// </summary>
        Type OutputType { get; }

        #endregion

        /// <summary>
        /// This is the return object from the method call
        /// </summary>
        object ReturnObject { get; }
    }
}
