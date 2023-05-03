using CodeGeneration.Generators;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace TestXsdCodeGeneration
{
    public partial class XsdToClassForm : Form
    {
        public XsdToClassForm() => InitializeComponent();

        XmlSchema m_schema;
        private void LoadXsdButton_Click(object sender, EventArgs e)
        {
            if (myOpenSchemaDialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using (XmlReader reader = XmlReader.Create(myOpenSchemaDialog.FileName))
                {
                    m_schema = XmlSchema.Read(reader, new ValidationEventHandler(ValidationEventHandler));
                    XmlSchemaSet set = new XmlSchemaSet();
                    set.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
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
                generator.SchemaImporterExtensions.Add(new CodeGeneration.ImporterExtensions.SimpleTypeExtension());
                generator.Compile();
                myAssemblyTreeView.SetAssembly(generator.Assembly);
                myCodeRichTextBox.Text = generator.CodeString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
                throw e.Exception;
            else
                Trace.WriteLine(e.Exception.ToString());
        }

    }
}