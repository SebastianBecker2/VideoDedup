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
            groupBox4 = new GroupBox();
            label9 = new Label();
            NumThumbnailViewCount = new NumericUpDown();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            groupBox5 = new GroupBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            label2 = new Label();
            label8 = new Label();
            label11 = new Label();
            CmbVideoDedupServiceLogLevel = new ComboBox();
            CmbComparisonManagerLogLevel = new ComboBox();
            CmbDedupEngineLogLevel = new ComboBox();
            CscComparisonSettings = new Controls.ComparisonSettings.ComparisonSettingsCtrl();
            VicVideoInput = new Controls.VideoInput.VideoInputCtrl();
            groupBox4.SuspendLayout();
            ((ISupportInitialize)NumThumbnailViewCount).BeginInit();
            tableLayoutPanel1.SuspendLayout();
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
            // groupBox4
            // 
            groupBox4.Controls.Add(label9);
            groupBox4.Controls.Add(NumThumbnailViewCount);
            groupBox4.Dock = DockStyle.Fill;
            groupBox4.Location = new Point(0, 260);
            groupBox4.Margin = new Padding(0);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4, 3, 4, 3);
            groupBox4.Size = new Size(353, 66);
            groupBox4.TabIndex = 26;
            groupBox4.TabStop = false;
            groupBox4.Text = "Resolving Duplicates";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.None;
            label9.AutoSize = true;
            label9.Location = new Point(92, 28);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(163, 15);
            label9.TabIndex = 19;
            label9.Text = "Number of Images to display:";
            // 
            // NumThumbnailViewCount
            // 
            NumThumbnailViewCount.Anchor = AnchorStyles.None;
            NumThumbnailViewCount.Location = new Point(266, 26);
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
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(VicVideoInput, 0, 0);
            tableLayoutPanel1.Location = new Point(14, 14);
            tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(757, 513);
            tableLayoutPanel1.TabIndex = 27;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(groupBox4, 0, 1);
            tableLayoutPanel2.Controls.Add(groupBox5, 0, 2);
            tableLayoutPanel2.Controls.Add(CscComparisonSettings, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(404, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50.7763977F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 12.9354954F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 36.288105F));
            tableLayoutPanel2.Size = new Size(353, 513);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(tableLayoutPanel5);
            groupBox5.Dock = DockStyle.Fill;
            groupBox5.Location = new Point(0, 326);
            groupBox5.Margin = new Padding(0);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(353, 187);
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
            tableLayoutPanel5.Size = new Size(347, 165);
            tableLayoutPanel5.TabIndex = 0;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(98, 20);
            label2.Name = "label2";
            label2.Size = new Size(112, 15);
            label2.TabIndex = 0;
            label2.Text = "VideoDedupService:";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(88, 75);
            label8.Name = "label8";
            label8.Size = new Size(122, 15);
            label8.TabIndex = 0;
            label8.Text = "ComparisonManager:";
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Location = new Point(129, 130);
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
            CmbVideoDedupServiceLogLevel.Location = new Point(216, 16);
            CmbVideoDedupServiceLogLevel.Name = "CmbVideoDedupServiceLogLevel";
            CmbVideoDedupServiceLogLevel.Size = new Size(128, 23);
            CmbVideoDedupServiceLogLevel.TabIndex = 1;
            // 
            // CmbComparisonManagerLogLevel
            // 
            CmbComparisonManagerLogLevel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CmbComparisonManagerLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbComparisonManagerLogLevel.FormattingEnabled = true;
            CmbComparisonManagerLogLevel.Location = new Point(216, 71);
            CmbComparisonManagerLogLevel.Name = "CmbComparisonManagerLogLevel";
            CmbComparisonManagerLogLevel.Size = new Size(128, 23);
            CmbComparisonManagerLogLevel.TabIndex = 1;
            // 
            // CmbDedupEngineLogLevel
            // 
            CmbDedupEngineLogLevel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CmbDedupEngineLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbDedupEngineLogLevel.FormattingEnabled = true;
            CmbDedupEngineLogLevel.Location = new Point(216, 126);
            CmbDedupEngineLogLevel.Name = "CmbDedupEngineLogLevel";
            CmbDedupEngineLogLevel.Size = new Size(128, 23);
            CmbDedupEngineLogLevel.TabIndex = 1;
            // 
            // CscComparisonSettings
            // 
            CscComparisonSettings.Location = new Point(3, 3);
            CscComparisonSettings.Name = "CscComparisonSettings";
            CscComparisonSettings.Size = new Size(347, 254);
            CscComparisonSettings.TabIndex = 28;
            CscComparisonSettings.TryComparisonClick += CscComparisonSettings_TryComparisonClick;
            // 
            // VicVideoInput
            // 
            VicVideoInput.Location = new Point(3, 3);
            VicVideoInput.Name = "VicVideoInput";
            VicVideoInput.Size = new Size(398, 507);
            VicVideoInput.TabIndex = 1;
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
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((ISupportInitialize)NumThumbnailViewCount).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button BtnCancel;
        private Button BtnOkay;
        private GroupBox groupBox4;
        private Label label9;
        private NumericUpDown NumThumbnailViewCount;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private GroupBox groupBox5;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label2;
        private Label label8;
        private Label label11;
        private ComboBox CmbVideoDedupServiceLogLevel;
        private ComboBox CmbComparisonManagerLogLevel;
        private ComboBox CmbDedupEngineLogLevel;
        private Controls.VideoInput.VideoInputCtrl VicVideoInput;
        private Controls.ComparisonSettings.ComparisonSettingsCtrl CscComparisonSettings;
    }
}
