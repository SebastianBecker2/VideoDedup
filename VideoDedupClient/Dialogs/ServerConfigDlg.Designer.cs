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
            CscComparisonSettings = new Controls.ComparisonSettings.ComparisonSettingsCtrl();
            LscLogSettings = new Controls.LogSettings.LogSettingsCtrl();
            RscResolutionSettings = new Controls.ResolutionSettings.ResolutionSettingsCtrl();
            FscFolderSettings = new Controls.FolderSettings.FolderSettingsCtrl();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            tableLayoutPanel3 = new TableLayoutPanel();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.Location = new Point(325, 478);
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
            BtnOkay.Location = new Point(230, 478);
            BtnOkay.Margin = new Padding(4, 3, 4, 3);
            BtnOkay.Name = "BtnOkay";
            BtnOkay.Size = new Size(88, 27);
            BtnOkay.TabIndex = 0;
            BtnOkay.Text = "&OK";
            BtnOkay.UseVisualStyleBackColor = true;
            BtnOkay.Click += BtnOkay_Click;
            // 
            // CscComparisonSettings
            // 
            CscComparisonSettings.Dock = DockStyle.Fill;
            CscComparisonSettings.Location = new Point(3, 3);
            CscComparisonSettings.Name = "CscComparisonSettings";
            CscComparisonSettings.Size = new Size(186, 66);
            CscComparisonSettings.TabIndex = 28;
            CscComparisonSettings.TryComparisonClick += CscComparisonSettings_TryComparisonClick;
            // 
            // LscLogSettings
            // 
            LscLogSettings.Dock = DockStyle.Fill;
            LscLogSettings.Location = new Point(3, 216);
            LscLogSettings.Name = "LscLogSettings";
            LscLogSettings.Size = new Size(382, 207);
            LscLogSettings.TabIndex = 29;
            // 
            // RscResolutionSettings
            // 
            RscResolutionSettings.Dock = DockStyle.Fill;
            RscResolutionSettings.Location = new Point(3, 3);
            RscResolutionSettings.Name = "RscResolutionSettings";
            RscResolutionSettings.Size = new Size(382, 207);
            RscResolutionSettings.TabIndex = 30;
            // 
            // FscFolderSettings
            // 
            FscFolderSettings.Dock = DockStyle.Fill;
            FscFolderSettings.Location = new Point(3, 3);
            FscFolderSettings.Name = "FscFolderSettings";
            FscFolderSettings.Size = new Size(388, 426);
            FscFolderSettings.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(402, 460);
            tabControl1.TabIndex = 28;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(FscFolderSettings);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(394, 432);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Video Input";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(CscComparisonSettings);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(192, 72);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Comparison";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(tableLayoutPanel3);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(394, 432);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Duplicates and Logging";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(LscLogSettings, 0, 1);
            tableLayoutPanel3.Controls.Add(RscResolutionSettings, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(388, 426);
            tableLayoutPanel3.TabIndex = 31;
            // 
            // ServerConfigDlg
            // 
            AcceptButton = BtnOkay;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = BtnCancel;
            ClientSize = new Size(426, 519);
            Controls.Add(tabControl1);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOkay);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "ServerConfigDlg";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Server Configuration";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button BtnCancel;
        private Button BtnOkay;
        private Controls.FolderSettings.FolderSettingsCtrl FscFolderSettings;
        private Controls.ComparisonSettings.ComparisonSettingsCtrl CscComparisonSettings;
        private Controls.LogSettings.LogSettingsCtrl LscLogSettings;
        private Controls.ResolutionSettings.ResolutionSettingsCtrl RscResolutionSettings;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TableLayoutPanel tableLayoutPanel3;
    }
}
