namespace VideoDedupClient.Controls.LogSettings
{
    partial class LogSettingsCtrl
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
            groupBox5 = new GroupBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            label2 = new Label();
            label8 = new Label();
            label11 = new Label();
            CmbVideoDedupServiceLogLevel = new ComboBox();
            CmbComparisonManagerLogLevel = new ComboBox();
            CmbDedupEngineLogLevel = new ComboBox();
            groupBox5.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(tableLayoutPanel5);
            groupBox5.Dock = DockStyle.Fill;
            groupBox5.Location = new Point(0, 0);
            groupBox5.Margin = new Padding(0);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(373, 176);
            groupBox5.TabIndex = 28;
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
            tableLayoutPanel5.Size = new Size(367, 154);
            tableLayoutPanel5.TabIndex = 0;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(110, 18);
            label2.Name = "label2";
            label2.Size = new Size(112, 15);
            label2.TabIndex = 0;
            label2.Text = "VideoDedupService:";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(100, 69);
            label8.Name = "label8";
            label8.Size = new Size(122, 15);
            label8.TabIndex = 0;
            label8.Text = "ComparisonManager:";
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Location = new Point(141, 120);
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
            CmbVideoDedupServiceLogLevel.Location = new Point(228, 14);
            CmbVideoDedupServiceLogLevel.Name = "CmbVideoDedupServiceLogLevel";
            CmbVideoDedupServiceLogLevel.Size = new Size(136, 23);
            CmbVideoDedupServiceLogLevel.TabIndex = 1;
            // 
            // CmbComparisonManagerLogLevel
            // 
            CmbComparisonManagerLogLevel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CmbComparisonManagerLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbComparisonManagerLogLevel.FormattingEnabled = true;
            CmbComparisonManagerLogLevel.Location = new Point(228, 65);
            CmbComparisonManagerLogLevel.Name = "CmbComparisonManagerLogLevel";
            CmbComparisonManagerLogLevel.Size = new Size(136, 23);
            CmbComparisonManagerLogLevel.TabIndex = 1;
            // 
            // CmbDedupEngineLogLevel
            // 
            CmbDedupEngineLogLevel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CmbDedupEngineLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbDedupEngineLogLevel.FormattingEnabled = true;
            CmbDedupEngineLogLevel.Location = new Point(228, 116);
            CmbDedupEngineLogLevel.Name = "CmbDedupEngineLogLevel";
            CmbDedupEngineLogLevel.Size = new Size(136, 23);
            CmbDedupEngineLogLevel.TabIndex = 1;
            // 
            // LogSettingsCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox5);
            Name = "LogSettingsCtrl";
            Size = new Size(373, 176);
            groupBox5.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox5;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label2;
        private Label label8;
        private Label label11;
        public ComboBox CmbVideoDedupServiceLogLevel;
        public ComboBox CmbComparisonManagerLogLevel;
        public ComboBox CmbDedupEngineLogLevel;
    }
}
