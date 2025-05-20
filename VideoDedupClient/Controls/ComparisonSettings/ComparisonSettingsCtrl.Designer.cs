namespace VideoDedupClient.Controls.ComparisonSettings
{
    partial class ComparisonSettingsCtrl
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
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new TableLayoutPanel();
            groupBox2 = new GroupBox();
            LblMaxDurationDifferenceUnit = new Label();
            label7 = new Label();
            RdbDurationDifferencePercent = new RadioButton();
            NumMaxDurationDifference = new NumericUpDown();
            RdbDurationDifferenceSeconds = new RadioButton();
            groupBox1 = new GroupBox();
            tableLayoutPanel4 = new TableLayoutPanel();
            BtnCustomVideoComparison = new Button();
            NumMaxImageComparison = new NumericUpDown();
            label6 = new Label();
            NumMaxDifferentImages = new NumericUpDown();
            label5 = new Label();
            NumMaxDifferentPercentage = new NumericUpDown();
            label3 = new Label();
            PibMaxDifferentPercentageInfo = new PictureBox();
            TipHints = new ToolTip(components);
            tableLayoutPanel1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumMaxDurationDifference).BeginInit();
            groupBox1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumMaxImageComparison).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumMaxDifferentImages).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumMaxDifferentPercentage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PibMaxDifferentPercentageInfo).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(groupBox2, 0, 1);
            tableLayoutPanel1.Controls.Add(groupBox1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(373, 327);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(LblMaxDurationDifferenceUnit);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(RdbDurationDifferencePercent);
            groupBox2.Controls.Add(NumMaxDurationDifference);
            groupBox2.Controls.Add(RdbDurationDifferenceSeconds);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 163);
            groupBox2.Margin = new Padding(0);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(373, 164);
            groupBox2.TabIndex = 25;
            groupBox2.TabStop = false;
            groupBox2.Text = "Comparing Duration";
            // 
            // LblMaxDurationDifferenceUnit
            // 
            LblMaxDurationDifferenceUnit.Anchor = AnchorStyles.None;
            LblMaxDurationDifferenceUnit.AutoSize = true;
            LblMaxDurationDifferenceUnit.Location = new Point(301, 110);
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
            label7.Location = new Point(90, 110);
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
            RdbDurationDifferencePercent.Location = new Point(33, 69);
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
            NumMaxDurationDifference.Location = new Point(219, 108);
            NumMaxDurationDifference.Margin = new Padding(4, 3, 4, 3);
            NumMaxDurationDifference.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NumMaxDurationDifference.Name = "NumMaxDurationDifference";
            NumMaxDurationDifference.Size = new Size(74, 23);
            NumMaxDurationDifference.TabIndex = 2;
            // 
            // RdbDurationDifferenceSeconds
            // 
            RdbDurationDifferenceSeconds.Anchor = AnchorStyles.None;
            RdbDurationDifferenceSeconds.AutoSize = true;
            RdbDurationDifferenceSeconds.Location = new Point(33, 44);
            RdbDurationDifferenceSeconds.Margin = new Padding(4, 3, 4, 3);
            RdbDurationDifferenceSeconds.Name = "RdbDurationDifferenceSeconds";
            RdbDurationDifferenceSeconds.Size = new Size(181, 19);
            RdbDurationDifferenceSeconds.TabIndex = 0;
            RdbDurationDifferenceSeconds.TabStop = true;
            RdbDurationDifferenceSeconds.Text = "Absolut difference in seconds";
            RdbDurationDifferenceSeconds.UseVisualStyleBackColor = true;
            RdbDurationDifferenceSeconds.CheckedChanged += HandleDurationDifferenceTypeChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tableLayoutPanel4);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(0);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(373, 163);
            groupBox1.TabIndex = 24;
            groupBox1.TabStop = false;
            groupBox1.Text = "Comparing Images";
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 3;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel4.Controls.Add(BtnCustomVideoComparison, 0, 3);
            tableLayoutPanel4.Controls.Add(NumMaxImageComparison, 1, 0);
            tableLayoutPanel4.Controls.Add(label6, 0, 2);
            tableLayoutPanel4.Controls.Add(NumMaxDifferentImages, 1, 1);
            tableLayoutPanel4.Controls.Add(label5, 0, 1);
            tableLayoutPanel4.Controls.Add(NumMaxDifferentPercentage, 1, 2);
            tableLayoutPanel4.Controls.Add(label3, 0, 0);
            tableLayoutPanel4.Controls.Add(PibMaxDifferentPercentageInfo, 2, 2);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(4, 19);
            tableLayoutPanel4.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 4;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.Size = new Size(365, 141);
            tableLayoutPanel4.TabIndex = 14;
            // 
            // BtnCustomVideoComparison
            // 
            BtnCustomVideoComparison.Anchor = AnchorStyles.Right;
            tableLayoutPanel4.SetColumnSpan(BtnCustomVideoComparison, 2);
            BtnCustomVideoComparison.Location = new Point(179, 109);
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
            NumMaxImageComparison.Location = new Point(249, 6);
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
            label6.Location = new Point(49, 80);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(192, 15);
            label6.TabIndex = 13;
            label6.Text = "Accepted percentage of difference:";
            // 
            // NumMaxDifferentImages
            // 
            NumMaxDifferentImages.Anchor = AnchorStyles.None;
            NumMaxDifferentImages.Location = new Point(249, 41);
            NumMaxDifferentImages.Margin = new Padding(4, 3, 4, 3);
            NumMaxDifferentImages.Name = "NumMaxDifferentImages";
            NumMaxDifferentImages.Size = new Size(74, 23);
            NumMaxDifferentImages.TabIndex = 1;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(33, 45);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(208, 15);
            label5.TabIndex = 12;
            label5.Text = "Accepted number of different Images:";
            // 
            // NumMaxDifferentPercentage
            // 
            NumMaxDifferentPercentage.Anchor = AnchorStyles.None;
            NumMaxDifferentPercentage.Location = new Point(249, 76);
            NumMaxDifferentPercentage.Margin = new Padding(4, 3, 4, 3);
            NumMaxDifferentPercentage.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            NumMaxDifferentPercentage.Name = "NumMaxDifferentPercentage";
            NumMaxDifferentPercentage.Size = new Size(74, 23);
            NumMaxDifferentPercentage.TabIndex = 2;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(68, 10);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(173, 15);
            label3.TabIndex = 11;
            label3.Text = "Number of Images to compare:";
            // 
            // PibMaxDifferentPercentageInfo
            // 
            PibMaxDifferentPercentageInfo.Image = Properties.Resources.information;
            PibMaxDifferentPercentageInfo.Location = new Point(330, 73);
            PibMaxDifferentPercentageInfo.Name = "PibMaxDifferentPercentageInfo";
            PibMaxDifferentPercentageInfo.Size = new Size(32, 29);
            PibMaxDifferentPercentageInfo.SizeMode = PictureBoxSizeMode.AutoSize;
            PibMaxDifferentPercentageInfo.TabIndex = 14;
            PibMaxDifferentPercentageInfo.TabStop = false;
            PibMaxDifferentPercentageInfo.Click += PibMaxDifferentPercentageInfo_Click;
            // 
            // ComparisonSettingsCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "ComparisonSettingsCtrl";
            Size = new Size(373, 327);
            tableLayoutPanel1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NumMaxDurationDifference).EndInit();
            groupBox1.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NumMaxImageComparison).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumMaxDifferentImages).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumMaxDifferentPercentage).EndInit();
            ((System.ComponentModel.ISupportInitialize)PibMaxDifferentPercentageInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox groupBox1;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label6;
        private Label label5;
        private Label label3;
        private GroupBox groupBox2;
        private Label LblMaxDurationDifferenceUnit;
        private Label label7;
        public NumericUpDown NumMaxImageComparison;
        public NumericUpDown NumMaxDifferentImages;
        public NumericUpDown NumMaxDifferentPercentage;
        public RadioButton RdbDurationDifferencePercent;
        public NumericUpDown NumMaxDurationDifference;
        public RadioButton RdbDurationDifferenceSeconds;
        private Button BtnCustomVideoComparison;
        private PictureBox PibMaxDifferentPercentageInfo;
        private ToolTip TipHints;
    }
}
