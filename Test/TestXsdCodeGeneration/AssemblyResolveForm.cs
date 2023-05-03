using System;
using System.Windows.Forms;

namespace TestXsdCodeGeneration
{
    public partial class AssemblyResolveForm : Form
    {
        public AssemblyResolveForm() => InitializeComponent();

        public string AssemblyName
        {
            get { return myAssemblyNameLabel.Text; }
            set { myAssemblyNameLabel.Text = value; }
        }

        public string Path
        {
            get { return myAssemblyFileNameTextBox.Text; }
            set { myAssemblyFileNameTextBox.Text = value; }
        }
        
        private void AssemblyResolveForm_Load(object sender, EventArgs e)
        {

        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (myOpenAssemblyFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            myAssemblyFileNameTextBox.Text = myOpenAssemblyFileDialog.FileName;
        }
    }
}