﻿#if COMMUNITY_API
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
#else
using EnvDTE;
#endif
using CodeGeneration.CodeModifiers;
using CodeGeneration.Generators;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;
using Configuration = BlueToque.XsdCodeGen.Library.Configuration;
using BlueToque.XsdCodeGenerator.Properties;
using BlueToque.XsdCodeGen.Library;

namespace BlueToque.XsdCodeGenerator
{
    public class XsdCodeGenerator : BaseCodeGeneratorWithSite
    {
        public override string GetDefaultExtension() => ".cs";

        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return Encoding.UTF8.GetBytes(GenerateCodeInternal(inputFileName, inputFileContent));
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                    ex = ex.InnerException;

                return Encoding.UTF8.GetBytes(string.Format(CultureInfo.CurrentCulture, Resources.CustomTool_GeneralError, ex));
            }
        }

        #region private

        /// <summary>
        /// Generates the output.
        /// </summary>
        string GenerateCodeInternal(string inputFileName, string inputFileContent)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            #region load the configuration
            Configuration.Load(inputFileName);
            StringBuilder output = new StringBuilder()
                .AppendFormat(Constants.TemplateAutogenerated, Constants.Name, Constants.Version, Environment.Version, DateTime.Now)
                .AppendFormat("\r\n//\thttp://BlueToque.ca\r\n");
            #endregion

            #region read the schema(s)
            Directory.SetCurrentDirectory(Path.GetDirectoryName(inputFileName));

            XmlSchema xsd;
            using (FileStream fs = File.OpenRead(inputFileName))
                xsd = XmlSchema.Read(fs, null);
            #endregion

            // create the class generator and set default options
            XsdClassGenerator xsdClassGenerator = CreateClassGenerator(xsd);

            // load the code modifiers and importers
            LoadCodeModifiers(Configuration.Current, xsdClassGenerator, output);
            LoadSchemaImporters(Configuration.Current, xsdClassGenerator, output);

            // generate code
            xsdClassGenerator.Compile();

            // save config file and make sure it's added to the project
            Configuration.Save(InputFilePath);

            // fire and forget?
#if COMMUNITY_API
            _ = AddToProjectAsync(Configuration.GetConfigFileName(InputFilePath));
#else
            _ = AddToProject(Configuration.GetConfigFileName(InputFilePath));
#endif
            // Workaround for known bug with fixed attributes:
            output
                .AppendLine(Resources.Message_1591)
                .AppendLine(Resources.Pragma_1591_Disable)
                .Append(xsdClassGenerator.CodeString)
                .Append(Resources.Pragma_1591_Enable);

            return output.ToString();
        }

        /// <summary>
        /// Create the class generator and set the options
        /// </summary>
        /// <param name="xsd"></param>
        /// <returns></returns>
        private XsdClassGenerator CreateClassGenerator(XmlSchema xsd)
        {
            CodeGenerationOptions options = CodeGenerationOptions.None;
            if (Configuration.Current.EnableDataBinding) options |= CodeGenerationOptions.EnableDataBinding;
            if (Configuration.Current.GenerateOrder) options |= CodeGenerationOptions.GenerateOrder;
            if (Configuration.Current.GenerateProperties) options |= CodeGenerationOptions.GenerateProperties;

            XsdClassGenerator xsdClassGenerator = new XsdClassGenerator(xsd, options)
            {
                CodeNamespaceString = FileNamespace,
                CodeGeneratorOptions = new BaseCodeGeneratorOptions()
                {
                    GenerateCodeString = true,
                    GenerateObjects = false,
                    CompileAssembly = false,
                }
            };
            return xsdClassGenerator;
        }

        /// <summary>
        /// Load the schema importers
        /// </summary>
        /// <param name="current"></param>
        /// <param name="xsdClassGenerator"></param>
        /// <param name="output"></param>
        private void LoadSchemaImporters(Configuration current, XsdClassGenerator xsdClassGenerator, StringBuilder output)
        {
            foreach (AssemblyType assembly in current.SchemaImporterExtensions)
            {
                ObjectHandle handle = Activator.CreateInstance(assembly.Assembly, assembly.Type);
                if (handle == null)
                {
                    output.AppendFormat("//\tWarning, could not create SchemaImporterExtensions type {0} from assembly {1}", assembly.Type, assembly.Assembly);
                    continue;
                }

                if (!(handle.Unwrap() is SchemaImporterExtension extension))
                {
                    output.AppendFormat("//\tWarning SchemaImporterExtensions {0} from assembly {1} does not derive from SchemaImporterExtension", assembly.Type, assembly.Assembly);
                    continue;
                }

                xsdClassGenerator.SchemaImporterExtensions.Add(extension);
            }
        }

        /// <summary>
        /// Load the code modifiers
        /// </summary>
        /// <param name="current"></param>
        /// <param name="xsdClassGenerator"></param>
        /// <param name="output"></param>
        private void LoadCodeModifiers(Configuration current, XsdClassGenerator xsdClassGenerator, StringBuilder output)
        {
            foreach (AssemblyType assembly in current.CodeModifiers)
            {
                ObjectHandle handle = Activator.CreateInstance(assembly.Assembly, assembly.Type);
                if (handle == null)
                {
                    output.AppendFormat("//\tWarning, could not create CodeModifier type {0} from assembly {1}", assembly.Type, assembly.Assembly);
                    continue;
                }

                if (!(handle.Unwrap() is ICodeModifier modifier))
                {
                    output.AppendFormat("//\tWarning CodeModifier {0} from assembly {1} does not derive from ICodeModifier", assembly.Type, assembly.Assembly);
                    continue;
                }

                modifier.XmlOptions = assembly.Any;
                xsdClassGenerator.CodeModifiers.Replace(modifier);
            }
        }

        /// <summary>
        /// Add a file to the project dependent upon the file we're currently generating code for.
        /// </summary>
        /// <param name="tempFileName"></param>
#if COMMUNITY_API
        internal async Task<bool> AddToProjectAsync(string fileName)
        {
            try
            {
                //ThreadHelper.ThrowIfNotOnUIThread();
                Project project = (await PhysicalFile.FromFileAsync(InputFilePath)).ContainingProject;
                project?.AddExistingFilesAsync(fileName);
                return true;
            }
            catch(Exception ex)
            {
                Trace.TraceError("Error adding file to project\r\n{0}", ex);
                return false;
            }
        }
#else
        internal bool AddToProject(string fileName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!(base.GetService(typeof(ProjectItem)) is ProjectItem item))
                return false;

            Project project = item?.ContainingProject;
            if (project == null)
                return false;

            // check to make sure the item is not already in the project
            string fileNameOnly = Path.GetFileName(fileName);

            foreach (ProjectItem projItem in item.ProjectItems)
            {
                if (projItem.Name == fileNameOnly)
                    return false;
            }

            // check to make sure the file exists
            if (!File.Exists(fileName))
                return false;

            try
            {
                // add as “DependentUpon”
                item.ProjectItems.AddFromFile(fileName);

                // expand the files below the selected item
                item.ExpandView();

                // save the project
                project.Save(project.FullName);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error adding file \'{0}\' to project \'{1}\':\r\n{2}", fileName, project.FullName, ex);
                return false;
            }
        }
#endif
        #endregion
    }
}