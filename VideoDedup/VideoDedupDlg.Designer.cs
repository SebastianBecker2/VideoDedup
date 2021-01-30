namespace VideoDedup
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoDedupDlg));
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.LblStatusInfo = new System.Windows.Forms.Label();
            this.BtnServerConfig = new System.Windows.Forms.Button();
            this.LblTimer = new System.Windows.Forms.Label();
            this.BtnResolveDuplicates = new System.Windows.Forms.Button();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LblDuplicateCount = new System.Windows.Forms.Label();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.BtnDiscardDuplicates = new System.Windows.Forms.Button();
            this.BtnClientConfig = new System.Windows.Forms.Button();
            this.DgvLog = new System.Windows.Forms.DataGridView();
            this.DgcLogMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLog)).BeginInit();
            this.SuspendLayout();
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(12, 29);
            this.ProgressBar.MarqueeAnimationSpeed = 50;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(795, 23);
            this.ProgressBar.TabIndex = 5;
            // 
            // LblStatusInfo
            // 
            this.LblStatusInfo.AutoSize = true;
            this.LblStatusInfo.Location = new System.Drawing.Point(9, 55);
            this.LblStatusInfo.Name = "LblStatusInfo";
            this.LblStatusInfo.Size = new System.Drawing.Size(49, 13);
            this.LblStatusInfo.TabIndex = 6;
            this.LblStatusInfo.Text = "Status:...";
            // 
            // BtnServerConfig
            // 
            this.BtnServerConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnServerConfig.Location = new System.Drawing.Point(133, 344);
            this.BtnServerConfig.Name = "BtnServerConfig";
            this.BtnServerConfig.Size = new System.Drawing.Size(115, 23);
            this.BtnServerConfig.TabIndex = 8;
            this.BtnServerConfig.Text = "Server Configuration";
            this.BtnServerConfig.UseVisualStyleBackColor = true;
            this.BtnServerConfig.Click += new System.EventHandler(this.BtnServerConfig_Click);
            // 
            // LblTimer
            // 
            this.LblTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblTimer.AutoSize = true;
            this.LblTimer.Location = new System.Drawing.Point(758, 55);
            this.LblTimer.Name = "LblTimer";
            this.LblTimer.Size = new System.Drawing.Size(49, 13);
            this.LblTimer.TabIndex = 9;
            this.LblTimer.Text = "00:00:00";
            // 
            // BtnResolveDuplicates
            // 
            this.BtnResolveDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnResolveDuplicates.Enabled = false;
            this.BtnResolveDuplicates.Location = new System.Drawing.Point(732, 344);
            this.BtnResolveDuplicates.Name = "BtnResolveDuplicates";
            this.BtnResolveDuplicates.Size = new System.Drawing.Size(75, 23);
            this.BtnResolveDuplicates.TabIndex = 10;
            this.BtnResolveDuplicates.Text = "&Resolve";
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
            this.MenuStrip.Size = new System.Drawing.Size(819, 24);
            this.MenuStrip.TabIndex = 11;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverConfigurationToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // serverConfigurationToolStripMenuItem
            // 
            this.serverConfigurationToolStripMenuItem.Name = "serverConfigurationToolStripMenuItem";
            this.serverConfigurationToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.serverConfigurationToolStripMenuItem.Text = "Server Configuration";
            this.serverConfigurationToolStripMenuItem.Click += new System.EventHandler(this.ServerConfigurationToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.closeToolStripMenuItem.Text = "&Close";
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
            // LblDuplicateCount
            // 
            this.LblDuplicateCount.AutoSize = true;
            this.LblDuplicateCount.Location = new System.Drawing.Point(9, 70);
            this.LblDuplicateCount.Name = "LblDuplicateCount";
            this.LblDuplicateCount.Size = new System.Drawing.Size(99, 13);
            this.LblDuplicateCount.TabIndex = 12;
            this.LblDuplicateCount.Text = "Duplicates found: 0";
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "VideoDedup";
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseDoubleClick);
            // 
            // BtnDiscardDuplicates
            // 
            this.BtnDiscardDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDiscardDuplicates.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnDiscardDuplicates.Enabled = false;
            this.BtnDiscardDuplicates.Location = new System.Drawing.Point(622, 344);
            this.BtnDiscardDuplicates.Name = "BtnDiscardDuplicates";
            this.BtnDiscardDuplicates.Size = new System.Drawing.Size(104, 23);
            this.BtnDiscardDuplicates.TabIndex = 14;
            this.BtnDiscardDuplicates.Text = "Discard Duplicates";
            this.BtnDiscardDuplicates.UseVisualStyleBackColor = true;
            this.BtnDiscardDuplicates.Click += new System.EventHandler(this.BtnDiscard_Click);
            // 
            // BtnClientConfig
            // 
            this.BtnClientConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnClientConfig.Location = new System.Drawing.Point(12, 344);
            this.BtnClientConfig.Name = "BtnClientConfig";
            this.BtnClientConfig.Size = new System.Drawing.Size(115, 23);
            this.BtnClientConfig.TabIndex = 16;
            this.BtnClientConfig.Text = "Client Configuration";
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
            this.DgvLog.Location = new System.Drawing.Point(12, 87);
            this.DgvLog.MultiSelect = false;
            this.DgvLog.Name = "DgvLog";
            this.DgvLog.ReadOnly = true;
            this.DgvLog.RowHeadersVisible = false;
            this.DgvLog.Size = new System.Drawing.Size(795, 251);
            this.DgvLog.TabIndex = 17;
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
            // VideoDedupDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 379);
            this.Controls.Add(this.DgvLog);
            this.Controls.Add(this.BtnClientConfig);
            this.Controls.Add(this.BtnDiscardDuplicates);
            this.Controls.Add(this.LblDuplicateCount);
            this.Controls.Add(this.BtnResolveDuplicates);
            this.Controls.Add(this.LblTimer);
            this.Controls.Add(this.BtnServerConfig);
            this.Controls.Add(this.LblStatusInfo);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "VideoDedupDlg";
            this.Text = "Video Dedup Client";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label LblStatusInfo;
        private System.Windows.Forms.Button BtnServerConfig;
        private System.Windows.Forms.Label LblTimer;
        private System.Windows.Forms.Button BtnResolveDuplicates;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label LblDuplicateCount;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.Button BtnDiscardDuplicates;
        private System.Windows.Forms.ToolStripMenuItem serverConfigurationToolStripMenuItem;
        private System.Windows.Forms.Button BtnClientConfig;
        private System.Windows.Forms.DataGridView DgvLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn DgcLogMessage;
    }
}

