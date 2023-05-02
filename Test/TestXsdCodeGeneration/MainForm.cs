using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestXsdCodeGeneration
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void myClassToXsdButton_Click(object sender, EventArgs e)
        {
            ClassToXsdForm form = new ClassToXsdForm();
            form.ShowDialog(this);
        }

        private void myXsdToClassesButton_Click(object sender, EventArgs e)
        {
            XsdToClassForm form = new XsdToClassForm();
            form.ShowDialog(this);
        }
    }
}