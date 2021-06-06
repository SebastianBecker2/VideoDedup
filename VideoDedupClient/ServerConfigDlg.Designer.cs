namespace VideoDedup
{
    partial class ServerConfigDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerConfigDlg));
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOkay = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnVideoComparisonPreview = new System.Windows.Forms.Button();
            this.NumMaxImageComparison = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.NumMaxDifferentImages = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.NumMaxDifferentPercentage = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
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
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnSelectSourcePath = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtSourcePath = new System.Windows.Forms.TextBox();
            this.BtnRemoveExcludedDirectory = new System.Windows.Forms.Button();
            this.BtnAddExcludedDirectory = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.LsbFileExtensions = new System.Windows.Forms.ListBox();
            this.BtnRemoveFileExtension = new System.Windows.Forms.Button();
            this.BtnAddFileExtension = new System.Windows.Forms.Button();
            this.TxtFileExtension = new System.Windows.Forms.TextBox();
            this.ChbRecursive = new System.Windows.Forms.CheckBox();
            this.LsbExcludedDirectories = new System.Windows.Forms.ListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.ChbMonitorFileChanges = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.NumSaveStateIntervalMinutes = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxImageComparison)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentImages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifferencePercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifferenceSeconds)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumThumbnailViewCount)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumSaveStateIntervalMinutes)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(586, 463);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 1;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            this.BtnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOkay.Location = new System.Drawing.Point(505, 463);
            this.BtnOkay.Name = "BtnOkay";
            this.BtnOkay.Size = new System.Drawing.Size(75, 23);
            this.BtnOkay.TabIndex = 0;
            this.BtnOkay.Text = "&OK";
            this.BtnOkay.UseVisualStyleBackColor = true;
            this.BtnOkay.Click += new System.EventHandler(this.BtnOkay_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(303, 152);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comparing Images";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.BtnVideoComparisonPreview, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.NumMaxImageComparison, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.NumMaxDifferentImages, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.NumMaxDifferentPercentage, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(297, 133);
            this.tableLayoutPanel4.TabIndex = 14;
            // 
            // BtnVideoComparisonPreview
            // 
            this.BtnVideoComparisonPreview.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tableLayoutPanel4.SetColumnSpan(this.BtnVideoComparisonPreview, 2);
            this.BtnVideoComparisonPreview.Location = new System.Drawing.Point(171, 104);
            this.BtnVideoComparisonPreview.Name = "BtnVideoComparisonPreview";
            this.BtnVideoComparisonPreview.Size = new System.Drawing.Size(123, 23);
            this.BtnVideoComparisonPreview.TabIndex = 3;
            this.BtnVideoComparisonPreview.Text = "Try these &settings...";
            this.BtnVideoComparisonPreview.UseVisualStyleBackColor = true;
            this.BtnVideoComparisonPreview.Click += new System.EventHandler(this.BtnVideoComparisonPreview_Click);
            // 
            // NumMaxImageComparison
            // 
            this.NumMaxImageComparison.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxImageComparison.Location = new System.Drawing.Point(231, 6);
            this.NumMaxImageComparison.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumMaxImageComparison.Name = "NumMaxImageComparison";
            this.NumMaxImageComparison.Size = new System.Drawing.Size(63, 20);
            this.NumMaxImageComparison.TabIndex = 0;
            this.NumMaxImageComparison.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(50, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(175, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Accepted percentage of difference:";
            // 
            // NumMaxDifferentImages
            // 
            this.NumMaxDifferentImages.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDifferentImages.Location = new System.Drawing.Point(231, 39);
            this.NumMaxDifferentImages.Name = "NumMaxDifferentImages";
            this.NumMaxDifferentImages.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDifferentImages.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(184, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Accepted number of different Images:";
            // 
            // NumMaxDifferentPercentage
            // 
            this.NumMaxDifferentPercentage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDifferentPercentage.Location = new System.Drawing.Point(231, 72);
            this.NumMaxDifferentPercentage.Name = "NumMaxDifferentPercentage";
            this.NumMaxDifferentPercentage.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDifferentPercentage.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(73, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Number of Images to compare:";
            // 
            // RdbDurationDifferenceSeconds
            // 
            this.RdbDurationDifferenceSeconds.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RdbDurationDifferenceSeconds.AutoSize = true;
            this.RdbDurationDifferenceSeconds.Location = new System.Drawing.Point(16, 38);
            this.RdbDurationDifferenceSeconds.Name = "RdbDurationDifferenceSeconds";
            this.RdbDurationDifferenceSeconds.Size = new System.Drawing.Size(110, 17);
            this.RdbDurationDifferenceSeconds.TabIndex = 0;
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
            this.groupBox2.Location = new System.Drawing.Point(0, 152);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(303, 162);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Comparing Duration";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(69, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(154, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Maximum difference in percent:";
            // 
            // NumMaxDurationDifferencePercent
            // 
            this.NumMaxDurationDifferencePercent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDurationDifferencePercent.Location = new System.Drawing.Point(229, 102);
            this.NumMaxDurationDifferencePercent.Name = "NumMaxDurationDifferencePercent";
            this.NumMaxDurationDifferencePercent.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDurationDifferencePercent.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(65, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(158, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Maximum difference in seconds:";
            // 
            // RdbDurationDifferencePercent
            // 
            this.RdbDurationDifferencePercent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RdbDurationDifferencePercent.AutoSize = true;
            this.RdbDurationDifferencePercent.Location = new System.Drawing.Point(16, 82);
            this.RdbDurationDifferencePercent.Name = "RdbDurationDifferencePercent";
            this.RdbDurationDifferencePercent.Size = new System.Drawing.Size(114, 17);
            this.RdbDurationDifferencePercent.TabIndex = 1;
            this.RdbDurationDifferencePercent.TabStop = true;
            this.RdbDurationDifferencePercent.Text = "Relative difference";
            this.RdbDurationDifferencePercent.UseVisualStyleBackColor = true;
            // 
            // NumMaxDurationDifferenceSeconds
            // 
            this.NumMaxDurationDifferenceSeconds.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDurationDifferenceSeconds.Location = new System.Drawing.Point(229, 62);
            this.NumMaxDurationDifferenceSeconds.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.NumMaxDurationDifferenceSeconds.Name = "NumMaxDurationDifferenceSeconds";
            this.NumMaxDurationDifferenceSeconds.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDurationDifferenceSeconds.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.NumThumbnailViewCount);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 314);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(303, 65);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Resolving Duplicates";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(80, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Number of Images to display:";
            // 
            // NumThumbnailViewCount
            // 
            this.NumThumbnailViewCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumThumbnailViewCount.Location = new System.Drawing.Point(229, 24);
            this.NumThumbnailViewCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumThumbnailViewCount.Name = "NumThumbnailViewCount";
            this.NumThumbnailViewCount.Size = new System.Drawing.Size(63, 20);
            this.NumThumbnailViewCount.TabIndex = 0;
            this.NumThumbnailViewCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(649, 445);
            this.tableLayoutPanel1.TabIndex = 27;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(346, 445);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Video Input";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.BtnSelectSourcePath, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.TxtSourcePath, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.BtnRemoveExcludedDirectory, 2, 6);
            this.tableLayoutPanel3.Controls.Add(this.BtnAddExcludedDirectory, 2, 5);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.LsbFileExtensions, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.BtnRemoveFileExtension, 2, 4);
            this.tableLayoutPanel3.Controls.Add(this.BtnAddFileExtension, 2, 3);
            this.tableLayoutPanel3.Controls.Add(this.TxtFileExtension, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.ChbRecursive, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.LsbExcludedDirectories, 1, 5);
            this.tableLayoutPanel3.Controls.Add(this.label10, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.ChbMonitorFileChanges, 1, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.21519F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.78481F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(340, 426);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // BtnSelectSourcePath
            // 
            this.BtnSelectSourcePath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnSelectSourcePath.Location = new System.Drawing.Point(316, 3);
            this.BtnSelectSourcePath.Name = "BtnSelectSourcePath";
            this.BtnSelectSourcePath.Size = new System.Drawing.Size(21, 23);
            this.BtnSelectSourcePath.TabIndex = 1;
            this.BtnSelectSourcePath.Text = "...";
            this.BtnSelectSourcePath.UseVisualStyleBackColor = true;
            this.BtnSelectSourcePath.Click += new System.EventHandler(this.BtnSelectSourcePath_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Source Directory:";
            // 
            // TxtSourcePath
            // 
            this.TxtSourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSourcePath.Location = new System.Drawing.Point(98, 4);
            this.TxtSourcePath.Name = "TxtSourcePath";
            this.TxtSourcePath.Size = new System.Drawing.Size(212, 20);
            this.TxtSourcePath.TabIndex = 0;
            // 
            // BtnRemoveExcludedDirectory
            // 
            this.BtnRemoveExcludedDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveExcludedDirectory.Location = new System.Drawing.Point(316, 288);
            this.BtnRemoveExcludedDirectory.Name = "BtnRemoveExcludedDirectory";
            this.BtnRemoveExcludedDirectory.Size = new System.Drawing.Size(21, 23);
            this.BtnRemoveExcludedDirectory.TabIndex = 10;
            this.BtnRemoveExcludedDirectory.Text = "-";
            this.BtnRemoveExcludedDirectory.UseVisualStyleBackColor = true;
            this.BtnRemoveExcludedDirectory.Click += new System.EventHandler(this.BtnRemoveExcludedDirectory_Click);
            // 
            // BtnAddExcludedDirectory
            // 
            this.BtnAddExcludedDirectory.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnAddExcludedDirectory.Location = new System.Drawing.Point(316, 259);
            this.BtnAddExcludedDirectory.Name = "BtnAddExcludedDirectory";
            this.BtnAddExcludedDirectory.Size = new System.Drawing.Size(21, 23);
            this.BtnAddExcludedDirectory.TabIndex = 9;
            this.BtnAddExcludedDirectory.Text = "+";
            this.BtnAddExcludedDirectory.UseVisualStyleBackColor = true;
            this.BtnAddExcludedDirectory.Click += new System.EventHandler(this.BtnAddExcludedDirectory_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 264);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Excluding:";
            // 
            // LsbFileExtensions
            // 
            this.LsbFileExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LsbFileExtensions.FormattingEnabled = true;
            this.LsbFileExtensions.Location = new System.Drawing.Point(98, 107);
            this.LsbFileExtensions.Name = "LsbFileExtensions";
            this.LsbFileExtensions.Size = new System.Drawing.Size(212, 134);
            this.LsbFileExtensions.TabIndex = 6;
            // 
            // BtnRemoveFileExtension
            // 
            this.BtnRemoveFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveFileExtension.Location = new System.Drawing.Point(316, 107);
            this.BtnRemoveFileExtension.Name = "BtnRemoveFileExtension";
            this.BtnRemoveFileExtension.Size = new System.Drawing.Size(21, 23);
            this.BtnRemoveFileExtension.TabIndex = 7;
            this.BtnRemoveFileExtension.Text = "-";
            this.BtnRemoveFileExtension.UseVisualStyleBackColor = true;
            this.BtnRemoveFileExtension.Click += new System.EventHandler(this.BtnRemoveFileExtension_Click);
            // 
            // BtnAddFileExtension
            // 
            this.BtnAddFileExtension.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnAddFileExtension.Location = new System.Drawing.Point(316, 78);
            this.BtnAddFileExtension.Name = "BtnAddFileExtension";
            this.BtnAddFileExtension.Size = new System.Drawing.Size(21, 23);
            this.BtnAddFileExtension.TabIndex = 5;
            this.BtnAddFileExtension.Text = "+";
            this.BtnAddFileExtension.UseVisualStyleBackColor = true;
            this.BtnAddFileExtension.Click += new System.EventHandler(this.BtnAddFileExtension_Click);
            // 
            // TxtFileExtension
            // 
            this.TxtFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtFileExtension.Location = new System.Drawing.Point(98, 79);
            this.TxtFileExtension.Name = "TxtFileExtension";
            this.TxtFileExtension.Size = new System.Drawing.Size(212, 20);
            this.TxtFileExtension.TabIndex = 4;
            // 
            // ChbRecursive
            // 
            this.ChbRecursive.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChbRecursive.AutoSize = true;
            this.ChbRecursive.Checked = true;
            this.ChbRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChbRecursive.Location = new System.Drawing.Point(98, 32);
            this.ChbRecursive.Name = "ChbRecursive";
            this.ChbRecursive.Size = new System.Drawing.Size(74, 17);
            this.ChbRecursive.TabIndex = 2;
            this.ChbRecursive.Text = "Recursive";
            this.ChbRecursive.UseVisualStyleBackColor = true;
            // 
            // LsbExcludedDirectories
            // 
            this.LsbExcludedDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LsbExcludedDirectories.FormattingEnabled = true;
            this.LsbExcludedDirectories.Location = new System.Drawing.Point(98, 259);
            this.LsbExcludedDirectories.Name = "LsbExcludedDirectories";
            this.tableLayoutPanel3.SetRowSpan(this.LsbExcludedDirectories, 2);
            this.LsbExcludedDirectories.Size = new System.Drawing.Size(212, 160);
            this.LsbExcludedDirectories.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 83);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(78, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "File Extentions:";
            // 
            // ChbMonitorFileChanges
            // 
            this.ChbMonitorFileChanges.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChbMonitorFileChanges.AutoSize = true;
            this.ChbMonitorFileChanges.Checked = true;
            this.ChbMonitorFileChanges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChbMonitorFileChanges.Location = new System.Drawing.Point(98, 55);
            this.ChbMonitorFileChanges.Name = "ChbMonitorFileChanges";
            this.ChbMonitorFileChanges.Size = new System.Drawing.Size(121, 17);
            this.ChbMonitorFileChanges.TabIndex = 3;
            this.ChbMonitorFileChanges.Text = "Monitor file changes";
            this.ChbMonitorFileChanges.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.groupBox4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.groupBox5, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(346, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.5133F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.4867F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(303, 445);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.NumSaveStateIntervalMinutes);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(0, 379);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(303, 66);
            this.groupBox5.TabIndex = 27;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Advanced Settings";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Save Engine State interval in minutes:";
            // 
            // NumSaveStateIntervalMinutes
            // 
            this.NumSaveStateIntervalMinutes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumSaveStateIntervalMinutes.Location = new System.Drawing.Point(229, 27);
            this.NumSaveStateIntervalMinutes.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.NumSaveStateIntervalMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumSaveStateIntervalMinutes.Name = "NumSaveStateIntervalMinutes";
            this.NumSaveStateIntervalMinutes.Size = new System.Drawing.Size(63, 20);
            this.NumSaveStateIntervalMinutes.TabIndex = 20;
            this.NumSaveStateIntervalMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ServerConfigDlg
            // 
            this.AcceptButton = this.BtnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(673, 498);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerConfigDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server Configuration";
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxImageComparison)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentImages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifferencePercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifferenceSeconds)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumThumbnailViewCount)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumSaveStateIntervalMinutes)).EndInit();
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
        private System.Windows.Forms.NumericUpDown NumMaxDifferentImages;
        private System.Windows.Forms.NumericUpDown NumMaxImageComparison;
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
        private System.Windows.Forms.TextBox TxtFileExtension;
        private System.Windows.Forms.ListBox LsbFileExtensions;
        private System.Windows.Forms.Button BtnRemoveFileExtension;
        private System.Windows.Forms.Button BtnAddFileExtension;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox LsbExcludedDirectories;
        private System.Windows.Forms.Button BtnAddExcludedDirectory;
        private System.Windows.Forms.Button BtnRemoveExcludedDirectory;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox ChbRecursive;
        private System.Windows.Forms.CheckBox ChbMonitorFileChanges;
        private System.Windows.Forms.Button BtnVideoComparisonPreview;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NumSaveStateIntervalMinutes;
    }
}