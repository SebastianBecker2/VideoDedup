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
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            CscComparisonSettings = new Controls.ComparisonSettings.ComparisonSettingsCtrl();
            LscLogSettings = new Controls.LogSettings.LogSettingsCtrl();
            VicVideoInput = new Controls.VideoInput.VideoInputCtrl();
            RscResolutionSettings = new Controls.ResolutionSettings.ResolutionSettingsCtrl();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
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
            tableLayoutPanel2.Controls.Add(CscComparisonSettings, 0, 0);
            tableLayoutPanel2.Controls.Add(LscLogSettings, 0, 2);
            tableLayoutPanel2.Controls.Add(RscResolutionSettings, 0, 1);
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
            // CscComparisonSettings
            // 
            CscComparisonSettings.Location = new Point(3, 3);
            CscComparisonSettings.Name = "CscComparisonSettings";
            CscComparisonSettings.Size = new Size(347, 254);
            CscComparisonSettings.TabIndex = 28;
            CscComparisonSettings.TryComparisonClick += CscComparisonSettings_TryComparisonClick;
            // 
            // LscLogSettings
            // 
            LscLogSettings.Location = new Point(3, 329);
            LscLogSettings.Name = "LscLogSettings";
            LscLogSettings.Size = new Size(347, 176);
            LscLogSettings.TabIndex = 29;
            // 
            // VicVideoInput
            // 
            VicVideoInput.Location = new Point(3, 3);
            VicVideoInput.Name = "VicVideoInput";
            VicVideoInput.Size = new Size(398, 507);
            VicVideoInput.TabIndex = 1;
            // 
            // RscResolutionSettings
            // 
            RscResolutionSettings.Location = new Point(3, 263);
            RscResolutionSettings.Name = "RscResolutionSettings";
            RscResolutionSettings.Size = new Size(347, 60);
            RscResolutionSettings.TabIndex = 30;
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
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button BtnCancel;
        private Button BtnOkay;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Controls.VideoInput.VideoInputCtrl VicVideoInput;
        private Controls.ComparisonSettings.ComparisonSettingsCtrl CscComparisonSettings;
        private Controls.LogSettings.LogSettingsCtrl LscLogSettings;
        private Controls.ResolutionSettings.ResolutionSettingsCtrl RscResolutionSettings;
    }
}
