namespace VideoDedup
{
    partial class VideoDedup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoDedup));
            this.BtnToDoManager = new System.Windows.Forms.Button();
            this.BtnDedup = new System.Windows.Forms.Button();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.LblStatusInfo = new System.Windows.Forms.Label();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnConfig = new System.Windows.Forms.Button();
            this.LblTimer = new System.Windows.Forms.Label();
            this.ElapsedTimer = new System.Windows.Forms.Timer(this.components);
            this.BtnResolveDuplicates = new System.Windows.Forms.Button();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LblDuplicateCount = new System.Windows.Forms.Label();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.BtnDiscardDuplicates = new System.Windows.Forms.Button();
            this.TxtLog = new System.Windows.Forms.TextBox();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnToDoManager
            // 
            this.BtnToDoManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnToDoManager.Location = new System.Drawing.Point(93, 344);
            this.BtnToDoManager.Name = "BtnToDoManager";
            this.BtnToDoManager.Size = new System.Drawing.Size(75, 23);
            this.BtnToDoManager.TabIndex = 1;
            this.BtnToDoManager.Text = "&ToDo List";
            this.BtnToDoManager.UseVisualStyleBackColor = true;
            this.BtnToDoManager.Click += new System.EventHandler(this.BtnToDoManager_Click);
            // 
            // BtnDedup
            // 
            this.BtnDedup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDedup.Location = new System.Drawing.Point(651, 344);
            this.BtnDedup.Name = "BtnDedup";
            this.BtnDedup.Size = new System.Drawing.Size(75, 23);
            this.BtnDedup.TabIndex = 0;
            this.BtnDedup.Text = "&Dedup";
            this.BtnDedup.UseVisualStyleBackColor = true;
            this.BtnDedup.Click += new System.EventHandler(this.BtnDedup_Click);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(12, 29);
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
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Enabled = false;
            this.BtnCancel.Location = new System.Drawing.Point(570, 344);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 7;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnConfig
            // 
            this.BtnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnConfig.Location = new System.Drawing.Point(12, 344);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.Size = new System.Drawing.Size(75, 23);
            this.BtnConfig.TabIndex = 8;
            this.BtnConfig.Text = "C&onfig";
            this.BtnConfig.UseVisualStyleBackColor = true;
            this.BtnConfig.Click += new System.EventHandler(this.BtnConfig_Click);
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
            // ElapsedTimer
            // 
            this.ElapsedTimer.Interval = 1000;
            this.ElapsedTimer.Tick += new System.EventHandler(this.ElapsedTimer_Tick);
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
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
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
            this.BtnDiscardDuplicates.Location = new System.Drawing.Point(460, 344);
            this.BtnDiscardDuplicates.Name = "BtnDiscardDuplicates";
            this.BtnDiscardDuplicates.Size = new System.Drawing.Size(104, 23);
            this.BtnDiscardDuplicates.TabIndex = 14;
            this.BtnDiscardDuplicates.Text = "Discard Duplicates";
            this.BtnDiscardDuplicates.UseVisualStyleBackColor = true;
            this.BtnDiscardDuplicates.Click += new System.EventHandler(this.BtnDiscard_Click);
            // 
            // TxtLog
            // 
            this.TxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLog.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.TxtLog.Location = new System.Drawing.Point(13, 87);
            this.TxtLog.Multiline = true;
            this.TxtLog.Name = "TxtLog";
            this.TxtLog.ReadOnly = true;
            this.TxtLog.Size = new System.Drawing.Size(794, 251);
            this.TxtLog.TabIndex = 15;
            // 
            // VideoDedup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(819, 379);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.BtnDiscardDuplicates);
            this.Controls.Add(this.LblDuplicateCount);
            this.Controls.Add(this.BtnResolveDuplicates);
            this.Controls.Add(this.LblTimer);
            this.Controls.Add(this.BtnConfig);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.LblStatusInfo);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.BtnDedup);
            this.Controls.Add(this.BtnToDoManager);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "VideoDedup";
            this.Text = "VideoDedup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VideoDedup_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnToDoManager;
        private System.Windows.Forms.Button BtnDedup;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label LblStatusInfo;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnConfig;
        private System.Windows.Forms.Label LblTimer;
        private System.Windows.Forms.Timer ElapsedTimer;
        private System.Windows.Forms.Button BtnResolveDuplicates;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label LblDuplicateCount;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.Button BtnDiscardDuplicates;
        private System.Windows.Forms.TextBox TxtLog;
    }
}

