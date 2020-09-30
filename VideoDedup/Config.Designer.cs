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
            this.label3 = new System.Windows.Forms.Label();
            this.LsbExceptionPaths = new System.Windows.Forms.ListBox();
            this.BtnAddExceptionPath = new System.Windows.Forms.Button();
            this.BtnRemoveExceptionPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnSelectSourcePath
            // 
            this.BtnSelectSourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSelectSourcePath.Location = new System.Drawing.Point(358, 10);
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
            this.TxtSourcePath.Size = new System.Drawing.Size(262, 20);
            this.TxtSourcePath.TabIndex = 17;
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(304, 166);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 21;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            this.BtnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOkay.Location = new System.Drawing.Point(223, 166);
            this.BtnOkay.Name = "BtnOkay";
            this.BtnOkay.Size = new System.Drawing.Size(75, 23);
            this.BtnOkay.TabIndex = 20;
            this.BtnOkay.Text = "OK";
            this.BtnOkay.UseVisualStyleBackColor = true;
            this.BtnOkay.Click += new System.EventHandler(this.BtnOkay_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Excluding:";
            // 
            // LsbExceptionPaths
            // 
            this.LsbExceptionPaths.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LsbExceptionPaths.FormattingEnabled = true;
            this.LsbExceptionPaths.Location = new System.Drawing.Point(90, 38);
            this.LsbExceptionPaths.Name = "LsbExceptionPaths";
            this.LsbExceptionPaths.Size = new System.Drawing.Size(262, 108);
            this.LsbExceptionPaths.TabIndex = 12;
            // 
            // BtnAddExceptionPath
            // 
            this.BtnAddExceptionPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAddExceptionPath.Location = new System.Drawing.Point(358, 38);
            this.BtnAddExceptionPath.Name = "BtnAddExceptionPath";
            this.BtnAddExceptionPath.Size = new System.Drawing.Size(21, 23);
            this.BtnAddExceptionPath.TabIndex = 13;
            this.BtnAddExceptionPath.Text = "+";
            this.BtnAddExceptionPath.UseVisualStyleBackColor = true;
            this.BtnAddExceptionPath.Click += new System.EventHandler(this.BtnAddExceptionPath_Click);
            // 
            // BtnRemoveExceptionPath
            // 
            this.BtnRemoveExceptionPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveExceptionPath.Location = new System.Drawing.Point(358, 67);
            this.BtnRemoveExceptionPath.Name = "BtnRemoveExceptionPath";
            this.BtnRemoveExceptionPath.Size = new System.Drawing.Size(21, 23);
            this.BtnRemoveExceptionPath.TabIndex = 14;
            this.BtnRemoveExceptionPath.Text = "-";
            this.BtnRemoveExceptionPath.UseVisualStyleBackColor = true;
            this.BtnRemoveExceptionPath.Click += new System.EventHandler(this.BtnRemoveExceptionPath_Click);
            // 
            // Config
            // 
            this.AcceptButton = this.BtnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(391, 201);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnSelectSourcePath);
            this.Controls.Add(this.LsbExceptionPaths);
            this.Controls.Add(this.BtnAddExceptionPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnRemoveExceptionPath);
            this.Controls.Add(this.TxtSourcePath);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Name = "Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnSelectSourcePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtSourcePath;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnOkay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox LsbExceptionPaths;
        private System.Windows.Forms.Button BtnAddExceptionPath;
        private System.Windows.Forms.Button BtnRemoveExceptionPath;
    }
}