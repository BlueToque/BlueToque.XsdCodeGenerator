using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Schema;
using System.Xml;
using CodeGeneration;
using System.Diagnostics;

namespace TestXsdCodeGeneration
{
    public partial class XsdToClassForm : Form
    {
        public XsdToClassForm()
        {
            InitializeComponent();
        }

        XmlSchema m_schema;
        private void myLoadXsdButton_Click(object sender, EventArgs e)
        {
            if (myOpenSchemaDialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using (XmlReader reader = XmlReader.Create(myOpenSchemaDialog.FileName))
                {
                    m_schema = XmlSchema.Read(reader, new ValidationEventHandler(myValidationEventHandler));
                    XmlSchemaSet set = new XmlSchemaSet();
                    set.ValidationEventHandler += new ValidationEventHandler(myValidationEventHandler);
                    set.Add(m_schema);
                    set.Compile();
                }
                mySchemaFileNameTextBox.Text = myOpenSchemaDialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            try
            {
                XsdClassGenerator generator = new XsdClassGenerator(m_schema);
                generator.SchemaImporterExtension.Add(new CodeGeneration.ImporterExtensions.SimpleTypeExtension());
                generator.Compile();
                myAssemblyTreeView.Assembly = generator.Assembly;
                myCodeRichTextBox.Text = generator.CodeString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        void myValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
                throw e.Exception;
            else
                Trace.WriteLine(e.Exception.ToString());
        }

    }
}