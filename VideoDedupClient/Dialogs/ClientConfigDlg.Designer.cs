namespace VideoDedupClient.Dialogs
{
    using System.ComponentModel;
    using Controls.DnsTextBox;

    partial class ClientConfigDlg
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
            var resources = new ComponentResourceManager(typeof(ClientConfigDlg));
            BtnCancel = new Button();
            BtnOkay = new Button();
            groupBox1 = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel7 = new TableLayoutPanel();
            label7 = new Label();
            TxtPinnedCertificatePath = new TextBox();
            BtnBrowsePinnedCertificate = new Button();
            label4 = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            PibServerAddressLoading = new PictureBox();
            TxtServerAddress = new DnsTextBoxCtrl();
            label1 = new Label();
            tableLayoutPanel4 = new TableLayoutPanel();
            label5 = new Label();
            label6 = new Label();
            CmbProtocol = new ComboBox();
            NudPort = new NumericUpDown();
            tableLayoutPanel5 = new TableLayoutPanel();
            label2 = new Label();
            NudStatusRequestInterval = new NumericUpDown();
            tableLayoutPanel6 = new TableLayoutPanel();
            TxtClientSourcePath = new TextBox();
            label3 = new Label();
            BtnSelectClientSourcePath = new Button();
            groupBox1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((ISupportInitialize)PibServerAddressLoading).BeginInit();
            tableLayoutPanel4.SuspendLayout();
            ((ISupportInitialize)NudPort).BeginInit();
            tableLayoutPanel5.SuspendLayout();
            ((ISupportInitialize)NudStatusRequestInterval).BeginInit();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            SuspendLayout();
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.Location = new Point(402, 360);
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
            BtnOkay.Location = new Point(307, 360);
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
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(tableLayoutPanel2);
            groupBox1.Location = new Point(14, 14);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(475, 335);
            groupBox1.TabIndex = 24;
            groupBox1.TabStop = false;
            groupBox1.Text = "Server Connection";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Controls.Add(label4, 0, 5);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel7, 0, 4);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel4, 0, 1);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel5, 0, 2);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel6, 0, 3);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(4, 19);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 6;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 32F));
            tableLayoutPanel2.Size = new Size(467, 310);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(4, 232);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(459, 30);
            label4.TabIndex = 21;
            label4.Text = "Set the path to the Source Directory from Clients point of view. If Server and Client are on the same system, you can leave this empty to use the same path.";
            label4.TextAlign = ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 3;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel7.Controls.Add(TxtPinnedCertificatePath, 1, 0);
            tableLayoutPanel7.Controls.Add(label7, 0, 0);
            tableLayoutPanel7.Controls.Add(BtnBrowsePinnedCertificate, 2, 0);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(0, 220);
            tableLayoutPanel7.Margin = new Padding(0);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 1;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.Size = new Size(467, 48);
            tableLayoutPanel7.TabIndex = 4;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(4, 16);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(97, 15);
            label7.TabIndex = 0;
            label7.Text = "Server certificate:";
            // 
            // TxtPinnedCertificatePath
            // 
            TxtPinnedCertificatePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TxtPinnedCertificatePath.Location = new Point(109, 12);
            TxtPinnedCertificatePath.Margin = new Padding(4, 3, 4, 3);
            TxtPinnedCertificatePath.Name = "TxtPinnedCertificatePath";
            TxtPinnedCertificatePath.PlaceholderText = "Leave empty to use install folder cert";
            TxtPinnedCertificatePath.Size = new Size(322, 23);
            TxtPinnedCertificatePath.TabIndex = 1;
            // 
            // BtnBrowsePinnedCertificate
            // 
            BtnBrowsePinnedCertificate.Anchor = AnchorStyles.Right;
            BtnBrowsePinnedCertificate.Location = new Point(439, 10);
            BtnBrowsePinnedCertificate.Margin = new Padding(4, 3, 4, 3);
            BtnBrowsePinnedCertificate.Name = "BtnBrowsePinnedCertificate";
            BtnBrowsePinnedCertificate.Size = new Size(24, 27);
            BtnBrowsePinnedCertificate.TabIndex = 2;
            BtnBrowsePinnedCertificate.Text = "...";
            BtnBrowsePinnedCertificate.UseVisualStyleBackColor = true;
            BtnBrowsePinnedCertificate.Click += BtnBrowsePinnedCertificate_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.Controls.Add(PibServerAddressLoading, 2, 0);
            tableLayoutPanel3.Controls.Add(TxtServerAddress, 1, 0);
            tableLayoutPanel3.Controls.Add(label1, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(467, 55);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // PibServerAddressLoading
            // 
            PibServerAddressLoading.Anchor = AnchorStyles.Left;
            PibServerAddressLoading.InitialImage = null;
            PibServerAddressLoading.Location = new Point(426, 11);
            PibServerAddressLoading.Margin = new Padding(4, 3, 4, 3);
            PibServerAddressLoading.Name = "PibServerAddressLoading";
            PibServerAddressLoading.Size = new Size(37, 33);
            PibServerAddressLoading.SizeMode = PictureBoxSizeMode.Zoom;
            PibServerAddressLoading.TabIndex = 2;
            PibServerAddressLoading.TabStop = false;
            // 
            // TxtServerAddress
            // 
            TxtServerAddress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TxtServerAddress.Location = new Point(99, 16);
            TxtServerAddress.Margin = new Padding(4, 3, 4, 3);
            TxtServerAddress.Name = "TxtServerAddress";
            TxtServerAddress.ResolvedSuccessfully = false;
            TxtServerAddress.Resolving = false;
            TxtServerAddress.Size = new Size(319, 23);
            TxtServerAddress.TabIndex = 0;
            TxtServerAddress.ResolveStarted += TxtServerAddress_ResolveStarted;
            TxtServerAddress.ResolveSuccessful += TxtServerAddress_ResolveSuccessful;
            TxtServerAddress.ResolveFailed += TxtServerAddress_ResolveFailed;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(4, 20);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 0;
            label1.Text = "Server Address:";
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 4;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.Controls.Add(label5, 0, 0);
            tableLayoutPanel4.Controls.Add(label6, 2, 0);
            tableLayoutPanel4.Controls.Add(CmbProtocol, 1, 0);
            tableLayoutPanel4.Controls.Add(NudPort, 3, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(0, 55);
            tableLayoutPanel4.Margin = new Padding(0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(467, 55);
            tableLayoutPanel4.TabIndex = 1;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(58, 20);
            label5.Name = "label5";
            label5.Size = new Size(55, 15);
            label5.TabIndex = 0;
            label5.Text = "Protocol:";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(313, 20);
            label6.Name = "label6";
            label6.Size = new Size(32, 15);
            label6.TabIndex = 1;
            label6.Text = "Port:";
            // 
            // CmbProtocol
            // 
            CmbProtocol.Anchor = AnchorStyles.Left;
            CmbProtocol.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbProtocol.FormattingEnabled = true;
            CmbProtocol.Items.AddRange(new object[] { "https", "http" });
            CmbProtocol.Location = new Point(119, 16);
            CmbProtocol.Name = "CmbProtocol";
            CmbProtocol.Size = new Size(110, 23);
            CmbProtocol.TabIndex = 2;
            // 
            // NudPort
            // 
            NudPort.Anchor = AnchorStyles.Left;
            NudPort.Location = new Point(351, 16);
            NudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NudPort.Name = "NudPort";
            NudPort.Size = new Size(91, 23);
            NudPort.TabIndex = 3;
            NudPort.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(label2, 0, 0);
            tableLayoutPanel5.Controls.Add(NudStatusRequestInterval, 1, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(0, 110);
            tableLayoutPanel5.Margin = new Padding(0);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel5.Size = new Size(467, 55);
            tableLayoutPanel5.TabIndex = 2;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(18, 20);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(211, 15);
            label2.TabIndex = 0;
            label2.Text = "Status Request Interval in milliseconds:";
            // 
            // NudStatusRequestInterval
            // 
            NudStatusRequestInterval.Anchor = AnchorStyles.Left;
            NudStatusRequestInterval.Location = new Point(237, 16);
            NudStatusRequestInterval.Margin = new Padding(4, 3, 4, 3);
            NudStatusRequestInterval.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            NudStatusRequestInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            NudStatusRequestInterval.Name = "NudStatusRequestInterval";
            NudStatusRequestInterval.Size = new Size(68, 23);
            NudStatusRequestInterval.TabIndex = 1;
            NudStatusRequestInterval.Value = new decimal(new int[] { 10000, 0, 0, 0 });
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 3;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel6.Controls.Add(TxtClientSourcePath, 1, 0);
            tableLayoutPanel6.Controls.Add(label3, 0, 0);
            tableLayoutPanel6.Controls.Add(BtnSelectClientSourcePath, 2, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(0, 165);
            tableLayoutPanel6.Margin = new Padding(0);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 1;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Size = new Size(467, 55);
            tableLayoutPanel6.TabIndex = 3;
            // 
            // TxtClientSourcePath
            // 
            TxtClientSourcePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TxtClientSourcePath.Location = new Point(109, 16);
            TxtClientSourcePath.Margin = new Padding(4, 3, 4, 3);
            TxtClientSourcePath.Name = "TxtClientSourcePath";
            TxtClientSourcePath.Size = new Size(322, 23);
            TxtClientSourcePath.TabIndex = 2;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(4, 12);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(97, 30);
            label3.TabIndex = 0;
            label3.Text = "Client-Side\r\nSource Directory:";
            // 
            // BtnSelectClientSourcePath
            // 
            BtnSelectClientSourcePath.Anchor = AnchorStyles.Right;
            BtnSelectClientSourcePath.Location = new Point(439, 14);
            BtnSelectClientSourcePath.Margin = new Padding(4, 3, 4, 3);
            BtnSelectClientSourcePath.Name = "BtnSelectClientSourcePath";
            BtnSelectClientSourcePath.Size = new Size(24, 27);
            BtnSelectClientSourcePath.TabIndex = 3;
            BtnSelectClientSourcePath.Text = "...";
            BtnSelectClientSourcePath.UseVisualStyleBackColor = true;
            BtnSelectClientSourcePath.Click += BtnSelectClientSourcePath_Click;
            // 
            // ClientConfigDlg
            // 
            AcceptButton = BtnOkay;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = BtnCancel;
            ClientSize = new Size(503, 400);
            Controls.Add(groupBox1);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOkay);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "ClientConfigDlg";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Client Configuration";
            groupBox1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ((ISupportInitialize)PibServerAddressLoading).EndInit();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            ((ISupportInitialize)NudPort).EndInit();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ((ISupportInitialize)NudStatusRequestInterval).EndInit();
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private Button BtnCancel;
        private Button BtnOkay;
        private GroupBox groupBox1;
        private Label label1;
        private Label label2;
        private PictureBox PibServerAddressLoading;
        private DnsTextBoxCtrl TxtServerAddress;
        private NumericUpDown NudStatusRequestInterval;
        private TextBox TxtClientSourcePath;
        private Label label3;
        private Button BtnSelectClientSourcePath;
        private Label label4;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label5;
        private Label label6;
        private ComboBox CmbProtocol;
        private NumericUpDown NudPort;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel7;
        private Label label7;
        private TextBox TxtPinnedCertificatePath;
        private Button BtnBrowsePinnedCertificate;
    }
}
