namespace VideoDedup
{
    partial class Config
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
            this.BtnSelectSourcePath = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtSourcePath = new System.Windows.Forms.TextBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOkay = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.LsbFileExtensions = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnRemoveFileExtension = new System.Windows.Forms.Button();
            this.BtnAddFileExtension = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.LsbExcludedDirectories = new System.Windows.Forms.ListBox();
            this.BtnAddExcludedDirectory = new System.Windows.Forms.Button();
            this.BtnRemoveExcludedDirectory = new System.Windows.Forms.Button();
            this.TxtFileExtension = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnSelectSourcePath
            // 
            this.BtnSelectSourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSelectSourcePath.Location = new System.Drawing.Point(393, 10);
            this.BtnSelectSourcePath.Name = "BtnSelectSourcePath";
            this.BtnSelectSourcePath.Size = new System.Drawing.Size(21, 23);
            this.BtnSelectSourcePath.TabIndex = 19;
            this.BtnSelectSourcePath.Text = "...";
            this.BtnSelectSourcePath.UseVisualStyleBackColor = true;
            this.BtnSelectSourcePath.Click += new System.EventHandler(this.BtnSelectSourcePath_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Source Path:";
            // 
            // TxtSourcePath
            // 
            this.TxtSourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSourcePath.Location = new System.Drawing.Point(90, 12);
            this.TxtSourcePath.Name = "TxtSourcePath";
            this.TxtSourcePath.Size = new System.Drawing.Size(297, 20);
            this.TxtSourcePath.TabIndex = 17;
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(339, 377);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 21;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            this.BtnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOkay.Location = new System.Drawing.Point(258, 377);
            this.BtnOkay.Name = "BtnOkay";
            this.BtnOkay.Size = new System.Drawing.Size(75, 23);
            this.BtnOkay.TabIndex = 20;
            this.BtnOkay.Text = "OK";
            this.BtnOkay.UseVisualStyleBackColor = true;
            this.BtnOkay.Click += new System.EventHandler(this.BtnOkay_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 38);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TxtFileExtension);
            this.splitContainer1.Panel1.Controls.Add(this.LsbFileExtensions);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.BtnRemoveFileExtension);
            this.splitContainer1.Panel1.Controls.Add(this.BtnAddFileExtension);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.LsbExcludedDirectories);
            this.splitContainer1.Panel2.Controls.Add(this.BtnAddExcludedDirectory);
            this.splitContainer1.Panel2.Controls.Add(this.BtnRemoveExcludedDirectory);
            this.splitContainer1.Size = new System.Drawing.Size(402, 333);
            this.splitContainer1.SplitterDistance = 164;
            this.splitContainer1.TabIndex = 22;
            // 
            // LsbFileExtensions
            // 
            this.LsbFileExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LsbFileExtensions.FormattingEnabled = true;
            this.LsbFileExtensions.Location = new System.Drawing.Point(94, 29);
            this.LsbFileExtensions.Name = "LsbFileExtensions";
            this.LsbFileExtensions.Size = new System.Drawing.Size(281, 121);
            this.LsbFileExtensions.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "File Extentions:";
            // 
            // BtnRemoveFileExtension
            // 
            this.BtnRemoveFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveFileExtension.Location = new System.Drawing.Point(381, 32);
            this.BtnRemoveFileExtension.Name = "BtnRemoveFileExtension";
            this.BtnRemoveFileExtension.Size = new System.Drawing.Size(21, 23);
            this.BtnRemoveFileExtension.TabIndex = 9;
            this.BtnRemoveFileExtension.Text = "-";
            this.BtnRemoveFileExtension.UseVisualStyleBackColor = true;
            this.BtnRemoveFileExtension.Click += new System.EventHandler(this.BtnRemoveFileExtension_Click);
            // 
            // BtnAddFileExtension
            // 
            this.BtnAddFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAddFileExtension.Location = new System.Drawing.Point(381, 3);
            this.BtnAddFileExtension.Name = "BtnAddFileExtension";
            this.BtnAddFileExtension.Size = new System.Drawing.Size(21, 23);
            this.BtnAddFileExtension.TabIndex = 8;
            this.BtnAddFileExtension.Text = "+";
            this.BtnAddFileExtension.UseVisualStyleBackColor = true;
            this.BtnAddFileExtension.Click += new System.EventHandler(this.BtnAddFileExtension_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Excluding:";
            // 
            // LsbExcludedDirectories
            // 
            this.LsbExcludedDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LsbExcludedDirectories.FormattingEnabled = true;
            this.LsbExcludedDirectories.Location = new System.Drawing.Point(94, 3);
            this.LsbExcludedDirectories.Name = "LsbExcludedDirectories";
            this.LsbExcludedDirectories.Size = new System.Drawing.Size(281, 147);
            this.LsbExcludedDirectories.TabIndex = 12;
            // 
            // BtnAddExcludedDirectory
            // 
            this.BtnAddExcludedDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAddExcludedDirectory.Location = new System.Drawing.Point(381, 3);
            this.BtnAddExcludedDirectory.Name = "BtnAddExcludedDirectory";
            this.BtnAddExcludedDirectory.Size = new System.Drawing.Size(21, 23);
            this.BtnAddExcludedDirectory.TabIndex = 13;
            this.BtnAddExcludedDirectory.Text = "+";
            this.BtnAddExcludedDirectory.UseVisualStyleBackColor = true;
            this.BtnAddExcludedDirectory.Click += new System.EventHandler(this.BtnAddExcludedDirectory_Click);
            // 
            // BtnRemoveExcludedDirectory
            // 
            this.BtnRemoveExcludedDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveExcludedDirectory.Location = new System.Drawing.Point(381, 32);
            this.BtnRemoveExcludedDirectory.Name = "BtnRemoveExcludedDirectory";
            this.BtnRemoveExcludedDirectory.Size = new System.Drawing.Size(21, 23);
            this.BtnRemoveExcludedDirectory.TabIndex = 14;
            this.BtnRemoveExcludedDirectory.Text = "-";
            this.BtnRemoveExcludedDirectory.UseVisualStyleBackColor = true;
            this.BtnRemoveExcludedDirectory.Click += new System.EventHandler(this.BtnRemoveExcludedDirectory_Click);
            // 
            // TxtFileExtension
            // 
            this.TxtFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtFileExtension.Location = new System.Drawing.Point(94, 3);
            this.TxtFileExtension.Name = "TxtFileExtension";
            this.TxtFileExtension.Size = new System.Drawing.Size(281, 20);
            this.TxtFileExtension.TabIndex = 10;
            // 
            // Config
            // 
            this.AcceptButton = this.BtnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(426, 412);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.BtnSelectSourcePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtSourcePath);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Name = "Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Config";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnSelectSourcePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtSourcePath;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnOkay;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox LsbFileExtensions;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnRemoveFileExtension;
        private System.Windows.Forms.Button BtnAddFileExtension;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox LsbExcludedDirectories;
        private System.Windows.Forms.Button BtnAddExcludedDirectory;
        private System.Windows.Forms.Button BtnRemoveExcludedDirectory;
        private System.Windows.Forms.TextBox TxtFileExtension;
    }
}