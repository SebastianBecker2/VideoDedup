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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.BtnToDoManager = new System.Windows.Forms.Button();
            this.BtnDedup = new System.Windows.Forms.Button();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.LblStatusInfo = new System.Windows.Forms.Label();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnConfig = new System.Windows.Forms.Button();
            this.LblTimer = new System.Windows.Forms.Label();
            this.ElapsedTimer = new System.Windows.Forms.Timer(this.components);
            this.BtnResolveConflicts = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LblDuplicateCount = new System.Windows.Forms.Label();
            this.LblCurrentFile = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnToDoManager
            // 
            this.BtnToDoManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnToDoManager.Location = new System.Drawing.Point(93, 135);
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
            this.BtnDedup.Location = new System.Drawing.Point(425, 135);
            this.BtnDedup.Name = "BtnDedup";
            this.BtnDedup.Size = new System.Drawing.Size(75, 23);
            this.BtnDedup.TabIndex = 0;
            this.BtnDedup.Text = "Dedup";
            this.BtnDedup.UseVisualStyleBackColor = true;
            this.BtnDedup.Click += new System.EventHandler(this.BtnDedup_Click);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(12, 29);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(569, 23);
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
            this.BtnCancel.Enabled = false;
            this.BtnCancel.Location = new System.Drawing.Point(344, 135);
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
            this.BtnConfig.Location = new System.Drawing.Point(12, 135);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.Size = new System.Drawing.Size(75, 23);
            this.BtnConfig.TabIndex = 8;
            this.BtnConfig.Text = "Config";
            this.BtnConfig.UseVisualStyleBackColor = true;
            this.BtnConfig.Click += new System.EventHandler(this.BtnConfig_Click);
            // 
            // LblTimer
            // 
            this.LblTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblTimer.AutoSize = true;
            this.LblTimer.Location = new System.Drawing.Point(532, 55);
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
            // BtnResolveConflicts
            // 
            this.BtnResolveConflicts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnResolveConflicts.Enabled = false;
            this.BtnResolveConflicts.Location = new System.Drawing.Point(506, 135);
            this.BtnResolveConflicts.Name = "BtnResolveConflicts";
            this.BtnResolveConflicts.Size = new System.Drawing.Size(75, 23);
            this.BtnResolveConflicts.TabIndex = 10;
            this.BtnResolveConflicts.Text = "Resolve";
            this.BtnResolveConflicts.UseVisualStyleBackColor = true;
            this.BtnResolveConflicts.Click += new System.EventHandler(this.BtnResolveConflicts_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(593, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(24, 20);
            this.toolStripMenuItem1.Text = "?";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            // LblCurrentFile
            // 
            this.LblCurrentFile.AutoSize = true;
            this.LblCurrentFile.Location = new System.Drawing.Point(9, 85);
            this.LblCurrentFile.Name = "LblCurrentFile";
            this.LblCurrentFile.Size = new System.Drawing.Size(55, 13);
            this.LblCurrentFile.TabIndex = 13;
            this.LblCurrentFile.Text = "Checking:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 170);
            this.Controls.Add(this.LblCurrentFile);
            this.Controls.Add(this.LblDuplicateCount);
            this.Controls.Add(this.BtnResolveConflicts);
            this.Controls.Add(this.LblTimer);
            this.Controls.Add(this.BtnConfig);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.LblStatusInfo);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.BtnDedup);
            this.Controls.Add(this.BtnToDoManager);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "VideoFileDedup";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.Button BtnResolveConflicts;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label LblDuplicateCount;
        private System.Windows.Forms.Label LblCurrentFile;
    }
}

