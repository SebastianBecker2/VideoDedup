namespace VideoDedupClient.Controls.StatusInfo
{
    using System.ComponentModel;

    partial class StatusInfoCtl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            LblDuplicateCount = new Label();
            LblCurrentFileCount = new Label();
            LblElapsedTime = new Label();
            LblRemainingTime = new Label();
            LblCurrentFileCountTitle = new Label();
            LblStatusInfo = new Label();
            LblDuplicateCountTitle = new Label();
            LblFileCountSpeedTitle = new Label();
            LblDuplicateSpeedTitle = new Label();
            LblElapsedTimeTitle = new Label();
            LblRemainingTimeTitle = new Label();
            LblFileCountSpeed = new Label();
            LblDuplicateSpeed = new Label();
            LblDuplicateSpeedUnit = new Label();
            LblFileCountSpeedUnit = new Label();
            PrgProgress = new Controls.ProgressGraph.ProgressGraphCtrl();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 7;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(LblDuplicateCount, 1, 3);
            tableLayoutPanel1.Controls.Add(LblCurrentFileCount, 1, 2);
            tableLayoutPanel1.Controls.Add(LblElapsedTime, 6, 2);
            tableLayoutPanel1.Controls.Add(LblRemainingTime, 6, 3);
            tableLayoutPanel1.Controls.Add(LblCurrentFileCountTitle, 0, 2);
            tableLayoutPanel1.Controls.Add(LblStatusInfo, 0, 0);
            tableLayoutPanel1.Controls.Add(LblDuplicateCountTitle, 0, 3);
            tableLayoutPanel1.Controls.Add(LblFileCountSpeedTitle, 2, 2);
            tableLayoutPanel1.Controls.Add(LblDuplicateSpeedTitle, 2, 3);
            tableLayoutPanel1.Controls.Add(LblElapsedTimeTitle, 5, 2);
            tableLayoutPanel1.Controls.Add(LblRemainingTimeTitle, 5, 3);
            tableLayoutPanel1.Controls.Add(LblFileCountSpeed, 3, 2);
            tableLayoutPanel1.Controls.Add(LblDuplicateSpeed, 3, 3);
            tableLayoutPanel1.Controls.Add(LblDuplicateSpeedUnit, 4, 3);
            tableLayoutPanel1.Controls.Add(LblFileCountSpeedUnit, 4, 2);
            tableLayoutPanel1.Controls.Add(PrgProgress, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 13.7513695F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 58.7458763F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 13.7513752F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 13.7513695F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(682, 367);
            tableLayoutPanel1.TabIndex = 14;
            // 
            // LblDuplicateCount
            // 
            LblDuplicateCount.Anchor = AnchorStyles.Left;
            LblDuplicateCount.AutoSize = true;
            LblDuplicateCount.Location = new Point(65, 333);
            LblDuplicateCount.Margin = new Padding(0, 5, 0, 5);
            LblDuplicateCount.Name = "LblDuplicateCount";
            LblDuplicateCount.Size = new Size(106, 15);
            LblDuplicateCount.TabIndex = 12;
            LblDuplicateCount.Text = "LblDuplicateCount";
            // 
            // LblCurrentFileCount
            // 
            LblCurrentFileCount.Anchor = AnchorStyles.Left;
            LblCurrentFileCount.AutoSize = true;
            LblCurrentFileCount.Location = new Point(65, 282);
            LblCurrentFileCount.Margin = new Padding(0, 5, 0, 5);
            LblCurrentFileCount.Name = "LblCurrentFileCount";
            LblCurrentFileCount.Size = new Size(114, 15);
            LblCurrentFileCount.TabIndex = 6;
            LblCurrentFileCount.Text = "LblCurrentFileCount";
            // 
            // LblElapsedTime
            // 
            LblElapsedTime.Anchor = AnchorStyles.Right;
            LblElapsedTime.AutoSize = true;
            LblElapsedTime.Location = new Point(593, 282);
            LblElapsedTime.Margin = new Padding(0, 5, 0, 5);
            LblElapsedTime.Name = "LblElapsedTime";
            LblElapsedTime.Size = new Size(89, 15);
            LblElapsedTime.TabIndex = 9;
            LblElapsedTime.Text = "LblElapsedTime";
            // 
            // LblRemainingTime
            // 
            LblRemainingTime.Anchor = AnchorStyles.Right;
            LblRemainingTime.AutoSize = true;
            LblRemainingTime.Location = new Point(576, 333);
            LblRemainingTime.Margin = new Padding(0, 5, 0, 5);
            LblRemainingTime.Name = "LblRemainingTime";
            LblRemainingTime.Size = new Size(106, 15);
            LblRemainingTime.TabIndex = 14;
            LblRemainingTime.Text = "LblRemainingTime";
            // 
            // LblCurrentFileCountTitle
            // 
            LblCurrentFileCountTitle.Anchor = AnchorStyles.Left;
            LblCurrentFileCountTitle.AutoSize = true;
            LblCurrentFileCountTitle.Location = new Point(0, 282);
            LblCurrentFileCountTitle.Margin = new Padding(0, 5, 0, 5);
            LblCurrentFileCountTitle.Name = "LblCurrentFileCountTitle";
            LblCurrentFileCountTitle.Size = new Size(55, 15);
            LblCurrentFileCountTitle.TabIndex = 6;
            LblCurrentFileCountTitle.Text = "Progress:";
            // 
            // LblStatusInfo
            // 
            LblStatusInfo.Anchor = AnchorStyles.Left;
            LblStatusInfo.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(LblStatusInfo, 7);
            LblStatusInfo.Location = new Point(0, 17);
            LblStatusInfo.Margin = new Padding(0, 5, 0, 5);
            LblStatusInfo.Name = "LblStatusInfo";
            LblStatusInfo.Size = new Size(76, 15);
            LblStatusInfo.TabIndex = 6;
            LblStatusInfo.Text = "LblStatusInfo";
            // 
            // LblDuplicateCountTitle
            // 
            LblDuplicateCountTitle.Anchor = AnchorStyles.Left;
            LblDuplicateCountTitle.AutoSize = true;
            LblDuplicateCountTitle.Location = new Point(0, 333);
            LblDuplicateCountTitle.Margin = new Padding(0, 5, 0, 5);
            LblDuplicateCountTitle.Name = "LblDuplicateCountTitle";
            LblDuplicateCountTitle.Size = new Size(65, 15);
            LblDuplicateCountTitle.TabIndex = 12;
            LblDuplicateCountTitle.Text = "Duplicates:";
            // 
            // LblFileCountSpeedTitle
            // 
            LblFileCountSpeedTitle.Anchor = AnchorStyles.Right;
            LblFileCountSpeedTitle.AutoSize = true;
            LblFileCountSpeedTitle.Location = new Point(249, 282);
            LblFileCountSpeedTitle.Margin = new Padding(0, 5, 0, 5);
            LblFileCountSpeedTitle.Name = "LblFileCountSpeedTitle";
            LblFileCountSpeedTitle.Size = new Size(42, 15);
            LblFileCountSpeedTitle.TabIndex = 14;
            LblFileCountSpeedTitle.Text = "Speed:";
            // 
            // LblDuplicateSpeedTitle
            // 
            LblDuplicateSpeedTitle.Anchor = AnchorStyles.Right;
            LblDuplicateSpeedTitle.AutoSize = true;
            LblDuplicateSpeedTitle.Location = new Point(249, 333);
            LblDuplicateSpeedTitle.Margin = new Padding(0, 5, 0, 5);
            LblDuplicateSpeedTitle.Name = "LblDuplicateSpeedTitle";
            LblDuplicateSpeedTitle.Size = new Size(42, 15);
            LblDuplicateSpeedTitle.TabIndex = 14;
            LblDuplicateSpeedTitle.Text = "Speed:";
            // 
            // LblElapsedTimeTitle
            // 
            LblElapsedTimeTitle.Anchor = AnchorStyles.Right;
            LblElapsedTimeTitle.AutoSize = true;
            LblElapsedTimeTitle.Location = new Point(526, 282);
            LblElapsedTimeTitle.Margin = new Padding(0, 5, 0, 5);
            LblElapsedTimeTitle.Name = "LblElapsedTimeTitle";
            LblElapsedTimeTitle.Size = new Size(50, 15);
            LblElapsedTimeTitle.TabIndex = 9;
            LblElapsedTimeTitle.Text = "Elapsed:";
            // 
            // LblRemainingTimeTitle
            // 
            LblRemainingTimeTitle.Anchor = AnchorStyles.Right;
            LblRemainingTimeTitle.AutoSize = true;
            LblRemainingTimeTitle.Location = new Point(509, 333);
            LblRemainingTimeTitle.Margin = new Padding(0, 5, 0, 5);
            LblRemainingTimeTitle.Name = "LblRemainingTimeTitle";
            LblRemainingTimeTitle.Size = new Size(67, 15);
            LblRemainingTimeTitle.TabIndex = 14;
            LblRemainingTimeTitle.Text = "Remaining:";
            // 
            // LblFileCountSpeed
            // 
            LblFileCountSpeed.Anchor = AnchorStyles.Right;
            LblFileCountSpeed.AutoSize = true;
            LblFileCountSpeed.Location = new Point(291, 282);
            LblFileCountSpeed.Margin = new Padding(0, 5, 0, 5);
            LblFileCountSpeed.Name = "LblFileCountSpeed";
            LblFileCountSpeed.Size = new Size(106, 15);
            LblFileCountSpeed.TabIndex = 14;
            LblFileCountSpeed.Text = "LblFileCountSpeed";
            // 
            // LblDuplicateSpeed
            // 
            LblDuplicateSpeed.Anchor = AnchorStyles.Right;
            LblDuplicateSpeed.AutoSize = true;
            LblDuplicateSpeed.Location = new Point(292, 333);
            LblDuplicateSpeed.Margin = new Padding(0, 5, 0, 5);
            LblDuplicateSpeed.Name = "LblDuplicateSpeed";
            LblDuplicateSpeed.Size = new Size(105, 15);
            LblDuplicateSpeed.TabIndex = 14;
            LblDuplicateSpeed.Text = "LblDuplicateSpeed";
            // 
            // LblDuplicateSpeedUnit
            // 
            LblDuplicateSpeedUnit.Anchor = AnchorStyles.Left;
            LblDuplicateSpeedUnit.AutoSize = true;
            LblDuplicateSpeedUnit.Location = new Point(397, 333);
            LblDuplicateSpeedUnit.Margin = new Padding(0, 5, 0, 5);
            LblDuplicateSpeedUnit.Name = "LblDuplicateSpeedUnit";
            LblDuplicateSpeedUnit.Size = new Size(78, 15);
            LblDuplicateSpeedUnit.TabIndex = 14;
            LblDuplicateSpeedUnit.Text = "Duplicates / s";
            // 
            // LblFileCountSpeedUnit
            // 
            LblFileCountSpeedUnit.Anchor = AnchorStyles.Left;
            LblFileCountSpeedUnit.AutoSize = true;
            LblFileCountSpeedUnit.Location = new Point(397, 282);
            LblFileCountSpeedUnit.Margin = new Padding(0, 5, 0, 5);
            LblFileCountSpeedUnit.Name = "LblFileCountSpeedUnit";
            LblFileCountSpeedUnit.Size = new Size(46, 15);
            LblFileCountSpeedUnit.TabIndex = 14;
            LblFileCountSpeedUnit.Text = "Files / s";
            // 
            // PrgProgress
            // 
            tableLayoutPanel1.SetColumnSpan(PrgProgress, 7);
            PrgProgress.Dock = DockStyle.Fill;
            PrgProgress.Location = new Point(3, 53);
            PrgProgress.MaxProgress = 100D;
            PrgProgress.Name = "PrgProgress";
            PrgProgress.Size = new Size(676, 209);
            PrgProgress.TabIndex = 15;
            // 
            // StatusInfoCtl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(tableLayoutPanel1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "StatusInfoCtl";
            Size = new Size(682, 367);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label LblStatusInfo;
        private Label LblDuplicateCount;
        private Label LblCurrentFileCount;
        private Label LblElapsedTime;
        private Label LblRemainingTime;
        private Label LblDuplicateSpeed;
        private Label LblFileCountSpeed;
        private Label LblCurrentFileCountTitle;
        private Label LblDuplicateCountTitle;
        private Label LblFileCountSpeedTitle;
        private Label LblDuplicateSpeedTitle;
        private Label LblElapsedTimeTitle;
        private Label LblRemainingTimeTitle;
        private Label LblDuplicateSpeedUnit;
        private Label LblFileCountSpeedUnit;
        private Controls.ProgressGraph.ProgressGraphCtrl PrgProgress;
    }
}
