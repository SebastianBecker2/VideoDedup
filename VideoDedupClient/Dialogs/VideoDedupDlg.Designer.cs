namespace VideoDedupClient.Dialogs
{
    using System.ComponentModel;
    using Controls.StatusInfo;

    partial class VideoDedupDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            var resources = new ComponentResourceManager(typeof(VideoDedupDlg));
            BtnServerConfig = new Button();
            BtnResolveDuplicates = new Button();
            MenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            ClientConfigurationToolStripMenuItem = new ToolStripMenuItem();
            ServerConfigurationToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            BtnDiscardDuplicates = new Button();
            BtnClientConfig = new Button();
            DgvLog = new DataGridView();
            DgcLogMessage = new DataGridViewTextBoxColumn();
            StiProgress = new StatusInfoCtl();
            MenuStrip.SuspendLayout();
            ((ISupportInitialize)DgvLog).BeginInit();
            SuspendLayout();
            // 
            // BtnServerConfig
            // 
            BtnServerConfig.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnServerConfig.Location = new Point(155, 474);
            BtnServerConfig.Margin = new Padding(4, 3, 4, 3);
            BtnServerConfig.Name = "BtnServerConfig";
            BtnServerConfig.Size = new Size(134, 27);
            BtnServerConfig.TabIndex = 1;
            BtnServerConfig.Text = "&Server Configuration";
            BtnServerConfig.UseVisualStyleBackColor = true;
            BtnServerConfig.Click += BtnServerConfig_Click;
            // 
            // BtnResolveDuplicates
            // 
            BtnResolveDuplicates.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnResolveDuplicates.Enabled = false;
            BtnResolveDuplicates.Location = new Point(820, 474);
            BtnResolveDuplicates.Margin = new Padding(4, 3, 4, 3);
            BtnResolveDuplicates.Name = "BtnResolveDuplicates";
            BtnResolveDuplicates.Size = new Size(121, 27);
            BtnResolveDuplicates.TabIndex = 3;
            BtnResolveDuplicates.Text = "&Resolve Duplicates";
            BtnResolveDuplicates.UseVisualStyleBackColor = true;
            BtnResolveDuplicates.Click += BtnResolveConflicts_Click;
            // 
            // MenuStrip
            // 
            MenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolStripMenuItem1 });
            MenuStrip.Location = new Point(0, 0);
            MenuStrip.Name = "MenuStrip";
            MenuStrip.Padding = new Padding(7, 2, 0, 2);
            MenuStrip.Size = new Size(955, 24);
            MenuStrip.TabIndex = 11;
            MenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ClientConfigurationToolStripMenuItem, ServerConfigurationToolStripMenuItem, closeToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // ClientConfigurationToolStripMenuItem
            // 
            ClientConfigurationToolStripMenuItem.Name = "ClientConfigurationToolStripMenuItem";
            ClientConfigurationToolStripMenuItem.Size = new Size(183, 22);
            ClientConfigurationToolStripMenuItem.Text = "&Client Configuration";
            ClientConfigurationToolStripMenuItem.Click += ClientConfigurationToolStripMenuItem_Click;
            // 
            // ServerConfigurationToolStripMenuItem
            // 
            ServerConfigurationToolStripMenuItem.Name = "ServerConfigurationToolStripMenuItem";
            ServerConfigurationToolStripMenuItem.Size = new Size(183, 22);
            ServerConfigurationToolStripMenuItem.Text = "&Server Configuration";
            ServerConfigurationToolStripMenuItem.Click += ServerConfigurationToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(183, 22);
            closeToolStripMenuItem.Text = "C&lose";
            closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(24, 20);
            toolStripMenuItem1.Text = "&?";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(107, 22);
            aboutToolStripMenuItem.Text = "&About";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // BtnDiscardDuplicates
            // 
            BtnDiscardDuplicates.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnDiscardDuplicates.DialogResult = DialogResult.Cancel;
            BtnDiscardDuplicates.Enabled = false;
            BtnDiscardDuplicates.Location = new Point(691, 474);
            BtnDiscardDuplicates.Margin = new Padding(4, 3, 4, 3);
            BtnDiscardDuplicates.Name = "BtnDiscardDuplicates";
            BtnDiscardDuplicates.Size = new Size(121, 27);
            BtnDiscardDuplicates.TabIndex = 2;
            BtnDiscardDuplicates.Text = "&Discard Duplicates";
            BtnDiscardDuplicates.UseVisualStyleBackColor = true;
            BtnDiscardDuplicates.Click += BtnDiscard_Click;
            // 
            // BtnClientConfig
            // 
            BtnClientConfig.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnClientConfig.Location = new Point(14, 474);
            BtnClientConfig.Margin = new Padding(4, 3, 4, 3);
            BtnClientConfig.Name = "BtnClientConfig";
            BtnClientConfig.Size = new Size(134, 27);
            BtnClientConfig.TabIndex = 0;
            BtnClientConfig.Text = "&Client Configuration";
            BtnClientConfig.UseVisualStyleBackColor = true;
            BtnClientConfig.Click += BtnClientConfig_Click;
            // 
            // DgvLog
            // 
            DgvLog.AllowUserToAddRows = false;
            DgvLog.AllowUserToDeleteRows = false;
            DgvLog.AllowUserToResizeColumns = false;
            DgvLog.AllowUserToResizeRows = false;
            DgvLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DgvLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvLog.ColumnHeadersVisible = false;
            DgvLog.Columns.AddRange(new DataGridViewColumn[] { DgcLogMessage });
            DgvLog.Location = new Point(14, 305);
            DgvLog.Margin = new Padding(4, 3, 4, 3);
            DgvLog.MultiSelect = false;
            DgvLog.Name = "DgvLog";
            DgvLog.ReadOnly = true;
            DgvLog.RowHeadersVisible = false;
            DgvLog.Size = new Size(927, 162);
            DgvLog.TabIndex = 4;
            DgvLog.VirtualMode = true;
            DgvLog.CellValueNeeded += DgvLog_CellValueNeeded;
            // 
            // DgcLogMessage
            // 
            DgcLogMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DgcLogMessage.HeaderText = "Message";
            DgcLogMessage.Name = "DgcLogMessage";
            DgcLogMessage.ReadOnly = true;
            // 
            // StiProgress
            // 
            StiProgress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            StiProgress.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            StiProgress.Location = new Point(14, 27);
            StiProgress.Margin = new Padding(4, 3, 4, 3);
            StiProgress.Name = "StiProgress";
            StiProgress.Size = new Size(927, 272);
            StiProgress.TabIndex = 12;
            // 
            // VideoDedupDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(955, 514);
            Controls.Add(StiProgress);
            Controls.Add(DgvLog);
            Controls.Add(BtnClientConfig);
            Controls.Add(BtnDiscardDuplicates);
            Controls.Add(BtnResolveDuplicates);
            Controls.Add(BtnServerConfig);
            Controls.Add(MenuStrip);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = MenuStrip;
            Margin = new Padding(4, 3, 4, 3);
            Name = "VideoDedupDlg";
            Text = "Video Dedup Client";
            Load += VideoDedupDlg_Load;
            MenuStrip.ResumeLayout(false);
            MenuStrip.PerformLayout();
            ((ISupportInitialize)DgvLog).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button BtnServerConfig;
        private Button BtnResolveDuplicates;
        private MenuStrip MenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Button BtnDiscardDuplicates;
        private ToolStripMenuItem ServerConfigurationToolStripMenuItem;
        private Button BtnClientConfig;
        private DataGridView DgvLog;
        private DataGridViewTextBoxColumn DgcLogMessage;
        private ToolStripMenuItem ClientConfigurationToolStripMenuItem;
        private StatusInfoCtl StiProgress;
    }
}

