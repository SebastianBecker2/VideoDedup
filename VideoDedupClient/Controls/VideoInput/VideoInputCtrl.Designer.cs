namespace VideoDedupClient.Controls.VideoInput
{
    partial class VideoInputCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox3 = new GroupBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            BtnSelectSourcePath = new Button();
            label1 = new Label();
            TxtSourcePath = new TextBox();
            BtnRemoveExcludedDirectory = new Button();
            BtnAddExcludedDirectory = new Button();
            label4 = new Label();
            LsbFileExtensions = new ListBox();
            BtnRemoveFileExtension = new Button();
            BtnAddFileExtension = new Button();
            TxtFileExtension = new TextBox();
            ChbRecursive = new CheckBox();
            LsbExcludedDirectories = new ListBox();
            label10 = new Label();
            ChbMonitorFileChanges = new CheckBox();
            groupBox3.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(tableLayoutPanel3);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(0, 0);
            groupBox3.Margin = new Padding(0);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 3, 4, 3);
            groupBox3.Size = new Size(440, 540);
            groupBox3.TabIndex = 1;
            groupBox3.TabStop = false;
            groupBox3.Text = "Video Input";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.Controls.Add(BtnSelectSourcePath, 2, 0);
            tableLayoutPanel3.Controls.Add(label1, 0, 0);
            tableLayoutPanel3.Controls.Add(TxtSourcePath, 1, 0);
            tableLayoutPanel3.Controls.Add(BtnRemoveExcludedDirectory, 2, 6);
            tableLayoutPanel3.Controls.Add(BtnAddExcludedDirectory, 2, 5);
            tableLayoutPanel3.Controls.Add(label4, 0, 5);
            tableLayoutPanel3.Controls.Add(LsbFileExtensions, 1, 4);
            tableLayoutPanel3.Controls.Add(BtnRemoveFileExtension, 2, 4);
            tableLayoutPanel3.Controls.Add(BtnAddFileExtension, 2, 3);
            tableLayoutPanel3.Controls.Add(TxtFileExtension, 1, 3);
            tableLayoutPanel3.Controls.Add(ChbRecursive, 1, 1);
            tableLayoutPanel3.Controls.Add(LsbExcludedDirectories, 1, 5);
            tableLayoutPanel3.Controls.Add(label10, 0, 3);
            tableLayoutPanel3.Controls.Add(ChbMonitorFileChanges, 1, 2);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(4, 19);
            tableLayoutPanel3.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 6;
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 52.21519F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 47.78481F));
            tableLayoutPanel3.Size = new Size(432, 518);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // BtnSelectSourcePath
            // 
            BtnSelectSourcePath.Anchor = AnchorStyles.Right;
            BtnSelectSourcePath.Location = new Point(404, 3);
            BtnSelectSourcePath.Margin = new Padding(4, 3, 4, 3);
            BtnSelectSourcePath.Name = "BtnSelectSourcePath";
            BtnSelectSourcePath.Size = new Size(24, 27);
            BtnSelectSourcePath.TabIndex = 1;
            BtnSelectSourcePath.Text = "...";
            BtnSelectSourcePath.UseVisualStyleBackColor = true;
            BtnSelectSourcePath.Click += BtnSelectSourcePath_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(4, 9);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(97, 15);
            label1.TabIndex = 18;
            label1.Text = "Source Directory:";
            // 
            // TxtSourcePath
            // 
            TxtSourcePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TxtSourcePath.Location = new Point(109, 5);
            TxtSourcePath.Margin = new Padding(4, 3, 4, 3);
            TxtSourcePath.Name = "TxtSourcePath";
            TxtSourcePath.Size = new Size(287, 23);
            TxtSourcePath.TabIndex = 0;
            // 
            // BtnRemoveExcludedDirectory
            // 
            BtnRemoveExcludedDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnRemoveExcludedDirectory.Location = new Point(404, 344);
            BtnRemoveExcludedDirectory.Margin = new Padding(4, 3, 4, 3);
            BtnRemoveExcludedDirectory.Name = "BtnRemoveExcludedDirectory";
            BtnRemoveExcludedDirectory.Size = new Size(24, 27);
            BtnRemoveExcludedDirectory.TabIndex = 10;
            BtnRemoveExcludedDirectory.Text = "-";
            BtnRemoveExcludedDirectory.UseVisualStyleBackColor = true;
            BtnRemoveExcludedDirectory.Click += BtnRemoveExcludedDirectory_Click;
            // 
            // BtnAddExcludedDirectory
            // 
            BtnAddExcludedDirectory.Anchor = AnchorStyles.Right;
            BtnAddExcludedDirectory.Location = new Point(404, 311);
            BtnAddExcludedDirectory.Margin = new Padding(4, 3, 4, 3);
            BtnAddExcludedDirectory.Name = "BtnAddExcludedDirectory";
            BtnAddExcludedDirectory.Size = new Size(24, 27);
            BtnAddExcludedDirectory.TabIndex = 9;
            BtnAddExcludedDirectory.Text = "+";
            BtnAddExcludedDirectory.UseVisualStyleBackColor = true;
            BtnAddExcludedDirectory.Click += BtnAddExcludedDirectory_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(39, 317);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(62, 15);
            label4.TabIndex = 15;
            label4.Text = "Excluding:";
            // 
            // LsbFileExtensions
            // 
            LsbFileExtensions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LsbFileExtensions.FormattingEnabled = true;
            LsbFileExtensions.ItemHeight = 15;
            LsbFileExtensions.Location = new Point(109, 119);
            LsbFileExtensions.Margin = new Padding(4, 3, 4, 3);
            LsbFileExtensions.Name = "LsbFileExtensions";
            LsbFileExtensions.Size = new Size(287, 184);
            LsbFileExtensions.TabIndex = 6;
            // 
            // BtnRemoveFileExtension
            // 
            BtnRemoveFileExtension.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnRemoveFileExtension.Location = new Point(404, 119);
            BtnRemoveFileExtension.Margin = new Padding(4, 3, 4, 3);
            BtnRemoveFileExtension.Name = "BtnRemoveFileExtension";
            BtnRemoveFileExtension.Size = new Size(24, 27);
            BtnRemoveFileExtension.TabIndex = 7;
            BtnRemoveFileExtension.Text = "-";
            BtnRemoveFileExtension.UseVisualStyleBackColor = true;
            BtnRemoveFileExtension.Click += BtnRemoveFileExtension_Click;
            // 
            // BtnAddFileExtension
            // 
            BtnAddFileExtension.Anchor = AnchorStyles.Right;
            BtnAddFileExtension.Location = new Point(404, 86);
            BtnAddFileExtension.Margin = new Padding(4, 3, 4, 3);
            BtnAddFileExtension.Name = "BtnAddFileExtension";
            BtnAddFileExtension.Size = new Size(24, 27);
            BtnAddFileExtension.TabIndex = 5;
            BtnAddFileExtension.Text = "+";
            BtnAddFileExtension.UseVisualStyleBackColor = true;
            BtnAddFileExtension.Click += BtnAddFileExtension_Click;
            // 
            // TxtFileExtension
            // 
            TxtFileExtension.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TxtFileExtension.Location = new Point(109, 88);
            TxtFileExtension.Margin = new Padding(4, 3, 4, 3);
            TxtFileExtension.Name = "TxtFileExtension";
            TxtFileExtension.Size = new Size(287, 23);
            TxtFileExtension.TabIndex = 4;
            // 
            // ChbRecursive
            // 
            ChbRecursive.Anchor = AnchorStyles.Left;
            ChbRecursive.AutoSize = true;
            ChbRecursive.Checked = true;
            ChbRecursive.CheckState = CheckState.Checked;
            ChbRecursive.Location = new Point(109, 36);
            ChbRecursive.Margin = new Padding(4, 3, 4, 3);
            ChbRecursive.Name = "ChbRecursive";
            ChbRecursive.Size = new Size(76, 19);
            ChbRecursive.TabIndex = 2;
            ChbRecursive.Text = "Recursive";
            ChbRecursive.UseVisualStyleBackColor = true;
            // 
            // LsbExcludedDirectories
            // 
            LsbExcludedDirectories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LsbExcludedDirectories.FormattingEnabled = true;
            LsbExcludedDirectories.ItemHeight = 15;
            LsbExcludedDirectories.Location = new Point(109, 311);
            LsbExcludedDirectories.Margin = new Padding(4, 3, 4, 3);
            LsbExcludedDirectories.Name = "LsbExcludedDirectories";
            tableLayoutPanel3.SetRowSpan(LsbExcludedDirectories, 2);
            LsbExcludedDirectories.Size = new Size(287, 199);
            LsbExcludedDirectories.TabIndex = 8;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Location = new Point(15, 92);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(86, 15);
            label10.TabIndex = 20;
            label10.Text = "File Extentions:";
            // 
            // ChbMonitorFileChanges
            // 
            ChbMonitorFileChanges.Anchor = AnchorStyles.Left;
            ChbMonitorFileChanges.AutoSize = true;
            ChbMonitorFileChanges.Checked = true;
            ChbMonitorFileChanges.CheckState = CheckState.Checked;
            ChbMonitorFileChanges.Location = new Point(109, 61);
            ChbMonitorFileChanges.Margin = new Padding(4, 3, 4, 3);
            ChbMonitorFileChanges.Name = "ChbMonitorFileChanges";
            ChbMonitorFileChanges.Size = new Size(135, 19);
            ChbMonitorFileChanges.TabIndex = 3;
            ChbMonitorFileChanges.Text = "Monitor file changes";
            ChbMonitorFileChanges.UseVisualStyleBackColor = true;
            // 
            // VideoInputCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox3);
            Name = "VideoInputCtrl";
            Size = new Size(440, 540);
            groupBox3.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox3;
        private TableLayoutPanel tableLayoutPanel3;
        private Button BtnSelectSourcePath;
        private Label label1;
        private Button BtnRemoveExcludedDirectory;
        private Button BtnAddExcludedDirectory;
        private Label label4;
        private Button BtnRemoveFileExtension;
        private Button BtnAddFileExtension;
        private Label label10;
        public TextBox TxtSourcePath;
        public ListBox LsbFileExtensions;
        public TextBox TxtFileExtension;
        public CheckBox ChbRecursive;
        public ListBox LsbExcludedDirectories;
        public CheckBox ChbMonitorFileChanges;
    }
}
