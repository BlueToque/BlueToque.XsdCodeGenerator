using System;
using System.Windows.Forms;

namespace TestXsdCodeGeneration
{
    public partial class MainForm : Form
    {
        public MainForm() => InitializeComponent();

        private void ClassToXsdButton_Click(object sender, EventArgs e) => new ClassToXsdForm().ShowDialog(this);

        private void XsdToClassesButton_Click(object sender, EventArgs e) => new XsdToClassForm().ShowDialog(this);
    }
}