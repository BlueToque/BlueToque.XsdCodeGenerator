using BlueToque.XsdCodeGen.Library;
using CodeGeneration;
using CodeGeneration.Generators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace TestXsdCodeGeneration
{
    public partial class ClassToXsdForm : Form
    {
        public ClassToXsdForm() => InitializeComponent();

        void AssemblyTreeView_TypeSelected(object sender, TreeViewEventArgs e)
        {
            Type type = e.Node.Tag as Type;
            if (type == null)
                return;

            try
            {
                IList<XmlSchema> schemas = new ClassXsdGenerator().GenerateSchema(type);
                AddSchemas(schemas);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AddSchemas(IList<XmlSchema> schemas)
        {
            ClearSchemas();
            foreach (XmlSchema schema in schemas)
                AddSchema(schema);
        }

        private void AssemblyTreeView_MethodSelected(object sender, TreeViewEventArgs e)
        {
            MethodInfo methodInfo = e.Node.Tag as MethodInfo;
            if (methodInfo == null)
                return;

            try
            {
                List<ParameterInfo> parameterList = MethodCallTools.GetInputParameters(methodInfo);
                IList<XmlSchema> schemas = MethodCallTools.ConvertParametersToSchemas(parameterList);
                AddSchemas(schemas);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void LoadAssemblyButton_Click(object sender, EventArgs e)
        {
            if (myOpenAssemblyDialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                Assembly asm = Assembly.LoadFrom(myOpenAssemblyDialog.FileName);
                myAssemblyFileNameTextBox.Text = myOpenAssemblyDialog.FileName;
                myAssemblyTreeView.SetAssembly(asm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }


        }

        private void AddSchema(XmlSchema schema)
        {
            this.mySchemaTabControl.SuspendLayout();
            RichTextBox richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Text = SchemaToText(schema),
                Font = new Font("Lucida Console", 10.0f)
            };

            TabPage tabPage = new TabPage
            {
                Padding = new Padding(3),
                Text = string.IsNullOrEmpty(schema.TargetNamespace) ? "namespace" : schema.TargetNamespace,
                UseVisualStyleBackColor = true
            };
            tabPage.Controls.Add(richTextBox);

            mySchemaTabControl.TabPages.Add(tabPage);

            this.mySchemaTabControl.ResumeLayout(false);
        }

        private void ClearSchemas() => mySchemaTabControl.TabPages.Clear();

        private static string SchemaToText(XmlSchema schema)
        {
            if (schema == null) return string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xtw = new XmlTextWriter(sw))
                {
                    xtw.Formatting = Formatting.Indented;
                    schema.Write(xtw);
                }
                return sw.ToString();
            }
        }

    }
}