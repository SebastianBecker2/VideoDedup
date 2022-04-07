namespace VideoDedupClient.Dialogs
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
            this.BtnCustomVideoComparison = new System.Windows.Forms.Button();
            this.NumMaxImageComparison = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.NumMaxDifferentImages = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.NumMaxDifferentPercentage = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.RdbDurationDifferenceSeconds = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.LblMaxDurationDifferenceUnit = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.RdbDurationDifferencePercent = new System.Windows.Forms.RadioButton();
            this.NumMaxDurationDifference = new System.Windows.Forms.NumericUpDown();
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
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxImageComparison)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentImages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifference)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumThumbnailViewCount)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(684, 534);
            this.BtnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(88, 27);
            this.BtnCancel.TabIndex = 1;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            this.BtnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOkay.Location = new System.Drawing.Point(589, 534);
            this.BtnOkay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnOkay.Name = "BtnOkay";
            this.BtnOkay.Size = new System.Drawing.Size(88, 27);
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
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(353, 237);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comparing Images";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.BtnCustomVideoComparison, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.NumMaxImageComparison, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.NumMaxDifferentImages, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.NumMaxDifferentPercentage, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(4, 19);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(345, 215);
            this.tableLayoutPanel4.TabIndex = 14;
            // 
            // BtnCustomVideoComparison
            // 
            this.BtnCustomVideoComparison.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tableLayoutPanel4.SetColumnSpan(this.BtnCustomVideoComparison, 2);
            this.BtnCustomVideoComparison.Location = new System.Drawing.Point(197, 173);
            this.BtnCustomVideoComparison.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnCustomVideoComparison.Name = "BtnCustomVideoComparison";
            this.BtnCustomVideoComparison.Size = new System.Drawing.Size(144, 27);
            this.BtnCustomVideoComparison.TabIndex = 3;
            this.BtnCustomVideoComparison.Text = "Try these &settings...";
            this.BtnCustomVideoComparison.UseVisualStyleBackColor = true;
            this.BtnCustomVideoComparison.Click += new System.EventHandler(this.BtnCustomVideoComparison_Click);
            // 
            // NumMaxImageComparison
            // 
            this.NumMaxImageComparison.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxImageComparison.Location = new System.Drawing.Point(267, 15);
            this.NumMaxImageComparison.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NumMaxImageComparison.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumMaxImageComparison.Name = "NumMaxImageComparison";
            this.NumMaxImageComparison.Size = new System.Drawing.Size(74, 23);
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
            this.label6.Location = new System.Drawing.Point(67, 125);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(192, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Accepted percentage of difference:";
            // 
            // NumMaxDifferentImages
            // 
            this.NumMaxDifferentImages.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDifferentImages.Location = new System.Drawing.Point(267, 68);
            this.NumMaxDifferentImages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NumMaxDifferentImages.Name = "NumMaxDifferentImages";
            this.NumMaxDifferentImages.Size = new System.Drawing.Size(74, 23);
            this.NumMaxDifferentImages.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 72);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(208, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Accepted number of different Images:";
            // 
            // NumMaxDifferentPercentage
            // 
            this.NumMaxDifferentPercentage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDifferentPercentage.Location = new System.Drawing.Point(267, 121);
            this.NumMaxDifferentPercentage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NumMaxDifferentPercentage.Name = "NumMaxDifferentPercentage";
            this.NumMaxDifferentPercentage.Size = new System.Drawing.Size(74, 23);
            this.NumMaxDifferentPercentage.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(86, 19);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Number of Images to compare:";
            // 
            // RdbDurationDifferenceSeconds
            // 
            this.RdbDurationDifferenceSeconds.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RdbDurationDifferenceSeconds.AutoSize = true;
            this.RdbDurationDifferenceSeconds.Location = new System.Drawing.Point(18, 40);
            this.RdbDurationDifferenceSeconds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.RdbDurationDifferenceSeconds.Name = "RdbDurationDifferenceSeconds";
            this.RdbDurationDifferenceSeconds.Size = new System.Drawing.Size(181, 19);
            this.RdbDurationDifferenceSeconds.TabIndex = 0;
            this.RdbDurationDifferenceSeconds.TabStop = true;
            this.RdbDurationDifferenceSeconds.Text = "Absolut difference in seconds";
            this.RdbDurationDifferenceSeconds.UseVisualStyleBackColor = true;
            this.RdbDurationDifferenceSeconds.CheckedChanged += new System.EventHandler(this.HandleDurationDifferenceTypeChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LblMaxDurationDifferenceUnit);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.RdbDurationDifferencePercent);
            this.groupBox2.Controls.Add(this.NumMaxDurationDifference);
            this.groupBox2.Controls.Add(this.RdbDurationDifferenceSeconds);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 237);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(353, 178);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Comparing Duration";
            // 
            // LblMaxDurationDifferenceUnit
            // 
            this.LblMaxDurationDifferenceUnit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblMaxDurationDifferenceUnit.AutoSize = true;
            this.LblMaxDurationDifferenceUnit.Location = new System.Drawing.Point(286, 106);
            this.LblMaxDurationDifferenceUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblMaxDurationDifferenceUnit.Name = "LblMaxDurationDifferenceUnit";
            this.LblMaxDurationDifferenceUnit.Size = new System.Drawing.Size(51, 15);
            this.LblMaxDurationDifferenceUnit.TabIndex = 16;
            this.LblMaxDurationDifferenceUnit.Text = "Seconds";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(75, 106);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 15);
            this.label7.TabIndex = 15;
            this.label7.Text = "Maximum difference:";
            // 
            // RdbDurationDifferencePercent
            // 
            this.RdbDurationDifferencePercent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RdbDurationDifferencePercent.AutoSize = true;
            this.RdbDurationDifferencePercent.Location = new System.Drawing.Point(18, 65);
            this.RdbDurationDifferencePercent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.RdbDurationDifferencePercent.Name = "RdbDurationDifferencePercent";
            this.RdbDurationDifferencePercent.Size = new System.Drawing.Size(178, 19);
            this.RdbDurationDifferencePercent.TabIndex = 1;
            this.RdbDurationDifferencePercent.TabStop = true;
            this.RdbDurationDifferencePercent.Text = "Relative difference in percent";
            this.RdbDurationDifferencePercent.UseVisualStyleBackColor = true;
            this.RdbDurationDifferencePercent.CheckedChanged += new System.EventHandler(this.HandleDurationDifferenceTypeChanged);
            // 
            // NumMaxDurationDifference
            // 
            this.NumMaxDurationDifference.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumMaxDurationDifference.Location = new System.Drawing.Point(204, 104);
            this.NumMaxDurationDifference.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NumMaxDurationDifference.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.NumMaxDurationDifference.Name = "NumMaxDurationDifference";
            this.NumMaxDurationDifference.Size = new System.Drawing.Size(74, 23);
            this.NumMaxDurationDifference.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.NumThumbnailViewCount);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 415);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox4.Size = new System.Drawing.Size(353, 98);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Resolving Duplicates";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(92, 41);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(163, 15);
            this.label9.TabIndex = 19;
            this.label9.Text = "Number of Images to display:";
            // 
            // NumThumbnailViewCount
            // 
            this.NumThumbnailViewCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumThumbnailViewCount.Location = new System.Drawing.Point(266, 39);
            this.NumThumbnailViewCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NumThumbnailViewCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumThumbnailViewCount.Name = "NumThumbnailViewCount";
            this.NumThumbnailViewCount.Size = new System.Drawing.Size(74, 23);
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(14, 14);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(757, 513);
            this.tableLayoutPanel1.TabIndex = 27;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Size = new System.Drawing.Size(404, 513);
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
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 19);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.21519F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.78481F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(396, 491);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // BtnSelectSourcePath
            // 
            this.BtnSelectSourcePath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnSelectSourcePath.Location = new System.Drawing.Point(368, 3);
            this.BtnSelectSourcePath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnSelectSourcePath.Name = "BtnSelectSourcePath";
            this.BtnSelectSourcePath.Size = new System.Drawing.Size(24, 27);
            this.BtnSelectSourcePath.TabIndex = 1;
            this.BtnSelectSourcePath.Text = "...";
            this.BtnSelectSourcePath.UseVisualStyleBackColor = true;
            this.BtnSelectSourcePath.Click += new System.EventHandler(this.BtnSelectSourcePath_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 18;
            this.label1.Text = "Source Directory:";
            // 
            // TxtSourcePath
            // 
            this.TxtSourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSourcePath.Location = new System.Drawing.Point(109, 5);
            this.TxtSourcePath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtSourcePath.Name = "TxtSourcePath";
            this.TxtSourcePath.Size = new System.Drawing.Size(251, 23);
            this.TxtSourcePath.TabIndex = 0;
            // 
            // BtnRemoveExcludedDirectory
            // 
            this.BtnRemoveExcludedDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveExcludedDirectory.Location = new System.Drawing.Point(368, 330);
            this.BtnRemoveExcludedDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnRemoveExcludedDirectory.Name = "BtnRemoveExcludedDirectory";
            this.BtnRemoveExcludedDirectory.Size = new System.Drawing.Size(24, 27);
            this.BtnRemoveExcludedDirectory.TabIndex = 10;
            this.BtnRemoveExcludedDirectory.Text = "-";
            this.BtnRemoveExcludedDirectory.UseVisualStyleBackColor = true;
            this.BtnRemoveExcludedDirectory.Click += new System.EventHandler(this.BtnRemoveExcludedDirectory_Click);
            // 
            // BtnAddExcludedDirectory
            // 
            this.BtnAddExcludedDirectory.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnAddExcludedDirectory.Location = new System.Drawing.Point(368, 297);
            this.BtnAddExcludedDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnAddExcludedDirectory.Name = "BtnAddExcludedDirectory";
            this.BtnAddExcludedDirectory.Size = new System.Drawing.Size(24, 27);
            this.BtnAddExcludedDirectory.TabIndex = 9;
            this.BtnAddExcludedDirectory.Text = "+";
            this.BtnAddExcludedDirectory.UseVisualStyleBackColor = true;
            this.BtnAddExcludedDirectory.Click += new System.EventHandler(this.BtnAddExcludedDirectory_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 303);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 15);
            this.label4.TabIndex = 15;
            this.label4.Text = "Excluding:";
            // 
            // LsbFileExtensions
            // 
            this.LsbFileExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LsbFileExtensions.FormattingEnabled = true;
            this.LsbFileExtensions.ItemHeight = 15;
            this.LsbFileExtensions.Location = new System.Drawing.Point(109, 119);
            this.LsbFileExtensions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LsbFileExtensions.Name = "LsbFileExtensions";
            this.LsbFileExtensions.Size = new System.Drawing.Size(251, 169);
            this.LsbFileExtensions.TabIndex = 6;
            // 
            // BtnRemoveFileExtension
            // 
            this.BtnRemoveFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveFileExtension.Location = new System.Drawing.Point(368, 119);
            this.BtnRemoveFileExtension.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnRemoveFileExtension.Name = "BtnRemoveFileExtension";
            this.BtnRemoveFileExtension.Size = new System.Drawing.Size(24, 27);
            this.BtnRemoveFileExtension.TabIndex = 7;
            this.BtnRemoveFileExtension.Text = "-";
            this.BtnRemoveFileExtension.UseVisualStyleBackColor = true;
            this.BtnRemoveFileExtension.Click += new System.EventHandler(this.BtnRemoveFileExtension_Click);
            // 
            // BtnAddFileExtension
            // 
            this.BtnAddFileExtension.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnAddFileExtension.Location = new System.Drawing.Point(368, 86);
            this.BtnAddFileExtension.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnAddFileExtension.Name = "BtnAddFileExtension";
            this.BtnAddFileExtension.Size = new System.Drawing.Size(24, 27);
            this.BtnAddFileExtension.TabIndex = 5;
            this.BtnAddFileExtension.Text = "+";
            this.BtnAddFileExtension.UseVisualStyleBackColor = true;
            this.BtnAddFileExtension.Click += new System.EventHandler(this.BtnAddFileExtension_Click);
            // 
            // TxtFileExtension
            // 
            this.TxtFileExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtFileExtension.Location = new System.Drawing.Point(109, 88);
            this.TxtFileExtension.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtFileExtension.Name = "TxtFileExtension";
            this.TxtFileExtension.Size = new System.Drawing.Size(251, 23);
            this.TxtFileExtension.TabIndex = 4;
            // 
            // ChbRecursive
            // 
            this.ChbRecursive.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChbRecursive.AutoSize = true;
            this.ChbRecursive.Checked = true;
            this.ChbRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChbRecursive.Location = new System.Drawing.Point(109, 36);
            this.ChbRecursive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChbRecursive.Name = "ChbRecursive";
            this.ChbRecursive.Size = new System.Drawing.Size(76, 19);
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
            this.LsbExcludedDirectories.ItemHeight = 15;
            this.LsbExcludedDirectories.Location = new System.Drawing.Point(109, 297);
            this.LsbExcludedDirectories.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LsbExcludedDirectories.Name = "LsbExcludedDirectories";
            this.tableLayoutPanel3.SetRowSpan(this.LsbExcludedDirectories, 2);
            this.LsbExcludedDirectories.Size = new System.Drawing.Size(251, 184);
            this.LsbExcludedDirectories.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 92);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 15);
            this.label10.TabIndex = 20;
            this.label10.Text = "File Extentions:";
            // 
            // ChbMonitorFileChanges
            // 
            this.ChbMonitorFileChanges.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChbMonitorFileChanges.AutoSize = true;
            this.ChbMonitorFileChanges.Checked = true;
            this.ChbMonitorFileChanges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChbMonitorFileChanges.Location = new System.Drawing.Point(109, 61);
            this.ChbMonitorFileChanges.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChbMonitorFileChanges.Name = "ChbMonitorFileChanges";
            this.ChbMonitorFileChanges.Size = new System.Drawing.Size(135, 19);
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
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(404, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.19976F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34.81192F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.98832F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(353, 513);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // ServerConfigDlg
            // 
            this.AcceptButton = this.BtnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(785, 575);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDurationDifference)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumThumbnailViewCount)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
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
        private System.Windows.Forms.NumericUpDown NumMaxDifferentImages;
        private System.Windows.Forms.NumericUpDown NumMaxImageComparison;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton RdbDurationDifferencePercent;
        private System.Windows.Forms.NumericUpDown NumMaxDurationDifference;
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
        private System.Windows.Forms.Button BtnCustomVideoComparison;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private Label LblMaxDurationDifferenceUnit;
    }
}
