namespace TestXsdCodeGeneration
{
    partial class XsdToClassForm
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
            this.mySchemaFileNameTextBox = new System.Windows.Forms.TextBox();
            this.myLoadXsdButton = new System.Windows.Forms.Button();
            this.myOpenSchemaDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.myAssemblyTreeView = new AssemblyTreeView.AssemblyTreeView();
            this.myCodeRichTextBox = new System.Windows.Forms.RichTextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mySchemaFileNameTextBox
            // 
            this.mySchemaFileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mySchemaFileNameTextBox.Location = new System.Drawing.Point(12, 12);
            this.mySchemaFileNameTextBox.Name = "mySchemaFileNameTextBox";
            this.mySchemaFileNameTextBox.Size = new System.Drawing.Size(409, 20);
            this.mySchemaFileNameTextBox.TabIndex = 5;
            // 
            // myLoadXsdButton
            // 
            this.myLoadXsdButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myLoadXsdButton.Location = new System.Drawing.Point(427, 10);
            this.myLoadXsdButton.Name = "myLoadXsdButton";
            this.myLoadXsdButton.Size = new System.Drawing.Size(75, 23);
            this.myLoadXsdButton.TabIndex = 4;
            this.myLoadXsdButton.Text = "Load";
            this.myLoadXsdButton.UseVisualStyleBackColor = true;
            this.myLoadXsdButton.Click += new System.EventHandler(this.myLoadXsdButton_Click);
            // 
            // myOpenSchemaDialog
            // 
            this.myOpenSchemaDialog.Filter = "Schemas|*.xsd";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 39);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.myAssemblyTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.myCodeRichTextBox);
            this.splitContainer1.Size = new System.Drawing.Size(490, 516);
            this.splitContainer1.SplitterDistance = 258;
            this.splitContainer1.TabIndex = 7;
            // 
            // myAssemblyTreeView
            // 
            this.myAssemblyTreeView.Assembly = null;
            this.myAssemblyTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myAssemblyTreeView.Location = new System.Drawing.Point(0, 0);
            this.myAssemblyTreeView.Name = "myAssemblyTreeView";
            this.myAssemblyTreeView.Size = new System.Drawing.Size(490, 258);
            this.myAssemblyTreeView.TabIndex = 1;
            // 
            // myCodeRichTextBox
            // 
            this.myCodeRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myCodeRichTextBox.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myCodeRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.myCodeRichTextBox.Name = "myCodeRichTextBox";
            this.myCodeRichTextBox.Size = new System.Drawing.Size(490, 254);
            this.myCodeRichTextBox.TabIndex = 0;
            this.myCodeRichTextBox.Text = "";
            // 
            // XsdToClassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 567);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mySchemaFileNameTextBox);
            this.Controls.Add(this.myLoadXsdButton);
            this.Name = "XsdToClassForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "XsdToClassForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mySchemaFileNameTextBox;
        private System.Windows.Forms.Button myLoadXsdButton;
        private System.Windows.Forms.OpenFileDialog myOpenSchemaDialog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private AssemblyTreeView.AssemblyTreeView myAssemblyTreeView;
        private System.Windows.Forms.RichTextBox myCodeRichTextBox;
    }
}