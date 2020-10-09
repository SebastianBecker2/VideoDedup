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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Config));
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOkay = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NumMaxDifferentPercentage = new System.Windows.Forms.NumericUpDown();
            this.NumMaxDifferentThumbnails = new System.Windows.Forms.NumericUpDown();
            this.NumMaxThumbnailComparison = new System.Windows.Forms.NumericUpDown();
            this.RdbDurationDifferenceSeconds = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.NumMaxDurationDifferencePercent = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.RdbDurationDifferencePercent = new System.Windows.Forms.RadioButton();
            this.NumMaxDurationDifferenceSeconds = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.NumThumbnailViewCount = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.BtnSelectSourcePath = new System.Windows.Forms.Button();
            this.TxtSourcePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TxtFileExtension = new System.Windows.Forms.TextBox();
            this.LsbFileExtensions = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnRemoveFileExtension = new System.Windows.Forms.Button();
            this.BtnAddFileExtension = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.LsbExcludedDirectories = new System.Windows.Forms.ListBox();
            this.BtnAddExcludedDirectory = new System.Windows.Forms.Button();
            this.BtnRemoveExcludedDirectory = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentThumbnails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxThumbnailComparison)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifferencePercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifferenceSeconds)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumThumbnailViewCount)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(585, 373);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 21;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            this.BtnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOkay.Location = new System.Drawing.Point(504, 373);
            this.BtnOkay.Name = "BtnOkay";
            this.BtnOkay.Size = new System.Drawing.Size(75, 23);
            this.BtnOkay.TabIndex = 20;
            this.BtnOkay.Text = "&OK";
            this.BtnOkay.UseVisualStyleBackColor = true;
            this.BtnOkay.Click += new System.EventHandler(this.BtnOkay_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.NumMaxDifferentPercentage);
            this.groupBox1.Controls.Add(this.NumMaxDifferentThumbnails);
            this.groupBox1.Controls.Add(this.NumMaxThumbnailComparison);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 134);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comparing Thumbnails";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(48, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(175, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Accepted percentage of difference:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(204, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Accepted number of different Thumbnails:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Number of Thumbnails to compare:";
            // 
            // NumMaxDifferentPercentage
            // 
            this.NumMaxDifferentPercentage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDifferentPercentage.Location = new System.Drawing.Point(229, 84);
            this.NumMaxDifferentPercentage.Name = "NumMaxDifferentPercentage";
            this.NumMaxDifferentPercentage.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDifferentPercentage.TabIndex = 2;
            // 
            // NumMaxDifferentThumbnails
            // 
            this.NumMaxDifferentThumbnails.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDifferentThumbnails.Location = new System.Drawing.Point(229, 58);
            this.NumMaxDifferentThumbnails.Name = "NumMaxDifferentThumbnails";
            this.NumMaxDifferentThumbnails.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDifferentThumbnails.TabIndex = 1;
            // 
            // NumMaxThumbnailComparison
            // 
            this.NumMaxThumbnailComparison.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxThumbnailComparison.Location = new System.Drawing.Point(229, 31);
            this.NumMaxThumbnailComparison.Name = "NumMaxThumbnailComparison";
            this.NumMaxThumbnailComparison.Size = new System.Drawing.Size(63, 20);
            this.NumMaxThumbnailComparison.TabIndex = 0;
            // 
            // RdbDurationDifferenceSeconds
            // 
            this.RdbDurationDifferenceSeconds.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RdbDurationDifferenceSeconds.AutoSize = true;
            this.RdbDurationDifferenceSeconds.Location = new System.Drawing.Point(16, 28);
            this.RdbDurationDifferenceSeconds.Name = "RdbDurationDifferenceSeconds";
            this.RdbDurationDifferenceSeconds.Size = new System.Drawing.Size(110, 17);
            this.RdbDurationDifferenceSeconds.TabIndex = 3;
            this.RdbDurationDifferenceSeconds.TabStop = true;
            this.RdbDurationDifferenceSeconds.Text = "Absolut difference";
            this.RdbDurationDifferenceSeconds.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.NumMaxDurationDifferencePercent);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.RdbDurationDifferencePercent);
            this.groupBox2.Controls.Add(this.NumMaxDurationDifferenceSeconds);
            this.groupBox2.Controls.Add(this.RdbDurationDifferenceSeconds);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 134);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 142);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Comparing Duration";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(69, 94);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(154, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Maximum difference in percent:";
            // 
            // NumMaxDurationDifferencePercent
            // 
            this.NumMaxDurationDifferencePercent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDurationDifferencePercent.Location = new System.Drawing.Point(229, 92);
            this.NumMaxDurationDifferencePercent.Name = "NumMaxDurationDifferencePercent";
            this.NumMaxDurationDifferencePercent.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDurationDifferencePercent.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(65, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(158, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Maximum difference in seconds:";
            // 
            // RdbDurationDifferencePercent
            // 
            this.RdbDurationDifferencePercent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RdbDurationDifferencePercent.AutoSize = true;
            this.RdbDurationDifferencePercent.Location = new System.Drawing.Point(16, 72);
            this.RdbDurationDifferencePercent.Name = "RdbDurationDifferencePercent";
            this.RdbDurationDifferencePercent.Size = new System.Drawing.Size(114, 17);
            this.RdbDurationDifferencePercent.TabIndex = 4;
            this.RdbDurationDifferencePercent.TabStop = true;
            this.RdbDurationDifferencePercent.Text = "Relative difference";
            this.RdbDurationDifferencePercent.UseVisualStyleBackColor = true;
            // 
            // NumMaxDurationDifferenceSeconds
            // 
            this.NumMaxDurationDifferenceSeconds.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDurationDifferenceSeconds.Location = new System.Drawing.Point(229, 52);
            this.NumMaxDurationDifferenceSeconds.Name = "NumMaxDurationDifferenceSeconds";
            this.NumMaxDurationDifferenceSeconds.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDurationDifferenceSeconds.TabIndex = 14;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.NumThumbnailViewCount);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 276);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(302, 79);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Resolving Duplicates";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(60, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(163, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Number of Thumbnails to display:";
            // 
            // NumThumbnailViewCount
            // 
            this.NumThumbnailViewCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumThumbnailViewCount.Location = new System.Drawing.Point(229, 29);
            this.NumThumbnailViewCount.Name = "NumThumbnailViewCount";
            this.NumThumbnailViewCount.Size = new System.Drawing.Size(63, 20);
            this.NumThumbnailViewCount.TabIndex = 18;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.44564F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.55436F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(648, 355);
            this.tableLayoutPanel1.TabIndex = 27;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.BtnSelectSourcePath);
            this.groupBox3.Controls.Add(this.TxtSourcePath);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.splitContainer1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(346, 355);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Video Input";
            // 
            // BtnSelectSourcePath
            // 
            this.BtnSelectSourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSelectSourcePath.Location = new System.Drawing.Point(319, 19);
            this.BtnSelectSourcePath.Name = "BtnSelectSourcePath";
            this.BtnSelectSourcePath.Size = new System.Drawing.Size(21, 23);
            this.BtnSelectSourcePath.TabIndex = 19;
            this.BtnSelectSourcePath.Text = "...";
            this.BtnSelectSourcePath.UseVisualStyleBackColor = true;
            // 
            // TxtSourcePath
            // 
            this.TxtSourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSourcePath.Location = new System.Drawing.Point(95, 21);
            this.TxtSourcePath.Name = "TxtSourcePath";
            this.TxtSourcePath.Size = new System.Drawing.Size(218, 20);
            this.TxtSourcePath.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Source Path:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(6, 47);
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
            this.splitContainer1.Size = new System.Drawing.Size(334, 302);
            this.splitContainer1.SplitterDistance = 173;
            this.splitContainer1.TabIndex = 22;
            // 
            // TxtFileExtension
            // 
            this.TxtFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtFileExtension.Location = new System.Drawing.Point(87, 3);
            this.TxtFileExtension.Name = "TxtFileExtension";
            this.TxtFileExtension.Size = new System.Drawing.Size(220, 20);
            this.TxtFileExtension.TabIndex = 10;
            // 
            // LsbFileExtensions
            // 
            this.LsbFileExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LsbFileExtensions.FormattingEnabled = true;
            this.LsbFileExtensions.Location = new System.Drawing.Point(87, 29);
            this.LsbFileExtensions.Name = "LsbFileExtensions";
            this.LsbFileExtensions.Size = new System.Drawing.Size(220, 134);
            this.LsbFileExtensions.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "File Extentions:";
            // 
            // BtnRemoveFileExtension
            // 
            this.BtnRemoveFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveFileExtension.Location = new System.Drawing.Point(313, 32);
            this.BtnRemoveFileExtension.Name = "BtnRemoveFileExtension";
            this.BtnRemoveFileExtension.Size = new System.Drawing.Size(21, 23);
            this.BtnRemoveFileExtension.TabIndex = 9;
            this.BtnRemoveFileExtension.Text = "-";
            this.BtnRemoveFileExtension.UseVisualStyleBackColor = true;
            // 
            // BtnAddFileExtension
            // 
            this.BtnAddFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAddFileExtension.Location = new System.Drawing.Point(313, 3);
            this.BtnAddFileExtension.Name = "BtnAddFileExtension";
            this.BtnAddFileExtension.Size = new System.Drawing.Size(21, 23);
            this.BtnAddFileExtension.TabIndex = 8;
            this.BtnAddFileExtension.Text = "+";
            this.BtnAddFileExtension.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 3);
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
            this.LsbExcludedDirectories.Location = new System.Drawing.Point(87, 3);
            this.LsbExcludedDirectories.Name = "LsbExcludedDirectories";
            this.LsbExcludedDirectories.Size = new System.Drawing.Size(220, 108);
            this.LsbExcludedDirectories.TabIndex = 12;
            // 
            // BtnAddExcludedDirectory
            // 
            this.BtnAddExcludedDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAddExcludedDirectory.Location = new System.Drawing.Point(313, 3);
            this.BtnAddExcludedDirectory.Name = "BtnAddExcludedDirectory";
            this.BtnAddExcludedDirectory.Size = new System.Drawing.Size(21, 23);
            this.BtnAddExcludedDirectory.TabIndex = 13;
            this.BtnAddExcludedDirectory.Text = "+";
            this.BtnAddExcludedDirectory.UseVisualStyleBackColor = true;
            // 
            // BtnRemoveExcludedDirectory
            // 
            this.BtnRemoveExcludedDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveExcludedDirectory.Location = new System.Drawing.Point(313, 32);
            this.BtnRemoveExcludedDirectory.Name = "BtnRemoveExcludedDirectory";
            this.BtnRemoveExcludedDirectory.Size = new System.Drawing.Size(21, 23);
            this.BtnRemoveExcludedDirectory.TabIndex = 14;
            this.BtnRemoveExcludedDirectory.Text = "-";
            this.BtnRemoveExcludedDirectory.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.groupBox4, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(346, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.48485F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.51515F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(302, 355);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // Config
            // 
            this.AcceptButton = this.BtnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(672, 408);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Config";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentThumbnails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxThumbnailComparison)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifferencePercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifferenceSeconds)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumThumbnailViewCount)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnOkay;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton RdbDurationDifferenceSeconds;
        private System.Windows.Forms.NumericUpDown NumMaxDifferentPercentage;
        private System.Windows.Forms.NumericUpDown NumMaxDifferentThumbnails;
        private System.Windows.Forms.NumericUpDown NumMaxThumbnailComparison;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown NumMaxDurationDifferencePercent;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton RdbDurationDifferencePercent;
        private System.Windows.Forms.NumericUpDown NumMaxDurationDifferenceSeconds;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown NumThumbnailViewCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BtnSelectSourcePath;
        private System.Windows.Forms.TextBox TxtSourcePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox TxtFileExtension;
        private System.Windows.Forms.ListBox LsbFileExtensions;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnRemoveFileExtension;
        private System.Windows.Forms.Button BtnAddFileExtension;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox LsbExcludedDirectories;
        private System.Windows.Forms.Button BtnAddExcludedDirectory;
        private System.Windows.Forms.Button BtnRemoveExcludedDirectory;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}