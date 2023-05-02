using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using CodeGeneration;
using System.Xml.Schema;
using System.IO;
using System.Xml;

namespace TestXsdCodeGeneration
{
    public partial class ClassToXsdForm : Form
    {
        public ClassToXsdForm()
        {
            InitializeComponent();
        }

        void myAssemblyTreeView_TypeSelected(object sender, Type type)
        {
            if (type == null)
                return;

            try
            {
                ClassXsdGenerator generator = new ClassXsdGenerator();
                IList<XmlSchema> schemas = generator.GenerateSchema(type);
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
            {
                AddSchema(schema);
            }
        }

        private void myAssemblyTreeView_MethodSelected(object sender, MethodInfo methodInfo)
        {
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

        private void myLoadAssemblyButton_Click(object sender, EventArgs e)
        {
            if (myOpenAssemblyDialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                Assembly asm = Assembly.LoadFrom(myOpenAssemblyDialog.FileName);
                myAssemblyFileNameTextBox.Text = myOpenAssemblyDialog.FileName;
                myAssemblyTreeView.Assembly = asm;
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
            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBox.Text = SchemaToText(schema);
            richTextBox.Font = new Font("Lucida Console", 10.0f);

            TabPage tabPage = new TabPage();
            tabPage.Controls.Add(richTextBox);
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Text = string.IsNullOrEmpty(schema.TargetNamespace) ? "namespace" : schema.TargetNamespace;
            tabPage.UseVisualStyleBackColor = true;

            mySchemaTabControl.TabPages.Add(tabPage);

            this.mySchemaTabControl.ResumeLayout(false);
        }

        private void ClearSchemas()
        {
            mySchemaTabControl.TabPages.Clear();
        }

        private static string SchemaToText(XmlSchema schema)
        {
            if (schema == null) return string.Empty;
            StringWriter sw = new StringWriter();
            using (XmlTextWriter xtw = new XmlTextWriter(sw))
            {
                xtw.Formatting = Formatting.Indented;
                schema.Write(xtw);
            }
            return sw.ToString();
        }

    }
}