//=============================================================================
//
// Copyright (C) 2007 Michael Coyle, Blue Toque
// Copyright (C) 2023 Michael Coyle, Blue Toque Software
// http://www.BlueToque.ca/Products/XsdToClasses.html
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
using BlueToque.XsdCodeGen.Library.Properties;
using CodeGeneration.CodeModifiers;
using CodeGeneration.Internal;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace BlueToque.XsdCodeGen.Library
{
    /// <summary>
    /// This class contains configuration settings for XsdToClasses
    /// </summary>
    partial class Configuration
    {
        private static Configuration s_configuration;

        /// <summary> Default constructor </summary>
        public Configuration()
        {
            CodeModifiers = new AssemblyTypeCollection();
            SchemaImporterExtensions = new AssemblyTypeCollection();
            EnableDataBinding = true;
            GenerateComplexTypes = true;
            GenerateComplexTypesSpecified = true;
            GenerateDebuggableCode = true;
            GenerateProperties = true;
            GenerateSoapTypes = false;
        }

        private static Configuration CreateDefaultConfiguration()
        {
            Configuration config = new Configuration
            {
                EnableDataBinding = true,
                GenerateOrder = true,
                GenerateProperties = true,
                GenerateComplexTypes = true,
                GenerateComplexTypesSpecified = true
            };

            config.CodeModifiers
                .Add(new ConvertArraysToCollections().ToAssemblyType())
                .Add(new RemoveDebuggerAttribute().ToAssemblyType())
                .Add(new ImportFixer().ToAssemblyType())
                .Add(new RemoveSpecifiedTypes().ToAssemblyType())
                .Add(new ModifyImports().ToAssemblyType());

            return config;
        }

        /// <summary>
        /// Get the filename to save the configuration to.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetConfigFileName(string fileName) => Path.ChangeExtension(fileName, Resources.GeneratedCodeExtension + ".xml");

        #region load and save
        /// <summary>
        /// Load the configuration from the output file if it exists
        /// Otherwise, create a new configuration
        /// </summary>
        /// <param name="fileName"></param>
        public static void Load(string fileName)
        {
            string configFile = (!fileName.EndsWith(".xml")) ? GetConfigFileName(fileName) : fileName;
            try
            {
#if REPLACEORIGINAL
            if (!File.Exists(configFile))
                m_configuration = new Configuration();

            StringBuilder sb = new StringBuilder();


            // parse the input file and look for the configuration
            using (StreamReader sr = new StreamReader(configFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(Resources.Configuration_StartDelimeter))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith(Resources.Configuration_EndDelimeter))
                                break;
                            sb.Append(line);
                        }

                        break;
                    }
                }

            }

            // if we found the configuration, deserialize it
            if (sb.Length > 0)
            {
                try
                {
                    m_configuration = Serializer.Deserialize<Configuration>(sb.ToString());
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    m_configuration = new Configuration();
                }
                
            }
            else
            {
                m_configuration = new Configuration();
            }
#else
                s_configuration = File.Exists(configFile) ? Serializer.DeserializeFromFile<Configuration>(configFile) : CreateDefaultConfiguration();
#endif
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading configuration file {configFile}. See Inner Exception below for details", ex);
            }
        }

        /// <summary>
        /// Save the output to the given file
        /// If the file already exists, don't overwrite it
        /// </summary>
        /// <param name="fileName"></param>
        public static void Save(string fileName)
        {
            string configFile = (!fileName.EndsWith(".xml")) ? GetConfigFileName(fileName) : fileName;
            try
            {
#if REPLACEORIGINAL
            StringBuilder original = new StringBuilder();
            
            string str;
            // read the original file
            using (StreamReader sr = new StreamReader(configFile))
                str = sr.ReadToEnd();

            original.Append(str);
            // find the delimeters and delete the old configuration
            int start = str.IndexOf(Resources.Configuration_StartDelimeter);
            if (start >= 0)
            {
                int end = str.LastIndexOf(Resources.Configuration_EndDelimeter) + Resources.Configuration_EndDelimeter.Length;
                original.Remove(start, end - start);
            }
            else
            {
                start = str.IndexOf("<xs:schema");
            }

            // insert the new configuration
            string config = string.Format(
                Resources.Configuration_FormatString,
                Resources.Configuration_StartDelimeter,
                Serializer.Serialize<Configuration>(m_configuration),
                Resources.Configuration_EndDelimeter);
            original.Insert(start, config);

            // write it out
            using (StreamWriter sw = new StreamWriter(configFile, false))
                sw.Write(original.ToString());
#else
                if (!File.Exists(configFile))
                    Serializer.SerializeToFile(s_configuration, configFile);
#endif
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving configuration file {configFile}. See Inner Exception below for details", ex);
            }
        }

        /// <summary>
        /// Save the configuration to a string
        /// </summary>
        /// <returns></returns>
        public static string Save() => Serializer.Serialize(s_configuration);

        #endregion

        /// <summary> Static accessor for the current configuration </summary>
        public static Configuration Current => s_configuration;

        #region properties

        /// <summary> </summary>
        [XmlIgnore]
        public bool GenerateDebuggableCode
        {
            get => CodeModifiers.Contains(typeof(RemoveDebuggerAttribute));
            set
            {
                if ((GenerateDebuggableCode == value)) return;
                if (value)
                    CodeModifiers.Add(typeof(RemoveDebuggerAttribute));
                else
                    CodeModifiers.Remove(typeof(RemoveDebuggerAttribute));
                RaisePropertyChanged("GenerateDebuggableCode");
            }
        }

        /// <summary> </summary>
        [XmlIgnore]
        public bool GenerateSoapTypes
        {
            get => CodeModifiers.Contains(typeof(CodeGeneration.ImporterExtensions.SoapTypeExtension));
            set
            {
                if (GenerateSoapTypes == value) return;
                if (value)
                    CodeModifiers.Add(typeof(CodeGeneration.ImporterExtensions.SoapTypeExtension));
                else
                    CodeModifiers.Remove(typeof(CodeGeneration.ImporterExtensions.SoapTypeExtension));
                RaisePropertyChanged("GenerateSoapTypes");
            }
        }
 
        #endregion

    }

    [DebuggerDisplay("Name ={Type}")]
    public partial class AssemblyType
    {
        public AssemblyType() { }

        public AssemblyType(Type type)
        {
            Type = type.FullName;
            Assembly = type.Assembly.FullName;
            ICodeModifier modifier = Activator.CreateInstance(type) as ICodeModifier;
            Any = modifier?.XmlOptions;
        }

    }

    [ClassInterface(ClassInterfaceType.None)]
    public partial class AssemblyTypeCollection
    {
        /// <summary>
        /// Returns true if this collection contains a reference to this type
        /// </summary>
        /// <param name="type"></param>
        public bool Contains(Type type) => this.Any(x => x.Type == type.FullName && x.Assembly == type.Assembly.FullName);

        public new bool Contains(AssemblyType type) => this.Any(x => x.Type == type.Type && x.Assembly == type.Assembly);

        /// <summary>
        /// Add an AssemblyType if it's not already added
        /// </summary>
        /// <param name="type"></param>
        public new AssemblyTypeCollection Add(AssemblyType type)
        {
            if (!Contains(type))
                base.Add(type);
            return this;
        }

        /// <summary>
        /// Add a AssemblyType if it's not already added
        /// </summary>
        /// <param name="type"></param>
        public AssemblyTypeCollection Add(Type type)
        {
            if (!Contains(type))
                base.Add(new AssemblyType(type));
            return this;
        }

        /// <summary>
        /// Remove a CodeModifierType
        /// </summary>
        /// <param name="type"></param>
        public void Remove(Type type)
        {
            var toRemove = this.FirstOrDefault(x => x.Type == type.FullName && x.Assembly == type.Assembly.FullName);
            if (toRemove != null)
                base.Remove(toRemove);
        }
    }
}
