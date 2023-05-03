namespace TestXsdCodeGeneration
{
    partial class MainForm
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
            this.myXsdToClassesButton = new System.Windows.Forms.Button();
            this.myClassToXsdButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // myXsdToClassesButton
            // 
            this.myXsdToClassesButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.myXsdToClassesButton.Location = new System.Drawing.Point(12, 12);
            this.myXsdToClassesButton.Name = "myXsdToClassesButton";
            this.myXsdToClassesButton.Size = new System.Drawing.Size(220, 23);
            this.myXsdToClassesButton.TabIndex = 0;
            this.myXsdToClassesButton.Text = "Xsd To Classes";
            this.myXsdToClassesButton.UseVisualStyleBackColor = true;
            this.myXsdToClassesButton.Click += new System.EventHandler(this.XsdToClassesButton_Click);
            // 
            // myClassToXsdButton
            // 
            this.myClassToXsdButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.myClassToXsdButton.Location = new System.Drawing.Point(12, 41);
            this.myClassToXsdButton.Name = "myClassToXsdButton";
            this.myClassToXsdButton.Size = new System.Drawing.Size(220, 23);
            this.myClassToXsdButton.TabIndex = 1;
            this.myClassToXsdButton.Text = "Class To Xsd";
            this.myClassToXsdButton.UseVisualStyleBackColor = true;
            this.myClassToXsdButton.Click += new System.EventHandler(this.ClassToXsdButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 76);
            this.Controls.Add(this.myClassToXsdButton);
            this.Controls.Add(this.myXsdToClassesButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Test Code Generation";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button myXsdToClassesButton;
        private System.Windows.Forms.Button myClassToXsdButton;
    }
}

