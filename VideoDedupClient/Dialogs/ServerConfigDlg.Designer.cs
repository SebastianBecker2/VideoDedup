namespace VideoDedupClient.Dialogs
{
    using System.ComponentModel;

    partial class ServerConfigDlg
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
            var resources = new ComponentResourceManager(typeof(ServerConfigDlg));
            BtnCancel = new Button();
            BtnOkay = new Button();
            groupBox1 = new GroupBox();
            tableLayoutPanel4 = new TableLayoutPanel();
            BtnCustomVideoComparison = new Button();
            NumMaxImageComparison = new NumericUpDown();
            label6 = new Label();
            NumMaxDifferentImages = new NumericUpDown();
            label5 = new Label();
            NumMaxDifferentPercentage = new NumericUpDown();
            label3 = new Label();
            RdbDurationDifferenceSeconds = new RadioButton();
            groupBox2 = new GroupBox();
            LblMaxDurationDifferenceUnit = new Label();
            label7 = new Label();
            RdbDurationDifferencePercent = new RadioButton();
            NumMaxDurationDifference = new NumericUpDown();
            groupBox4 = new GroupBox();
            label9 = new Label();
            NumThumbnailViewCount = new NumericUpDown();
            tableLayoutPanel1 = new TableLayoutPanel();
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
            tableLayoutPanel2 = new TableLayoutPanel();
            groupBox5 = new GroupBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            label2 = new Label();
            label8 = new Label();
            label11 = new Label();
            CmbVideoDedupServiceLogLevel = new ComboBox();
            CmbComparisonManagerLogLevel = new ComboBox();
            CmbDedupEngineLogLevel = new ComboBox();
            groupBox1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            ((ISupportInitialize)NumMaxImageComparison).BeginInit();
            ((ISupportInitialize)NumMaxDifferentImages).BeginInit();
            ((ISupportInitialize)NumMaxDifferentPercentage).BeginInit();
            groupBox2.SuspendLayout();
            ((ISupportInitialize)NumMaxDurationDifference).BeginInit();
            groupBox4.SuspendLayout();
            ((ISupportInitialize)NumThumbnailViewCount).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            groupBox3.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            groupBox5.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            SuspendLayout();
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.Location = new Point(684, 534);
            BtnCancel.Margin = new Padding(4, 3, 4, 3);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(88, 27);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "&Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            BtnOkay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnOkay.Location = new Point(589, 534);
            BtnOkay.Margin = new Padding(4, 3, 4, 3);
            BtnOkay.Name = "BtnOkay";
            BtnOkay.Size = new Size(88, 27);
            BtnOkay.TabIndex = 0;
            BtnOkay.Text = "&OK";
            BtnOkay.UseVisualStyleBackColor = true;
            BtnOkay.Click += BtnOkay_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tableLayoutPanel4);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(0);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(353, 174);
            groupBox1.TabIndex = 23;
            groupBox1.TabStop = false;
            groupBox1.Text = "Comparing Images";
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel4.Controls.Add(BtnCustomVideoComparison, 0, 3);
            tableLayoutPanel4.Controls.Add(NumMaxImageComparison, 1, 0);
            tableLayoutPanel4.Controls.Add(label6, 0, 2);
            tableLayoutPanel4.Controls.Add(NumMaxDifferentImages, 1, 1);
            tableLayoutPanel4.Controls.Add(label5, 0, 1);
            tableLayoutPanel4.Controls.Add(NumMaxDifferentPercentage, 1, 2);
            tableLayoutPanel4.Controls.Add(label3, 0, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(4, 19);
            tableLayoutPanel4.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 4;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.Size = new Size(345, 152);
            tableLayoutPanel4.TabIndex = 14;
            // 
            // BtnCustomVideoComparison
            // 
            BtnCustomVideoComparison.Anchor = AnchorStyles.Right;
            tableLayoutPanel4.SetColumnSpan(BtnCustomVideoComparison, 2);
            BtnCustomVideoComparison.Location = new Point(197, 119);
            BtnCustomVideoComparison.Margin = new Padding(4, 3, 4, 3);
            BtnCustomVideoComparison.Name = "BtnCustomVideoComparison";
            BtnCustomVideoComparison.Size = new Size(144, 27);
            BtnCustomVideoComparison.TabIndex = 3;
            BtnCustomVideoComparison.Text = "Try these &settings...";
            BtnCustomVideoComparison.UseVisualStyleBackColor = true;
            BtnCustomVideoComparison.Click += BtnCustomVideoComparison_Click;
            // 
            // NumMaxImageComparison
            // 
            NumMaxImageComparison.Anchor = AnchorStyles.None;
            NumMaxImageComparison.Location = new Point(267, 7);
            NumMaxImageComparison.Margin = new Padding(4, 3, 4, 3);
            NumMaxImageComparison.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumMaxImageComparison.Name = "NumMaxImageComparison";
            NumMaxImageComparison.Size = new Size(74, 23);
            NumMaxImageComparison.TabIndex = 0;
            NumMaxImageComparison.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(67, 87);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(192, 15);
            label6.TabIndex = 13;
            label6.Text = "Accepted percentage of difference:";
            // 
            // NumMaxDifferentImages
            // 
            NumMaxDifferentImages.Anchor = AnchorStyles.None;
            NumMaxDifferentImages.Location = new Point(267, 45);
            NumMaxDifferentImages.Margin = new Padding(4, 3, 4, 3);
            NumMaxDifferentImages.Name = "NumMaxDifferentImages";
            NumMaxDifferentImages.Size = new Size(74, 23);
            NumMaxDifferentImages.TabIndex = 1;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(51, 49);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(208, 15);
            label5.TabIndex = 12;
            label5.Text = "Accepted number of different Images:";
            // 
            // NumMaxDifferentPercentage
            // 
            NumMaxDifferentPercentage.Anchor = AnchorStyles.None;
            NumMaxDifferentPercentage.Location = new Point(267, 83);
            NumMaxDifferentPercentage.Margin = new Padding(4, 3, 4, 3);
            NumMaxDifferentPercentage.Name = "NumMaxDifferentPercentage";
            NumMaxDifferentPercentage.Size = new Size(74, 23);
            NumMaxDifferentPercentage.TabIndex = 2;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(86, 11);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(173, 15);
            label3.TabIndex = 11;
            label3.Text = "Number of Images to compare:";
            // 
            // RdbDurationDifferenceSeconds
            // 
            RdbDurationDifferenceSeconds.Anchor = AnchorStyles.None;
            RdbDurationDifferenceSeconds.AutoSize = true;
            RdbDurationDifferenceSeconds.Location = new Point(18, 28);
            RdbDurationDifferenceSeconds.Margin = new Padding(4, 3, 4, 3);
            RdbDurationDifferenceSeconds.Name = "RdbDurationDifferenceSeconds";
            RdbDurationDifferenceSeconds.Size = new Size(181, 19);
            RdbDurationDifferenceSeconds.TabIndex = 0;
            RdbDurationDifferenceSeconds.TabStop = true;
            RdbDurationDifferenceSeconds.Text = "Absolut difference in seconds";
            RdbDurationDifferenceSeconds.UseVisualStyleBackColor = true;
            RdbDurationDifferenceSeconds.CheckedChanged += HandleDurationDifferenceTypeChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(LblMaxDurationDifferenceUnit);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(RdbDurationDifferencePercent);
            groupBox2.Controls.Add(NumMaxDurationDifference);
            groupBox2.Controls.Add(RdbDurationDifferenceSeconds);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 174);
            groupBox2.Margin = new Padding(0);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(353, 139);
            groupBox2.TabIndex = 24;
            groupBox2.TabStop = false;
            groupBox2.Text = "Comparing Duration";
            // 
            // LblMaxDurationDifferenceUnit
            // 
            LblMaxDurationDifferenceUnit.Anchor = AnchorStyles.None;
            LblMaxDurationDifferenceUnit.AutoSize = true;
            LblMaxDurationDifferenceUnit.Location = new Point(286, 94);
            LblMaxDurationDifferenceUnit.Margin = new Padding(4, 0, 4, 0);
            LblMaxDurationDifferenceUnit.Name = "LblMaxDurationDifferenceUnit";
            LblMaxDurationDifferenceUnit.Size = new Size(51, 15);
            LblMaxDurationDifferenceUnit.TabIndex = 16;
            LblMaxDurationDifferenceUnit.Text = "Seconds";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.None;
            label7.AutoSize = true;
            label7.Location = new Point(75, 94);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(121, 15);
            label7.TabIndex = 15;
            label7.Text = "Maximum difference:";
            // 
            // RdbDurationDifferencePercent
            // 
            RdbDurationDifferencePercent.Anchor = AnchorStyles.None;
            RdbDurationDifferencePercent.AutoSize = true;
            RdbDurationDifferencePercent.Location = new Point(18, 53);
            RdbDurationDifferencePercent.Margin = new Padding(4, 3, 4, 3);
            RdbDurationDifferencePercent.Name = "RdbDurationDifferencePercent";
            RdbDurationDifferencePercent.Size = new Size(178, 19);
            RdbDurationDifferencePercent.TabIndex = 1;
            RdbDurationDifferencePercent.TabStop = true;
            RdbDurationDifferencePercent.Text = "Relative difference in percent";
            RdbDurationDifferencePercent.UseVisualStyleBackColor = true;
            RdbDurationDifferencePercent.CheckedChanged += HandleDurationDifferenceTypeChanged;
            // 
            // NumMaxDurationDifference
            // 
            NumMaxDurationDifference.Anchor = AnchorStyles.None;
            NumMaxDurationDifference.Location = new Point(204, 92);
            NumMaxDurationDifference.Margin = new Padding(4, 3, 4, 3);
            NumMaxDurationDifference.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NumMaxDurationDifference.Name = "NumMaxDurationDifference";
            NumMaxDurationDifference.Size = new Size(74, 23);
            NumMaxDurationDifference.TabIndex = 2;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(label9);
            groupBox4.Controls.Add(NumThumbnailViewCount);
            groupBox4.Dock = DockStyle.Fill;
            groupBox4.Location = new Point(0, 313);
            groupBox4.Margin = new Padding(0);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4, 3, 4, 3);
            groupBox4.Size = new Size(353, 52);
            groupBox4.TabIndex = 26;
            groupBox4.TabStop = false;
            groupBox4.Text = "Resolving Duplicates";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.None;
            label9.AutoSize = true;
            label9.Location = new Point(92, 21);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(163, 15);
            label9.TabIndex = 19;
            label9.Text = "Number of Images to display:";
            // 
            // NumThumbnailViewCount
            // 
            NumThumbnailViewCount.Anchor = AnchorStyles.None;
            NumThumbnailViewCount.Location = new Point(266, 19);
            NumThumbnailViewCount.Margin = new Padding(4, 3, 4, 3);
            NumThumbnailViewCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumThumbnailViewCount.Name = "NumThumbnailViewCount";
            NumThumbnailViewCount.Size = new Size(74, 23);
            NumThumbnailViewCount.TabIndex = 0;
            NumThumbnailViewCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 53.44564F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46.55436F));
            tableLayoutPanel1.Controls.Add(groupBox3, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Location = new Point(14, 14);
            tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(757, 513);
            tableLayoutPanel1.TabIndex = 27;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(tableLayoutPanel3);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(0, 0);
            groupBox3.Margin = new Padding(0);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 3, 4, 3);
            groupBox3.Size = new Size(404, 513);
            groupBox3.TabIndex = 0;
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
            tableLayoutPanel3.Size = new Size(396, 491);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // BtnSelectSourcePath
            // 
            BtnSelectSourcePath.Anchor = AnchorStyles.Right;
            BtnSelectSourcePath.Location = new Point(368, 3);
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
            TxtSourcePath.Size = new Size(251, 23);
            TxtSourcePath.TabIndex = 0;
            // 
            // BtnRemoveExcludedDirectory
            // 
            BtnRemoveExcludedDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnRemoveExcludedDirectory.Location = new Point(368, 330);
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
            BtnAddExcludedDirectory.Location = new Point(368, 297);
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
            label4.Location = new Point(39, 303);
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
            LsbFileExtensions.Size = new Size(251, 169);
            LsbFileExtensions.TabIndex = 6;
            // 
            // BtnRemoveFileExtension
            // 
            BtnRemoveFileExtension.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnRemoveFileExtension.Location = new Point(368, 119);
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
            BtnAddFileExtension.Location = new Point(368, 86);
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
            TxtFileExtension.Size = new Size(251, 23);
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
            LsbExcludedDirectories.Location = new Point(109, 297);
            LsbExcludedDirectories.Margin = new Padding(4, 3, 4, 3);
            LsbExcludedDirectories.Name = "LsbExcludedDirectories";
            tableLayoutPanel3.SetRowSpan(LsbExcludedDirectories, 2);
            LsbExcludedDirectories.Size = new Size(251, 184);
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
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(groupBox1, 0, 0);
            tableLayoutPanel2.Controls.Add(groupBox2, 0, 1);
            tableLayoutPanel2.Controls.Add(groupBox4, 0, 2);
            tableLayoutPanel2.Controls.Add(groupBox5, 0, 3);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(404, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.98869F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 27.1909523F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 10.2016239F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 28.61873F));
            tableLayoutPanel2.Size = new Size(353, 513);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(tableLayoutPanel5);
            groupBox5.Dock = DockStyle.Fill;
            groupBox5.Location = new Point(0, 365);
            groupBox5.Margin = new Padding(0);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(353, 148);
            groupBox5.TabIndex = 27;
            groupBox5.TabStop = false;
            groupBox5.Text = "Minimum Log Level";
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61.53846F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38.46154F));
            tableLayoutPanel5.Controls.Add(label2, 0, 0);
            tableLayoutPanel5.Controls.Add(label8, 0, 1);
            tableLayoutPanel5.Controls.Add(label11, 0, 2);
            tableLayoutPanel5.Controls.Add(CmbVideoDedupServiceLogLevel, 1, 0);
            tableLayoutPanel5.Controls.Add(CmbComparisonManagerLogLevel, 1, 1);
            tableLayoutPanel5.Controls.Add(CmbDedupEngineLogLevel, 1, 2);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 19);
            tableLayoutPanel5.Margin = new Padding(0);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 3;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel5.Size = new Size(347, 126);
            tableLayoutPanel5.TabIndex = 0;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(98, 13);
            label2.Name = "label2";
            label2.Size = new Size(112, 15);
            label2.TabIndex = 0;
            label2.Text = "VideoDedupService:";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(88, 55);
            label8.Name = "label8";
            label8.Size = new Size(122, 15);
            label8.TabIndex = 0;
            label8.Text = "ComparisonManager:";
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Location = new Point(129, 97);
            label11.Name = "label11";
            label11.Size = new Size(81, 15);
            label11.TabIndex = 0;
            label11.Text = "DedupEngine:";
            // 
            // CmbVideoDedupServiceLogLevel
            // 
            CmbVideoDedupServiceLogLevel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CmbVideoDedupServiceLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbVideoDedupServiceLogLevel.FormattingEnabled = true;
            CmbVideoDedupServiceLogLevel.Location = new Point(216, 9);
            CmbVideoDedupServiceLogLevel.Name = "CmbVideoDedupServiceLogLevel";
            CmbVideoDedupServiceLogLevel.Size = new Size(128, 23);
            CmbVideoDedupServiceLogLevel.TabIndex = 1;
            // 
            // CmbComparisonManagerLogLevel
            // 
            CmbComparisonManagerLogLevel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CmbComparisonManagerLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbComparisonManagerLogLevel.FormattingEnabled = true;
            CmbComparisonManagerLogLevel.Location = new Point(216, 51);
            CmbComparisonManagerLogLevel.Name = "CmbComparisonManagerLogLevel";
            CmbComparisonManagerLogLevel.Size = new Size(128, 23);
            CmbComparisonManagerLogLevel.TabIndex = 1;
            // 
            // CmbDedupEngineLogLevel
            // 
            CmbDedupEngineLogLevel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CmbDedupEngineLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbDedupEngineLogLevel.FormattingEnabled = true;
            CmbDedupEngineLogLevel.Location = new Point(216, 93);
            CmbDedupEngineLogLevel.Name = "CmbDedupEngineLogLevel";
            CmbDedupEngineLogLevel.Size = new Size(128, 23);
            CmbDedupEngineLogLevel.TabIndex = 1;
            // 
            // ServerConfigDlg
            // 
            AcceptButton = BtnOkay;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = BtnCancel;
            ClientSize = new Size(785, 575);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOkay);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "ServerConfigDlg";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Server Configuration";
            groupBox1.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            ((ISupportInitialize)NumMaxImageComparison).EndInit();
            ((ISupportInitialize)NumMaxDifferentImages).EndInit();
            ((ISupportInitialize)NumMaxDifferentPercentage).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((ISupportInitialize)NumMaxDurationDifference).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((ISupportInitialize)NumThumbnailViewCount).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button BtnCancel;
        private Button BtnOkay;
        private GroupBox groupBox1;
        private Label label6;
        private Label label5;
        private Label label3;
        private RadioButton RdbDurationDifferenceSeconds;
        private NumericUpDown NumMaxDifferentPercentage;
        private NumericUpDown NumMaxDifferentImages;
        private NumericUpDown NumMaxImageComparison;
        private GroupBox groupBox2;
        private Label label7;
        private RadioButton RdbDurationDifferencePercent;
        private NumericUpDown NumMaxDurationDifference;
        private GroupBox groupBox4;
        private Label label9;
        private NumericUpDown NumThumbnailViewCount;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox groupBox3;
        private Button BtnSelectSourcePath;
        private TextBox TxtSourcePath;
        private Label label1;
        private TextBox TxtFileExtension;
        private ListBox LsbFileExtensions;
        private Button BtnRemoveFileExtension;
        private Button BtnAddFileExtension;
        private Label label4;
        private ListBox LsbExcludedDirectories;
        private Button BtnAddExcludedDirectory;
        private Button BtnRemoveExcludedDirectory;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label10;
        private CheckBox ChbRecursive;
        private CheckBox ChbMonitorFileChanges;
        private Button BtnCustomVideoComparison;
        private TableLayoutPanel tableLayoutPanel4;
        private Label LblMaxDurationDifferenceUnit;
        private GroupBox groupBox5;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label2;
        private Label label8;
        private Label label11;
        private ComboBox CmbVideoDedupServiceLogLevel;
        private ComboBox CmbComparisonManagerLogLevel;
        private ComboBox CmbDedupEngineLogLevel;
    }
}
