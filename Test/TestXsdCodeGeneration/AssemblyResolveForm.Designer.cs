namespace TestXsdCodeGeneration
{
    partial class AssemblyResolveForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.myAssemblyFileNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.myBrowseButton = new System.Windows.Forms.Button();
            this.myAssemblyNameLabel = new System.Windows.Forms.Label();
            this.myOKButton = new System.Windows.Forms.Button();
            this.myCancelButton = new System.Windows.Forms.Button();
            this.myOpenAssemblyFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // myAssemblyFileNameTextBox
            // 
            this.myAssemblyFileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.myAssemblyFileNameTextBox.Location = new System.Drawing.Point(97, 28);
            this.myAssemblyFileNameTextBox.Name = "myAssemblyFileNameTextBox";
            this.myAssemblyFileNameTextBox.Size = new System.Drawing.Size(457, 20);
            this.myAssemblyFileNameTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Assembly Path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Looking for assembly:";
            // 
            // myBrowseButton
            // 
            this.myBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myBrowseButton.Location = new System.Drawing.Point(560, 26);
            this.myBrowseButton.Name = "myBrowseButton";
            this.myBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.myBrowseButton.TabIndex = 3;
            this.myBrowseButton.Text = "...";
            this.myBrowseButton.UseVisualStyleBackColor = true;
            this.myBrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // myAssemblyNameLabel
            // 
            this.myAssemblyNameLabel.AutoSize = true;
            this.myAssemblyNameLabel.Location = new System.Drawing.Point(127, 9);
            this.myAssemblyNameLabel.Name = "myAssemblyNameLabel";
            this.myAssemblyNameLabel.Size = new System.Drawing.Size(91, 13);
            this.myAssemblyNameLabel.TabIndex = 4;
            this.myAssemblyNameLabel.Text = "<AssemblyName>";
            // 
            // myOKButton
            // 
            this.myOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.myOKButton.Location = new System.Drawing.Point(479, 55);
            this.myOKButton.Name = "myOKButton";
            this.myOKButton.Size = new System.Drawing.Size(75, 23);
            this.myOKButton.TabIndex = 5;
            this.myOKButton.Text = "OK";
            this.myOKButton.UseVisualStyleBackColor = true;
            // 
            // myCancelButton
            // 
            this.myCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myCancelButton.Location = new System.Drawing.Point(560, 55);
            this.myCancelButton.Name = "myCancelButton";
            this.myCancelButton.Size = new System.Drawing.Size(75, 23);
            this.myCancelButton.TabIndex = 6;
            this.myCancelButton.Text = "Cancel";
            this.myCancelButton.UseVisualStyleBackColor = true;
            // 
            // myOpenAssemblyFileDialog
            // 
            this.myOpenAssemblyFileDialog.Filter = "Assemblies|*.exe;*.dll";
            // 
            // AssemblyResolveForm
            // 
            this.AcceptButton = this.myOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.myCancelButton;
            this.ClientSize = new System.Drawing.Size(647, 86);
            this.Controls.Add(this.myCancelButton);
            this.Controls.Add(this.myOKButton);
            this.Controls.Add(this.myAssemblyNameLabel);
            this.Controls.Add(this.myBrowseButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.myAssemblyFileNameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AssemblyResolveForm";
            this.Text = "Load Missing Assembly:";
            this.Load += new System.EventHandler(this.AssemblyResolveForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox myAssemblyFileNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button myBrowseButton;
        private System.Windows.Forms.Label myAssemblyNameLabel;
        private System.Windows.Forms.Button myOKButton;
        private System.Windows.Forms.Button myCancelButton;
        private System.Windows.Forms.OpenFileDialog myOpenAssemblyFileDialog;
    }
}