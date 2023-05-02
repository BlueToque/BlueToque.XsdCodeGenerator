﻿using CodeGeneration.CodeModifiers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace BlueToque.XsdCodeGen.Library
{
    internal static class Extensions
    {
        public static AssemblyType ToAssemblyType(this ICodeModifier codeModifier)
        {
            return new AssemblyType(codeModifier.GetType()) { Any = codeModifier.XmlOptions };
        }
    }

    static class Constants
    {
        /// <summary>
        /// {0}=Custom Tool Name
        /// {1}=Tool version
        /// {2}=.NET Runtime version
        /// {3}=current datetime
        /// </summary>
        public const string TemplateAutogenerated =
@"//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by the {0} tool.
//     Tool Version:    {1}
//     Runtime Version: {2}
//     Generated on:    {3}
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated. For more information see http://BlueToque.ca
// </autogenerated>
//------------------------------------------------------------------------------
";

        public static string Name => Assembly.GetExecutingAssembly().FullName;

        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

}