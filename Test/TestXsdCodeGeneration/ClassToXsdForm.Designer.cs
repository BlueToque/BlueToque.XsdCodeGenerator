namespace TestXsdCodeGeneration
{
    partial class ClassToXsdForm
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
            this.myLoadAssemblyButton = new System.Windows.Forms.Button();
            this.myAssemblyFileNameTextBox = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mySchemaTabControl = new System.Windows.Forms.TabControl();
            this.myOpenAssemblyDialog = new System.Windows.Forms.OpenFileDialog();
            this.myAssemblyTreeView = new AssemblyTreeView.AssemblyTreeView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // myLoadAssemblyButton
            // 
            this.myLoadAssemblyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myLoadAssemblyButton.Location = new System.Drawing.Point(472, 12);
            this.myLoadAssemblyButton.Name = "myLoadAssemblyButton";
            this.myLoadAssemblyButton.Size = new System.Drawing.Size(75, 23);
            this.myLoadAssemblyButton.TabIndex = 0;
            this.myLoadAssemblyButton.Text = "Load";
            this.myLoadAssemblyButton.UseVisualStyleBackColor = true;
            this.myLoadAssemblyButton.Click += new System.EventHandler(this.myLoadAssemblyButton_Click);
            // 
            // myAssemblyFileNameTextBox
            // 
            this.myAssemblyFileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.myAssemblyFileNameTextBox.Location = new System.Drawing.Point(12, 12);
            this.myAssemblyFileNameTextBox.Name = "myAssemblyFileNameTextBox";
            this.myAssemblyFileNameTextBox.Size = new System.Drawing.Size(454, 20);
            this.myAssemblyFileNameTextBox.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 41);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.myAssemblyTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mySchemaTabControl);
            this.splitContainer1.Size = new System.Drawing.Size(535, 496);
            this.splitContainer1.SplitterDistance = 248;
            this.splitContainer1.TabIndex = 4;
            // 
            // mySchemaTabControl
            // 
            this.mySchemaTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mySchemaTabControl.Location = new System.Drawing.Point(0, 0);
            this.mySchemaTabControl.Name = "mySchemaTabControl";
            this.mySchemaTabControl.SelectedIndex = 0;
            this.mySchemaTabControl.Size = new System.Drawing.Size(535, 244);
            this.mySchemaTabControl.TabIndex = 1;
            // 
            // myOpenAssemblyDialog
            // 
            this.myOpenAssemblyDialog.Filter = "Assemblies|*.dll;*.exe";
            // 
            // myAssemblyTreeView
            // 
            this.myAssemblyTreeView.Assembly = null;
            this.myAssemblyTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myAssemblyTreeView.Location = new System.Drawing.Point(0, 0);
            this.myAssemblyTreeView.Name = "myAssemblyTreeView";
            this.myAssemblyTreeView.Size = new System.Drawing.Size(535, 248);
            this.myAssemblyTreeView.TabIndex = 1;
            this.myAssemblyTreeView.TypeSelected += new AssemblyTreeView.TypeSelectedHandler(this.myAssemblyTreeView_TypeSelected);
            this.myAssemblyTreeView.MethodSelected += new AssemblyTreeView.MethodSelectedHandler(this.myAssemblyTreeView_MethodSelected);
            // 
            // ClassToXsdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 549);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.myAssemblyFileNameTextBox);
            this.Controls.Add(this.myLoadAssemblyButton);
            this.Name = "ClassToXsdForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ClassToXsdForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button myLoadAssemblyButton;
        private AssemblyTreeView.AssemblyTreeView myAssemblyTreeView;
        private System.Windows.Forms.TextBox myAssemblyFileNameTextBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.OpenFileDialog myOpenAssemblyDialog;
        private System.Windows.Forms.TabControl mySchemaTabControl;
    }
}