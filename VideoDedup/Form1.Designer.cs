namespace VideoDedup
{
    partial class Form1
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
            this.BtnToDoManager = new System.Windows.Forms.Button();
            this.BtnDedup = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.LblStatusInfo = new System.Windows.Forms.Label();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnConfig = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnToDoManager
            // 
            this.BtnToDoManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnToDoManager.Location = new System.Drawing.Point(93, 107);
            this.BtnToDoManager.Name = "BtnToDoManager";
            this.BtnToDoManager.Size = new System.Drawing.Size(75, 23);
            this.BtnToDoManager.TabIndex = 1;
            this.BtnToDoManager.Text = "ToDo List";
            this.BtnToDoManager.UseVisualStyleBackColor = true;
            this.BtnToDoManager.Click += new System.EventHandler(this.BtnToDoManager_Click);
            // 
            // BtnDedup
            // 
            this.BtnDedup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDedup.Location = new System.Drawing.Point(506, 107);
            this.BtnDedup.Name = "BtnDedup";
            this.BtnDedup.Size = new System.Drawing.Size(75, 23);
            this.BtnDedup.TabIndex = 0;
            this.BtnDedup.Text = "Dedup";
            this.BtnDedup.UseVisualStyleBackColor = true;
            this.BtnDedup.Click += new System.EventHandler(this.BtnDedup_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(569, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // LblStatusInfo
            // 
            this.LblStatusInfo.AutoSize = true;
            this.LblStatusInfo.Location = new System.Drawing.Point(9, 38);
            this.LblStatusInfo.Name = "LblStatusInfo";
            this.LblStatusInfo.Size = new System.Drawing.Size(49, 13);
            this.LblStatusInfo.TabIndex = 6;
            this.LblStatusInfo.Text = "Status:...";
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.Enabled = false;
            this.BtnCancel.Location = new System.Drawing.Point(425, 107);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 7;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnConfig
            // 
            this.BtnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnConfig.Location = new System.Drawing.Point(12, 107);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.Size = new System.Drawing.Size(75, 23);
            this.BtnConfig.TabIndex = 8;
            this.BtnConfig.Text = "Config";
            this.BtnConfig.UseVisualStyleBackColor = true;
            this.BtnConfig.Click += new System.EventHandler(this.BtnConfig_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 142);
            this.Controls.Add(this.BtnConfig);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.LblStatusInfo);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.BtnDedup);
            this.Controls.Add(this.BtnToDoManager);
            this.Name = "Form1";
            this.Text = "VideoFileDedup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnToDoManager;
        private System.Windows.Forms.Button BtnDedup;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LblStatusInfo;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnConfig;
    }
}

