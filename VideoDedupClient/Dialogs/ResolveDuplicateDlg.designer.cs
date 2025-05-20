namespace VideoDedupClient.Dialogs
{
    using System.ComponentModel;
    using Controls.FilePreview;

    partial class ResolveDuplicateDlg
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
            var resources = new ComponentResourceManager(typeof(ResolveDuplicateDlg));
            SplitterContainer = new SplitContainer();
            tableLayoutPanel1 = new TableLayoutPanel();
            FpvLeft = new FilePreviewCtl();
            btnDeleteLeft = new Button();
            BtnShowLeft = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            FpvRight = new FilePreviewCtl();
            btnDeleteRight = new Button();
            BtnShowRight = new Button();
            BtnClose = new Button();
            BtnSkip = new Button();
            BtnDiscard = new Button();
            BtnReviewComparison = new Button();
            ((ISupportInitialize)SplitterContainer).BeginInit();
            SplitterContainer.Panel1.SuspendLayout();
            SplitterContainer.Panel2.SuspendLayout();
            SplitterContainer.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // SplitterContainer
            // 
            SplitterContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SplitterContainer.Location = new Point(14, 14);
            SplitterContainer.Margin = new Padding(4, 3, 4, 3);
            SplitterContainer.Name = "SplitterContainer";
            // 
            // SplitterContainer.Panel1
            // 
            SplitterContainer.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // SplitterContainer.Panel2
            // 
            SplitterContainer.Panel2.Controls.Add(tableLayoutPanel2);
            SplitterContainer.Size = new Size(852, 467);
            SplitterContainer.SplitterDistance = 419;
            SplitterContainer.SplitterWidth = 7;
            SplitterContainer.TabIndex = 0;
            SplitterContainer.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(FpvLeft, 0, 0);
            tableLayoutPanel1.Controls.Add(btnDeleteLeft, 1, 1);
            tableLayoutPanel1.Controls.Add(BtnShowLeft, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(419, 467);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // FpvLeft
            // 
            FpvLeft.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(FpvLeft, 2);
            FpvLeft.HighlightColor = SystemColors.Control;
            FpvLeft.Location = new Point(4, 3);
            FpvLeft.Margin = new Padding(4, 3, 4, 3);
            FpvLeft.Name = "FpvLeft";
            FpvLeft.Size = new Size(411, 428);
            FpvLeft.TabIndex = 3;
            FpvLeft.VideoFile = null;
            // 
            // btnDeleteLeft
            // 
            btnDeleteLeft.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteLeft.Location = new Point(125, 437);
            btnDeleteLeft.Margin = new Padding(4, 3, 4, 3);
            btnDeleteLeft.Name = "btnDeleteLeft";
            btnDeleteLeft.Size = new Size(88, 27);
            btnDeleteLeft.TabIndex = 2;
            btnDeleteLeft.Text = "Delete &Left";
            btnDeleteLeft.UseVisualStyleBackColor = true;
            btnDeleteLeft.Click += BtnDeleteLeft_Click;
            // 
            // BtnShowLeft
            // 
            BtnShowLeft.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnShowLeft.Location = new Point(4, 437);
            BtnShowLeft.Margin = new Padding(4, 3, 4, 3);
            BtnShowLeft.Name = "BtnShowLeft";
            BtnShowLeft.Size = new Size(113, 27);
            BtnShowLeft.TabIndex = 1;
            BtnShowLeft.Text = "&Show in Explorer";
            BtnShowLeft.UseVisualStyleBackColor = true;
            BtnShowLeft.Click += BtnShowLeft_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(btnDeleteRight, 0, 1);
            tableLayoutPanel2.Controls.Add(FpvRight, 0, 0);
            tableLayoutPanel2.Controls.Add(BtnShowRight, 1, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.Size = new Size(426, 467);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // FpvRight
            // 
            FpvRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.SetColumnSpan(FpvRight, 2);
            FpvRight.HighlightColor = SystemColors.Control;
            FpvRight.Location = new Point(4, 3);
            FpvRight.Margin = new Padding(4, 3, 4, 3);
            FpvRight.Name = "FpvRight";
            FpvRight.Size = new Size(418, 428);
            FpvRight.TabIndex = 3;
            FpvRight.VideoFile = null;
            // 
            // btnDeleteRight
            // 
            btnDeleteRight.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDeleteRight.Location = new Point(213, 437);
            btnDeleteRight.Margin = new Padding(4, 3, 4, 3);
            btnDeleteRight.Name = "btnDeleteRight";
            btnDeleteRight.Size = new Size(88, 27);
            btnDeleteRight.TabIndex = 1;
            btnDeleteRight.Text = "Delete &Right";
            btnDeleteRight.UseVisualStyleBackColor = true;
            btnDeleteRight.Click += BtnDeleteRight_Click;
            // 
            // BtnShowRight
            // 
            BtnShowRight.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnShowRight.Location = new Point(309, 437);
            BtnShowRight.Margin = new Padding(4, 3, 4, 3);
            BtnShowRight.Name = "BtnShowRight";
            BtnShowRight.Size = new Size(113, 27);
            BtnShowRight.TabIndex = 2;
            BtnShowRight.Text = "Show in &Explorer";
            BtnShowRight.UseVisualStyleBackColor = true;
            BtnShowRight.Click += BtnShowRight_Click;
            // 
            // BtnClose
            // 
            BtnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnClose.Location = new Point(779, 487);
            BtnClose.Margin = new Padding(4, 3, 4, 3);
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new Size(88, 27);
            BtnClose.TabIndex = 4;
            BtnClose.Text = "&Close";
            BtnClose.UseVisualStyleBackColor = true;
            BtnClose.Click += BtnClose_Click;
            // 
            // BtnSkip
            // 
            BtnSkip.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnSkip.Location = new Point(684, 487);
            BtnSkip.Margin = new Padding(4, 3, 4, 3);
            BtnSkip.Name = "BtnSkip";
            BtnSkip.Size = new Size(88, 27);
            BtnSkip.TabIndex = 3;
            BtnSkip.Text = "&Skip";
            BtnSkip.UseVisualStyleBackColor = true;
            BtnSkip.Click += BtnSkip_Click;
            // 
            // BtnDiscard
            // 
            BtnDiscard.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnDiscard.Location = new Point(590, 487);
            BtnDiscard.Margin = new Padding(4, 3, 4, 3);
            BtnDiscard.Name = "BtnDiscard";
            BtnDiscard.Size = new Size(88, 27);
            BtnDiscard.TabIndex = 2;
            BtnDiscard.Text = "&Discard";
            BtnDiscard.UseVisualStyleBackColor = true;
            BtnDiscard.Click += BtnDiscard_Click;
            // 
            // BtnReviewComparison
            // 
            BtnReviewComparison.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnReviewComparison.Location = new Point(455, 487);
            BtnReviewComparison.Margin = new Padding(4, 3, 4, 3);
            BtnReviewComparison.Name = "BtnReviewComparison";
            BtnReviewComparison.Size = new Size(127, 27);
            BtnReviewComparison.TabIndex = 1;
            BtnReviewComparison.Text = "Re&view Comparison";
            BtnReviewComparison.UseVisualStyleBackColor = true;
            BtnReviewComparison.Click += BtnReviewComparison_Click;
            // 
            // ResolveDuplicateDlg
            // 
            AcceptButton = BtnSkip;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = BtnClose;
            ClientSize = new Size(880, 528);
            Controls.Add(BtnReviewComparison);
            Controls.Add(BtnDiscard);
            Controls.Add(BtnSkip);
            Controls.Add(BtnClose);
            Controls.Add(SplitterContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "ResolveDuplicateDlg";
            Text = "Resolve Duplicate";
            WindowState = FormWindowState.Maximized;
            SplitterContainer.Panel1.ResumeLayout(false);
            SplitterContainer.Panel2.ResumeLayout(false);
            ((ISupportInitialize)SplitterContainer).EndInit();
            SplitterContainer.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private SplitContainer SplitterContainer;
        private Button BtnClose;
        private Button btnDeleteLeft;
        private Button btnDeleteRight;
        private Button BtnShowRight;
        private Button BtnShowLeft;
        private Button BtnSkip;
        private Button BtnDiscard;
        private Button BtnReviewComparison;
        private FilePreviewCtl FpvLeft;
        private FilePreviewCtl FpvRight;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
    }
}
