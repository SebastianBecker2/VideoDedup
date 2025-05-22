namespace VideoDedupClient.Controls.ResolutionSettings
{
    partial class ResolutionSettingsCtrl
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
            groupBox4 = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            label9 = new Label();
            NumThumbnailViewCount = new NumericUpDown();
            tableLayoutPanel2 = new TableLayoutPanel();
            RdbDeleteFiles = new RadioButton();
            RdbMoveToTrash = new RadioButton();
            PibMoveToTrashHint = new PictureBox();
            TipHints = new ToolTip(components);
            groupBox4.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumThumbnailViewCount).BeginInit();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PibMoveToTrashHint).BeginInit();
            SuspendLayout();
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(tableLayoutPanel1);
            groupBox4.Dock = DockStyle.Fill;
            groupBox4.Location = new Point(0, 0);
            groupBox4.Margin = new Padding(0);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4, 3, 4, 3);
            groupBox4.Size = new Size(352, 259);
            groupBox4.TabIndex = 27;
            groupBox4.TabStop = false;
            groupBox4.Text = "Resolving Duplicates";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label9, 0, 0);
            tableLayoutPanel1.Controls.Add(NumThumbnailViewCount, 1, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(4, 19);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(344, 237);
            tableLayoutPanel1.TabIndex = 20;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new Point(5, 51);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(163, 15);
            label9.TabIndex = 19;
            label9.Text = "Number of Images to display:";
            // 
            // NumThumbnailViewCount
            // 
            NumThumbnailViewCount.Anchor = AnchorStyles.None;
            NumThumbnailViewCount.Location = new Point(221, 47);
            NumThumbnailViewCount.Margin = new Padding(4, 3, 4, 3);
            NumThumbnailViewCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumThumbnailViewCount.Name = "NumThumbnailViewCount";
            NumThumbnailViewCount.Size = new Size(74, 23);
            NumThumbnailViewCount.TabIndex = 0;
            NumThumbnailViewCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel1.SetColumnSpan(tableLayoutPanel2, 2);
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(RdbDeleteFiles, 0, 0);
            tableLayoutPanel2.Controls.Add(RdbMoveToTrash, 0, 1);
            tableLayoutPanel2.Controls.Add(PibMoveToTrashHint, 1, 1);
            tableLayoutPanel2.Location = new Point(83, 121);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(178, 113);
            tableLayoutPanel2.TabIndex = 22;
            // 
            // RdbDeleteFiles
            // 
            RdbDeleteFiles.Anchor = AnchorStyles.Left;
            RdbDeleteFiles.AutoSize = true;
            RdbDeleteFiles.Location = new Point(3, 18);
            RdbDeleteFiles.Name = "RdbDeleteFiles";
            RdbDeleteFiles.Size = new Size(134, 19);
            RdbDeleteFiles.TabIndex = 20;
            RdbDeleteFiles.TabStop = true;
            RdbDeleteFiles.Text = "Delete Files immediately";
            RdbDeleteFiles.UseVisualStyleBackColor = true;
            // 
            // RdbMoveToTrash
            // 
            RdbMoveToTrash.Anchor = AnchorStyles.Left;
            RdbMoveToTrash.AutoSize = true;
            RdbMoveToTrash.Location = new Point(3, 75);
            RdbMoveToTrash.Name = "RdbMoveToTrash";
            RdbMoveToTrash.Size = new Size(134, 19);
            RdbMoveToTrash.TabIndex = 20;
            RdbMoveToTrash.TabStop = true;
            RdbMoveToTrash.Text = "Move deleted Files to Trash";
            RdbMoveToTrash.UseVisualStyleBackColor = true;
            // 
            // PibMoveToTrashHint
            // 
            PibMoveToTrashHint.Anchor = AnchorStyles.Left;
            PibMoveToTrashHint.Image = Properties.Resources.information;
            PibMoveToTrashHint.Location = new Point(143, 68);
            PibMoveToTrashHint.Name = "PibMoveToTrashHint";
            PibMoveToTrashHint.Size = new Size(32, 32);
            PibMoveToTrashHint.SizeMode = PictureBoxSizeMode.AutoSize;
            PibMoveToTrashHint.TabIndex = 21;
            PibMoveToTrashHint.TabStop = false;
            PibMoveToTrashHint.Click += PibMoveToTrashHint_Click;
            // 
            // ResolutionSettingsCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox4);
            Name = "ResolutionSettingsCtrl";
            Size = new Size(352, 259);
            groupBox4.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NumThumbnailViewCount).EndInit();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PibMoveToTrashHint).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox4;
        private Label label9;
        public NumericUpDown NumThumbnailViewCount;
        private TableLayoutPanel tableLayoutPanel1;
        public RadioButton RdbDeleteFiles;
        public RadioButton RdbMoveToTrash;
        private TableLayoutPanel tableLayoutPanel2;
        private PictureBox PibMoveToTrashHint;
        private ToolTip TipHints;
    }
}
