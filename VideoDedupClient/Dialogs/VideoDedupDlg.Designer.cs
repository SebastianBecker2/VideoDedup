namespace VideoDedupClient.Dialogs
{
    partial class VideoDedupDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoDedupDlg));
            this.BtnServerConfig = new System.Windows.Forms.Button();
            this.BtnResolveDuplicates = new System.Windows.Forms.Button();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClientConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ServerConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnDiscardDuplicates = new System.Windows.Forms.Button();
            this.BtnClientConfig = new System.Windows.Forms.Button();
            this.DgvLog = new System.Windows.Forms.DataGridView();
            this.DgcLogMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StiProgress = new VideoDedupClient.Controls.StatusInfo.StatusInfoCtl();
            this.MenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLog)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnServerConfig
            // 
            this.BtnServerConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnServerConfig.Location = new System.Drawing.Point(155, 397);
            this.BtnServerConfig.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnServerConfig.Name = "BtnServerConfig";
            this.BtnServerConfig.Size = new System.Drawing.Size(134, 27);
            this.BtnServerConfig.TabIndex = 1;
            this.BtnServerConfig.Text = "&Server Configuration";
            this.BtnServerConfig.UseVisualStyleBackColor = true;
            this.BtnServerConfig.Click += new System.EventHandler(this.BtnServerConfig_Click);
            // 
            // BtnResolveDuplicates
            // 
            this.BtnResolveDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnResolveDuplicates.Enabled = false;
            this.BtnResolveDuplicates.Location = new System.Drawing.Point(820, 397);
            this.BtnResolveDuplicates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnResolveDuplicates.Name = "BtnResolveDuplicates";
            this.BtnResolveDuplicates.Size = new System.Drawing.Size(121, 27);
            this.BtnResolveDuplicates.TabIndex = 3;
            this.BtnResolveDuplicates.Text = "&Resolve Duplicates";
            this.BtnResolveDuplicates.UseVisualStyleBackColor = true;
            this.BtnResolveDuplicates.Click += new System.EventHandler(this.BtnResolveConflicts_Click);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem1});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.MenuStrip.Size = new System.Drawing.Size(955, 24);
            this.MenuStrip.TabIndex = 11;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClientConfigurationToolStripMenuItem,
            this.ServerConfigurationToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // ClientConfigurationToolStripMenuItem
            // 
            this.ClientConfigurationToolStripMenuItem.Name = "ClientConfigurationToolStripMenuItem";
            this.ClientConfigurationToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.ClientConfigurationToolStripMenuItem.Text = "&Client Configuration";
            this.ClientConfigurationToolStripMenuItem.Click += new System.EventHandler(this.ClientConfigurationToolStripMenuItem_Click);
            // 
            // ServerConfigurationToolStripMenuItem
            // 
            this.ServerConfigurationToolStripMenuItem.Name = "ServerConfigurationToolStripMenuItem";
            this.ServerConfigurationToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.ServerConfigurationToolStripMenuItem.Text = "&Server Configuration";
            this.ServerConfigurationToolStripMenuItem.Click += new System.EventHandler(this.ServerConfigurationToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.closeToolStripMenuItem.Text = "C&lose";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(24, 20);
            this.toolStripMenuItem1.Text = "&?";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // BtnDiscardDuplicates
            // 
            this.BtnDiscardDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDiscardDuplicates.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnDiscardDuplicates.Enabled = false;
            this.BtnDiscardDuplicates.Location = new System.Drawing.Point(691, 397);
            this.BtnDiscardDuplicates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnDiscardDuplicates.Name = "BtnDiscardDuplicates";
            this.BtnDiscardDuplicates.Size = new System.Drawing.Size(121, 27);
            this.BtnDiscardDuplicates.TabIndex = 2;
            this.BtnDiscardDuplicates.Text = "&Discard Duplicates";
            this.BtnDiscardDuplicates.UseVisualStyleBackColor = true;
            this.BtnDiscardDuplicates.Click += new System.EventHandler(this.BtnDiscard_Click);
            // 
            // BtnClientConfig
            // 
            this.BtnClientConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnClientConfig.Location = new System.Drawing.Point(14, 397);
            this.BtnClientConfig.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnClientConfig.Name = "BtnClientConfig";
            this.BtnClientConfig.Size = new System.Drawing.Size(134, 27);
            this.BtnClientConfig.TabIndex = 0;
            this.BtnClientConfig.Text = "&Client Configuration";
            this.BtnClientConfig.UseVisualStyleBackColor = true;
            this.BtnClientConfig.Click += new System.EventHandler(this.BtnClientConfig_Click);
            // 
            // DgvLog
            // 
            this.DgvLog.AllowUserToAddRows = false;
            this.DgvLog.AllowUserToDeleteRows = false;
            this.DgvLog.AllowUserToResizeColumns = false;
            this.DgvLog.AllowUserToResizeRows = false;
            this.DgvLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvLog.ColumnHeadersVisible = false;
            this.DgvLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DgcLogMessage});
            this.DgvLog.Location = new System.Drawing.Point(14, 172);
            this.DgvLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DgvLog.MultiSelect = false;
            this.DgvLog.Name = "DgvLog";
            this.DgvLog.ReadOnly = true;
            this.DgvLog.RowHeadersVisible = false;
            this.DgvLog.Size = new System.Drawing.Size(927, 218);
            this.DgvLog.TabIndex = 4;
            this.DgvLog.VirtualMode = true;
            this.DgvLog.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.DgvLog_CellValueNeeded);
            // 
            // DgcLogMessage
            // 
            this.DgcLogMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DgcLogMessage.HeaderText = "Message";
            this.DgcLogMessage.Name = "DgcLogMessage";
            this.DgcLogMessage.ReadOnly = true;
            // 
            // StiProgress
            // 
            this.StiProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StiProgress.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.StiProgress.Location = new System.Drawing.Point(14, 27);
            this.StiProgress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StiProgress.Name = "StiProgress";
            this.StiProgress.Size = new System.Drawing.Size(927, 139);
            this.StiProgress.TabIndex = 12;
            // 
            // VideoDedupDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 437);
            this.Controls.Add(this.StiProgress);
            this.Controls.Add(this.DgvLog);
            this.Controls.Add(this.BtnClientConfig);
            this.Controls.Add(this.BtnDiscardDuplicates);
            this.Controls.Add(this.BtnResolveDuplicates);
            this.Controls.Add(this.BtnServerConfig);
            this.Controls.Add(this.MenuStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "VideoDedupDlg";
            this.Text = "Video Dedup Client";
            this.Load += new System.EventHandler(this.VideoDedupDlg_Load);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnServerConfig;
        private System.Windows.Forms.Button BtnResolveDuplicates;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button BtnDiscardDuplicates;
        private System.Windows.Forms.ToolStripMenuItem ServerConfigurationToolStripMenuItem;
        private System.Windows.Forms.Button BtnClientConfig;
        private System.Windows.Forms.DataGridView DgvLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn DgcLogMessage;
        private System.Windows.Forms.ToolStripMenuItem ClientConfigurationToolStripMenuItem;
        private Controls.StatusInfo.StatusInfoCtl StiProgress;
    }
}

