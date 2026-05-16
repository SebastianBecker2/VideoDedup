namespace SetupBootstrapperUI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.PnlHeader = new System.Windows.Forms.Panel();
            this.LblStep = new System.Windows.Forms.Label();
            this.LblHeaderSubtitle = new System.Windows.Forms.Label();
            this.LblHeaderTitle = new System.Windows.Forms.Label();
            this.PnlFooter = new System.Windows.Forms.Panel();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnNext = new System.Windows.Forms.Button();
            this.BtnBack = new System.Windows.Forms.Button();
            this.PnlContent = new System.Windows.Forms.Panel();
            this.PnlConnectivity = new System.Windows.Forms.Panel();
            this.ClbNetworkBindings = new System.Windows.Forms.CheckedListBox();
            this.LblConnectivityNetworks = new System.Windows.Forms.Label();
            this.LblConnectivityPortHint = new System.Windows.Forms.Label();
            this.NudServerPort = new System.Windows.Forms.NumericUpDown();
            this.LblConnectivityPort = new System.Windows.Forms.Label();
            this.LblConnectivityHelp = new System.Windows.Forms.Label();
            this.PnlSelection = new System.Windows.Forms.Panel();
            this.LblReview = new System.Windows.Forms.Label();
            this.LblComponents = new System.Windows.Forms.Label();
            this.LblMode = new System.Windows.Forms.Label();
            this.RdoUninstall = new System.Windows.Forms.RadioButton();
            this.RdoUpdate = new System.Windows.Forms.RadioButton();
            this.RdoInstall = new System.Windows.Forms.RadioButton();
            this.BtnMaintenanceExportCert = new System.Windows.Forms.Button();
            this.BtnMaintenanceImportCert = new System.Windows.Forms.Button();
            this.LblMaintenanceHint = new System.Windows.Forms.Label();
            this.ChbClient = new System.Windows.Forms.CheckBox();
            this.ChbServer = new System.Windows.Forms.CheckBox();
            this.PnlClientCertPicker = new System.Windows.Forms.Panel();
            this.LblClientCertThumbPreview = new System.Windows.Forms.Label();
            this.BtnClientCertBrowse = new System.Windows.Forms.Button();
            this.TxtClientCertPath = new System.Windows.Forms.TextBox();
            this.LblClientCertHelp = new System.Windows.Forms.Label();
            this.PnlServerCertExport = new System.Windows.Forms.Panel();
            this.BtnCopyThumbprint = new System.Windows.Forms.Button();
            this.LblServerCertThumbprint = new System.Windows.Forms.Label();
            this.BtnSaveCertCopy = new System.Windows.Forms.Button();
            this.BtnOpenCertFolder = new System.Windows.Forms.Button();
            this.TxtServerCertPath = new System.Windows.Forms.TextBox();
            this.LblServerCertHelp = new System.Windows.Forms.Label();
            this.PnlProgress = new System.Windows.Forms.Panel();
            this.LblStatus = new System.Windows.Forms.Label();
            this.PrgProgress = new System.Windows.Forms.ProgressBar();
            this.PnlComplete = new System.Windows.Forms.Panel();
            this.LblResult = new System.Windows.Forms.Label();
            this.PnlHeader.SuspendLayout();
            this.PnlFooter.SuspendLayout();
            this.PnlContent.SuspendLayout();
            this.PnlConnectivity.SuspendLayout();
            this.PnlSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudServerPort)).BeginInit();
            this.PnlClientCertPicker.SuspendLayout();
            this.PnlServerCertExport.SuspendLayout();
            this.PnlProgress.SuspendLayout();
            this.PnlComplete.SuspendLayout();
            this.SuspendLayout();
            // 
            // PnlHeader
            // 
            this.PnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(88)))), ((int)(((byte)(145)))));
            this.PnlHeader.Controls.Add(this.LblStep);
            this.PnlHeader.Controls.Add(this.LblHeaderSubtitle);
            this.PnlHeader.Controls.Add(this.LblHeaderTitle);
            this.PnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.PnlHeader.Location = new System.Drawing.Point(0, 0);
            this.PnlHeader.Name = "PnlHeader";
            this.PnlHeader.Size = new System.Drawing.Size(651, 85);
            this.PnlHeader.TabIndex = 0;
            // 
            // LblStep
            // 
            this.LblStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblStep.ForeColor = System.Drawing.Color.White;
            this.LblStep.Location = new System.Drawing.Point(470, 12);
            this.LblStep.Name = "LblStep";
            this.LblStep.Size = new System.Drawing.Size(171, 17);
            this.LblStep.TabIndex = 2;
            this.LblStep.Text = "Step 1 of 4";
            this.LblStep.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // LblHeaderSubtitle
            // 
            this.LblHeaderSubtitle.ForeColor = System.Drawing.Color.White;
            this.LblHeaderSubtitle.Location = new System.Drawing.Point(21, 42);
            this.LblHeaderSubtitle.Name = "LblHeaderSubtitle";
            this.LblHeaderSubtitle.Size = new System.Drawing.Size(600, 33);
            this.LblHeaderSubtitle.TabIndex = 1;
            this.LblHeaderSubtitle.Text = "Choose what you want to install. The most common option is Client + Server.";
            // 
            // LblHeaderTitle
            // 
            this.LblHeaderTitle.AutoSize = true;
            this.LblHeaderTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblHeaderTitle.ForeColor = System.Drawing.Color.White;
            this.LblHeaderTitle.Location = new System.Drawing.Point(19, 12);
            this.LblHeaderTitle.Name = "LblHeaderTitle";
            this.LblHeaderTitle.Size = new System.Drawing.Size(167, 25);
            this.LblHeaderTitle.TabIndex = 0;
            this.LblHeaderTitle.Text = "Welcome to setup";
            // 
            // PnlFooter
            // 
            this.PnlFooter.Controls.Add(this.BtnCancel);
            this.PnlFooter.Controls.Add(this.BtnNext);
            this.PnlFooter.Controls.Add(this.BtnBack);
            this.PnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlFooter.Location = new System.Drawing.Point(0, 407);
            this.PnlFooter.Name = "PnlFooter";
            this.PnlFooter.Size = new System.Drawing.Size(651, 54);
            this.PnlFooter.TabIndex = 2;
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.Location = new System.Drawing.Point(567, 16);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 26);
            this.BtnCancel.TabIndex = 2;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnNext
            // 
            this.BtnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnNext.Location = new System.Drawing.Point(487, 16);
            this.BtnNext.Name = "BtnNext";
            this.BtnNext.Size = new System.Drawing.Size(75, 26);
            this.BtnNext.TabIndex = 1;
            this.BtnNext.Text = "Next >";
            this.BtnNext.UseVisualStyleBackColor = true;
            this.BtnNext.Click += new System.EventHandler(this.BtnNext_Click);
            // 
            // BtnBack
            // 
            this.BtnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBack.Location = new System.Drawing.Point(407, 16);
            this.BtnBack.Name = "BtnBack";
            this.BtnBack.Size = new System.Drawing.Size(75, 26);
            this.BtnBack.TabIndex = 0;
            this.BtnBack.Text = "< Back";
            this.BtnBack.UseVisualStyleBackColor = true;
            this.BtnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // PnlContent
            // 
            this.PnlContent.BackColor = System.Drawing.Color.White;
            this.PnlContent.Controls.Add(this.PnlSelection);
            this.PnlContent.Controls.Add(this.PnlConnectivity);
            this.PnlContent.Controls.Add(this.PnlClientCertPicker);
            this.PnlContent.Controls.Add(this.PnlServerCertExport);
            this.PnlContent.Controls.Add(this.PnlProgress);
            this.PnlContent.Controls.Add(this.PnlComplete);
            this.PnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlContent.Location = new System.Drawing.Point(0, 85);
            this.PnlContent.Name = "PnlContent";
            this.PnlContent.Padding = new System.Windows.Forms.Padding(17, 16, 17, 16);
            this.PnlContent.Size = new System.Drawing.Size(651, 322);
            this.PnlContent.TabIndex = 1;
            // 
            // PnlSelection
            // 
            this.PnlSelection.Controls.Add(this.LblReview);
            this.PnlSelection.Controls.Add(this.LblComponents);
            this.PnlSelection.Controls.Add(this.LblMode);
            this.PnlSelection.Controls.Add(this.RdoUninstall);
            this.PnlSelection.Controls.Add(this.RdoUpdate);
            this.PnlSelection.Controls.Add(this.RdoInstall);
            this.PnlSelection.Controls.Add(this.BtnMaintenanceExportCert);
            this.PnlSelection.Controls.Add(this.BtnMaintenanceImportCert);
            this.PnlSelection.Controls.Add(this.LblMaintenanceHint);
            this.PnlSelection.Controls.Add(this.ChbClient);
            this.PnlSelection.Controls.Add(this.ChbServer);
            this.PnlSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlSelection.Location = new System.Drawing.Point(17, 16);
            this.PnlSelection.Name = "PnlSelection";
            this.PnlSelection.Size = new System.Drawing.Size(617, 290);
            this.PnlSelection.TabIndex = 0;
            // 
            // LblReview
            // 
            this.LblReview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblReview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.LblReview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblReview.Location = new System.Drawing.Point(21, 184);
            this.LblReview.Name = "LblReview";
            this.LblReview.Padding = new System.Windows.Forms.Padding(9, 9, 9, 9);
            this.LblReview.Size = new System.Drawing.Size(576, 63);
            this.LblReview.TabIndex = 10;
            this.LblReview.Text = "Review";
            // 
            // LblComponents
            // 
            this.LblComponents.AutoSize = true;
            this.LblComponents.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.LblComponents.Location = new System.Drawing.Point(17, 89);
            this.LblComponents.Name = "LblComponents";
            this.LblComponents.Size = new System.Drawing.Size(161, 19);
            this.LblComponents.TabIndex = 9;
            this.LblComponents.Text = "2) Choose components";
            // 
            // LblMode
            // 
            this.LblMode.AutoSize = true;
            this.LblMode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.LblMode.Location = new System.Drawing.Point(17, 7);
            this.LblMode.Name = "LblMode";
            this.LblMode.Size = new System.Drawing.Size(149, 19);
            this.LblMode.TabIndex = 8;
            this.LblMode.Text = "1) Choose task mode";
            // 
            // RdoUninstall
            // 
            this.RdoUninstall.AutoSize = true;
            this.RdoUninstall.Location = new System.Drawing.Point(21, 68);
            this.RdoUninstall.Name = "RdoUninstall";
            this.RdoUninstall.Size = new System.Drawing.Size(216, 17);
            this.RdoUninstall.TabIndex = 2;
            this.RdoUninstall.TabStop = true;
            this.RdoUninstall.Text = "Remove VideoDedup from this computer";
            this.RdoUninstall.UseVisualStyleBackColor = true;
            this.RdoUninstall.CheckedChanged += new System.EventHandler(this.SelectionChanged);
            // 
            // RdoUpdate
            // 
            this.RdoUpdate.AutoSize = true;
            this.RdoUpdate.Location = new System.Drawing.Point(21, 48);
            this.RdoUpdate.Name = "RdoUpdate";
            this.RdoUpdate.Size = new System.Drawing.Size(208, 17);
            this.RdoUpdate.TabIndex = 1;
            this.RdoUpdate.TabStop = true;
            this.RdoUpdate.Text = "Change or repair an existing installation";
            this.RdoUpdate.UseVisualStyleBackColor = true;
            this.RdoUpdate.CheckedChanged += new System.EventHandler(this.SelectionChanged);
            // 
            // RdoInstall
            // 
            this.RdoInstall.AutoSize = true;
            this.RdoInstall.Location = new System.Drawing.Point(21, 29);
            this.RdoInstall.Name = "RdoInstall";
            this.RdoInstall.Size = new System.Drawing.Size(229, 17);
            this.RdoInstall.TabIndex = 0;
            this.RdoInstall.TabStop = true;
            this.RdoInstall.Text = "Install VideoDedup for the first time (default)";
            this.RdoInstall.UseVisualStyleBackColor = true;
            this.RdoInstall.CheckedChanged += new System.EventHandler(this.SelectionChanged);
            // 
            // BtnMaintenanceExportCert
            // 
            this.BtnMaintenanceExportCert.Location = new System.Drawing.Point(189, 250);
            this.BtnMaintenanceExportCert.Name = "BtnMaintenanceExportCert";
            this.BtnMaintenanceExportCert.Size = new System.Drawing.Size(196, 26);
            this.BtnMaintenanceExportCert.TabIndex = 7;
            this.BtnMaintenanceExportCert.Text = "Show server certificate details";
            this.BtnMaintenanceExportCert.UseVisualStyleBackColor = true;
            this.BtnMaintenanceExportCert.Visible = false;
            this.BtnMaintenanceExportCert.Click += new System.EventHandler(this.BtnMaintenanceExportCert_Click);
            // 
            // BtnMaintenanceImportCert
            // 
            this.BtnMaintenanceImportCert.Location = new System.Drawing.Point(21, 250);
            this.BtnMaintenanceImportCert.Name = "BtnMaintenanceImportCert";
            this.BtnMaintenanceImportCert.Size = new System.Drawing.Size(163, 26);
            this.BtnMaintenanceImportCert.TabIndex = 6;
            this.BtnMaintenanceImportCert.Text = "Import server certificate";
            this.BtnMaintenanceImportCert.UseVisualStyleBackColor = true;
            this.BtnMaintenanceImportCert.Visible = false;
            this.BtnMaintenanceImportCert.Click += new System.EventHandler(this.BtnMaintenanceImportCert_Click);
            // 
            // LblMaintenanceHint
            // 
            this.LblMaintenanceHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblMaintenanceHint.Location = new System.Drawing.Point(21, 151);
            this.LblMaintenanceHint.Name = "LblMaintenanceHint";
            this.LblMaintenanceHint.Size = new System.Drawing.Size(576, 29);
            this.LblMaintenanceHint.TabIndex = 5;
            this.LblMaintenanceHint.Text = "Maintenance tools: if your client cannot connect yet, import the server certifica" +
    "te. You can also view and copy server certificate details.";
            this.LblMaintenanceHint.Visible = false;
            // 
            // ChbClient
            // 
            this.ChbClient.AutoSize = true;
            this.ChbClient.Checked = true;
            this.ChbClient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChbClient.Location = new System.Drawing.Point(34, 113);
            this.ChbClient.Name = "ChbClient";
            this.ChbClient.Size = new System.Drawing.Size(215, 17);
            this.ChbClient.TabIndex = 3;
            this.ChbClient.Text = "Client app (use this PC to browse media)";
            this.ChbClient.UseVisualStyleBackColor = true;
            this.ChbClient.CheckedChanged += new System.EventHandler(this.SelectionChanged);
            // 
            // ChbServer
            // 
            this.ChbServer.AutoSize = true;
            this.ChbServer.Checked = true;
            this.ChbServer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChbServer.Location = new System.Drawing.Point(34, 131);
            this.ChbServer.Name = "ChbServer";
            this.ChbServer.Size = new System.Drawing.Size(287, 17);
            this.ChbServer.TabIndex = 4;
            this.ChbServer.Text = "Server service (host media index and trusted certificate)";
            this.ChbServer.UseVisualStyleBackColor = true;
            this.ChbServer.CheckedChanged += new System.EventHandler(this.SelectionChanged);
            // 
            // PnlConnectivity
            // 
            this.PnlConnectivity.Controls.Add(this.ClbNetworkBindings);
            this.PnlConnectivity.Controls.Add(this.LblConnectivityNetworks);
            this.PnlConnectivity.Controls.Add(this.LblConnectivityPortHint);
            this.PnlConnectivity.Controls.Add(this.NudServerPort);
            this.PnlConnectivity.Controls.Add(this.LblConnectivityPort);
            this.PnlConnectivity.Controls.Add(this.LblConnectivityHelp);
            this.PnlConnectivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlConnectivity.Location = new System.Drawing.Point(17, 16);
            this.PnlConnectivity.Name = "PnlConnectivity";
            this.PnlConnectivity.Size = new System.Drawing.Size(617, 290);
            this.PnlConnectivity.TabIndex = 5;
            this.PnlConnectivity.Visible = false;
            // 
            // LblConnectivityHelp
            // 
            this.LblConnectivityHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblConnectivityHelp.Location = new System.Drawing.Point(21, 17);
            this.LblConnectivityHelp.Name = "LblConnectivityHelp";
            this.LblConnectivityHelp.Size = new System.Drawing.Size(576, 36);
            this.LblConnectivityHelp.TabIndex = 0;
            this.LblConnectivityHelp.Text = "Choose the TCP port and network addresses the server will listen on. Remote client" +
    "s must use a reachable address and the same port.";
            // 
            // LblConnectivityPort
            // 
            this.LblConnectivityPort.AutoSize = true;
            this.LblConnectivityPort.Location = new System.Drawing.Point(21, 60);
            this.LblConnectivityPort.Name = "LblConnectivityPort";
            this.LblConnectivityPort.Size = new System.Drawing.Size(124, 15);
            this.LblConnectivityPort.TabIndex = 1;
            this.LblConnectivityPort.Text = "Server listen port (TCP)";
            // 
            // NudServerPort
            // 
            this.NudServerPort.Location = new System.Drawing.Point(220, 57);
            this.NudServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NudServerPort.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.NudServerPort.Name = "NudServerPort";
            this.NudServerPort.Size = new System.Drawing.Size(86, 23);
            this.NudServerPort.TabIndex = 2;
            this.NudServerPort.Value = new decimal(new int[] {
            51726,
            0,
            0,
            0});
            // 
            // LblConnectivityPortHint
            // 
            this.LblConnectivityPortHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblConnectivityPortHint.Location = new System.Drawing.Point(21, 84);
            this.LblConnectivityPortHint.Name = "LblConnectivityPortHint";
            this.LblConnectivityPortHint.Size = new System.Drawing.Size(576, 18);
            this.LblConnectivityPortHint.TabIndex = 3;
            this.LblConnectivityPortHint.Text = "Leave at 51726 unless you need a different port.";
            // 
            // LblConnectivityNetworks
            // 
            this.LblConnectivityNetworks.AutoSize = true;
            this.LblConnectivityNetworks.Location = new System.Drawing.Point(21, 110);
            this.LblConnectivityNetworks.Name = "LblConnectivityNetworks";
            this.LblConnectivityNetworks.Size = new System.Drawing.Size(99, 15);
            this.LblConnectivityNetworks.TabIndex = 4;
            this.LblConnectivityNetworks.Text = "Listen on networks";
            // 
            // ClbNetworkBindings
            // 
            this.ClbNetworkBindings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClbNetworkBindings.CheckOnClick = true;
            this.ClbNetworkBindings.FormattingEnabled = true;
            this.ClbNetworkBindings.Location = new System.Drawing.Point(21, 130);
            this.ClbNetworkBindings.Name = "ClbNetworkBindings";
            this.ClbNetworkBindings.Size = new System.Drawing.Size(576, 139);
            this.ClbNetworkBindings.TabIndex = 5;
            this.ClbNetworkBindings.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ClbNetworkBindings_ItemCheck);
            // 
            // PnlClientCertPicker
            // 
            this.PnlClientCertPicker.Controls.Add(this.LblClientCertThumbPreview);
            this.PnlClientCertPicker.Controls.Add(this.BtnClientCertBrowse);
            this.PnlClientCertPicker.Controls.Add(this.TxtClientCertPath);
            this.PnlClientCertPicker.Controls.Add(this.LblClientCertHelp);
            this.PnlClientCertPicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlClientCertPicker.Location = new System.Drawing.Point(17, 16);
            this.PnlClientCertPicker.Name = "PnlClientCertPicker";
            this.PnlClientCertPicker.Size = new System.Drawing.Size(617, 290);
            this.PnlClientCertPicker.TabIndex = 1;
            this.PnlClientCertPicker.Visible = false;
            // 
            // LblClientCertThumbPreview
            // 
            this.LblClientCertThumbPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblClientCertThumbPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.LblClientCertThumbPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblClientCertThumbPreview.Location = new System.Drawing.Point(21, 134);
            this.LblClientCertThumbPreview.Name = "LblClientCertThumbPreview";
            this.LblClientCertThumbPreview.Padding = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.LblClientCertThumbPreview.Size = new System.Drawing.Size(576, 48);
            this.LblClientCertThumbPreview.TabIndex = 3;
            this.LblClientCertThumbPreview.Text = "Thumbprint: —";
            // 
            // BtnClientCertBrowse
            // 
            this.BtnClientCertBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClientCertBrowse.Location = new System.Drawing.Point(519, 97);
            this.BtnClientCertBrowse.Name = "BtnClientCertBrowse";
            this.BtnClientCertBrowse.Size = new System.Drawing.Size(77, 24);
            this.BtnClientCertBrowse.TabIndex = 2;
            this.BtnClientCertBrowse.Text = "Browse...";
            this.BtnClientCertBrowse.UseVisualStyleBackColor = true;
            this.BtnClientCertBrowse.Click += new System.EventHandler(this.BtnClientCertBrowse_Click);
            // 
            // TxtClientCertPath
            // 
            this.TxtClientCertPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtClientCertPath.Location = new System.Drawing.Point(21, 101);
            this.TxtClientCertPath.Name = "TxtClientCertPath";
            this.TxtClientCertPath.Size = new System.Drawing.Size(494, 20);
            this.TxtClientCertPath.TabIndex = 1;
            this.TxtClientCertPath.TextChanged += new System.EventHandler(this.TxtClientCertPath_TextChanged);
            // 
            // LblClientCertHelp
            // 
            this.LblClientCertHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblClientCertHelp.Location = new System.Drawing.Point(21, 17);
            this.LblClientCertHelp.Name = "LblClientCertHelp";
            this.LblClientCertHelp.Size = new System.Drawing.Size(576, 69);
            this.LblClientCertHelp.TabIndex = 0;
            this.LblClientCertHelp.Text = "If this computer is a Client only setup, choose VideoDedup.crt copied from your s" +
    "erver PC (USB drive or network share). If unsure, you can continue without one a" +
    "nd import it later.";
            // 
            // PnlServerCertExport
            // 
            this.PnlServerCertExport.Controls.Add(this.BtnCopyThumbprint);
            this.PnlServerCertExport.Controls.Add(this.LblServerCertThumbprint);
            this.PnlServerCertExport.Controls.Add(this.BtnSaveCertCopy);
            this.PnlServerCertExport.Controls.Add(this.BtnOpenCertFolder);
            this.PnlServerCertExport.Controls.Add(this.TxtServerCertPath);
            this.PnlServerCertExport.Controls.Add(this.LblServerCertHelp);
            this.PnlServerCertExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlServerCertExport.Location = new System.Drawing.Point(17, 16);
            this.PnlServerCertExport.Name = "PnlServerCertExport";
            this.PnlServerCertExport.Size = new System.Drawing.Size(617, 290);
            this.PnlServerCertExport.TabIndex = 2;
            this.PnlServerCertExport.Visible = false;
            // 
            // BtnCopyThumbprint
            // 
            this.BtnCopyThumbprint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCopyThumbprint.Location = new System.Drawing.Point(519, 199);
            this.BtnCopyThumbprint.Name = "BtnCopyThumbprint";
            this.BtnCopyThumbprint.Size = new System.Drawing.Size(77, 24);
            this.BtnCopyThumbprint.TabIndex = 4;
            this.BtnCopyThumbprint.Text = "Copy";
            this.BtnCopyThumbprint.UseVisualStyleBackColor = true;
            this.BtnCopyThumbprint.Click += new System.EventHandler(this.BtnCopyThumbprint_Click);
            // 
            // LblServerCertThumbprint
            // 
            this.LblServerCertThumbprint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblServerCertThumbprint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.LblServerCertThumbprint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblServerCertThumbprint.Location = new System.Drawing.Point(21, 199);
            this.LblServerCertThumbprint.Name = "LblServerCertThumbprint";
            this.LblServerCertThumbprint.Padding = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.LblServerCertThumbprint.Size = new System.Drawing.Size(494, 48);
            this.LblServerCertThumbprint.TabIndex = 3;
            this.LblServerCertThumbprint.Text = "Thumbprint: —";
            // 
            // BtnSaveCertCopy
            // 
            this.BtnSaveCertCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSaveCertCopy.Location = new System.Drawing.Point(437, 159);
            this.BtnSaveCertCopy.Name = "BtnSaveCertCopy";
            this.BtnSaveCertCopy.Size = new System.Drawing.Size(159, 24);
            this.BtnSaveCertCopy.TabIndex = 2;
            this.BtnSaveCertCopy.Text = "Save copy as...";
            this.BtnSaveCertCopy.UseVisualStyleBackColor = true;
            this.BtnSaveCertCopy.Click += new System.EventHandler(this.BtnSaveCertCopy_Click);
            // 
            // BtnOpenCertFolder
            // 
            this.BtnOpenCertFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOpenCertFolder.Location = new System.Drawing.Point(355, 159);
            this.BtnOpenCertFolder.Name = "BtnOpenCertFolder";
            this.BtnOpenCertFolder.Size = new System.Drawing.Size(77, 24);
            this.BtnOpenCertFolder.TabIndex = 1;
            this.BtnOpenCertFolder.Text = "Open folder";
            this.BtnOpenCertFolder.UseVisualStyleBackColor = true;
            this.BtnOpenCertFolder.Click += new System.EventHandler(this.BtnOpenCertFolder_Click);
            // 
            // TxtServerCertPath
            // 
            this.TxtServerCertPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtServerCertPath.Location = new System.Drawing.Point(21, 111);
            this.TxtServerCertPath.Name = "TxtServerCertPath";
            this.TxtServerCertPath.ReadOnly = true;
            this.TxtServerCertPath.Size = new System.Drawing.Size(577, 20);
            this.TxtServerCertPath.TabIndex = 0;
            // 
            // LblServerCertHelp
            // 
            this.LblServerCertHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblServerCertHelp.Location = new System.Drawing.Point(21, 17);
            this.LblServerCertHelp.Name = "LblServerCertHelp";
            this.LblServerCertHelp.Size = new System.Drawing.Size(576, 83);
            this.LblServerCertHelp.TabIndex = 5;
            this.LblServerCertHelp.Text = "Server certificate ready.\r\n\r\n1) Save a copy to USB or network share.\r\n2) Install " +
    "VideoDedup Client on other PCs.\r\n3) During client setup, select this certificate" +
    " file.";
            // 
            // PnlProgress
            // 
            this.PnlProgress.Controls.Add(this.LblStatus);
            this.PnlProgress.Controls.Add(this.PrgProgress);
            this.PnlProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlProgress.Location = new System.Drawing.Point(17, 16);
            this.PnlProgress.Name = "PnlProgress";
            this.PnlProgress.Size = new System.Drawing.Size(617, 290);
            this.PnlProgress.TabIndex = 3;
            this.PnlProgress.Visible = false;
            // 
            // LblStatus
            // 
            this.LblStatus.AutoSize = true;
            this.LblStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.LblStatus.Location = new System.Drawing.Point(21, 31);
            this.LblStatus.Name = "LblStatus";
            this.LblStatus.Size = new System.Drawing.Size(73, 19);
            this.LblStatus.TabIndex = 0;
            this.LblStatus.Text = "Installing...";
            // 
            // PrgProgress
            // 
            this.PrgProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PrgProgress.Location = new System.Drawing.Point(21, 62);
            this.PrgProgress.Name = "PrgProgress";
            this.PrgProgress.Size = new System.Drawing.Size(576, 24);
            this.PrgProgress.TabIndex = 1;
            // 
            // PnlComplete
            // 
            this.PnlComplete.Controls.Add(this.LblResult);
            this.PnlComplete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlComplete.Location = new System.Drawing.Point(17, 16);
            this.PnlComplete.Name = "PnlComplete";
            this.PnlComplete.Size = new System.Drawing.Size(617, 290);
            this.PnlComplete.TabIndex = 4;
            this.PnlComplete.Visible = false;
            // 
            // LblResult
            // 
            this.LblResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblResult.Location = new System.Drawing.Point(21, 26);
            this.LblResult.Name = "LblResult";
            this.LblResult.Size = new System.Drawing.Size(576, 240);
            this.LblResult.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AcceptButton = this.BtnNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 461);
            this.Controls.Add(this.PnlContent);
            this.Controls.Add(this.PnlFooter);
            this.Controls.Add(this.PnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VideoDedup Setup";
            this.PnlHeader.ResumeLayout(false);
            this.PnlHeader.PerformLayout();
            this.PnlFooter.ResumeLayout(false);
            this.PnlContent.ResumeLayout(false);
            this.PnlConnectivity.ResumeLayout(false);
            this.PnlConnectivity.PerformLayout();
            this.PnlSelection.ResumeLayout(false);
            this.PnlSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudServerPort)).EndInit();
            this.PnlClientCertPicker.ResumeLayout(false);
            this.PnlClientCertPicker.PerformLayout();
            this.PnlServerCertExport.ResumeLayout(false);
            this.PnlServerCertExport.PerformLayout();
            this.PnlProgress.ResumeLayout(false);
            this.PnlProgress.PerformLayout();
            this.PnlComplete.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PnlHeader;
        private System.Windows.Forms.Label LblStep;
        private System.Windows.Forms.Label LblHeaderSubtitle;
        private System.Windows.Forms.Label LblHeaderTitle;
        private System.Windows.Forms.Panel PnlFooter;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnNext;
        private System.Windows.Forms.Button BtnBack;
        private System.Windows.Forms.Panel PnlContent;
        private System.Windows.Forms.Panel PnlSelection;
        private System.Windows.Forms.Label LblReview;
        private System.Windows.Forms.Label LblComponents;
        private System.Windows.Forms.Label LblMode;
        private System.Windows.Forms.RadioButton RdoUninstall;
        private System.Windows.Forms.RadioButton RdoUpdate;
        private System.Windows.Forms.RadioButton RdoInstall;
        private System.Windows.Forms.Label LblMaintenanceHint;
        private System.Windows.Forms.Button BtnMaintenanceImportCert;
        private System.Windows.Forms.Button BtnMaintenanceExportCert;
        private System.Windows.Forms.CheckBox ChbClient;
        private System.Windows.Forms.CheckBox ChbServer;
        private System.Windows.Forms.Panel PnlConnectivity;
        private System.Windows.Forms.Label LblConnectivityHelp;
        private System.Windows.Forms.Label LblConnectivityPort;
        private System.Windows.Forms.NumericUpDown NudServerPort;
        private System.Windows.Forms.Label LblConnectivityPortHint;
        private System.Windows.Forms.Label LblConnectivityNetworks;
        private System.Windows.Forms.CheckedListBox ClbNetworkBindings;
        private System.Windows.Forms.Panel PnlClientCertPicker;
        private System.Windows.Forms.Label LblClientCertHelp;
        private System.Windows.Forms.TextBox TxtClientCertPath;
        private System.Windows.Forms.Button BtnClientCertBrowse;
        private System.Windows.Forms.Label LblClientCertThumbPreview;
        private System.Windows.Forms.Panel PnlServerCertExport;
        private System.Windows.Forms.Label LblServerCertHelp;
        private System.Windows.Forms.TextBox TxtServerCertPath;
        private System.Windows.Forms.Button BtnOpenCertFolder;
        private System.Windows.Forms.Button BtnSaveCertCopy;
        private System.Windows.Forms.Label LblServerCertThumbprint;
        private System.Windows.Forms.Button BtnCopyThumbprint;
        private System.Windows.Forms.Panel PnlProgress;
        private System.Windows.Forms.Label LblStatus;
        private System.Windows.Forms.ProgressBar PrgProgress;
        private System.Windows.Forms.Panel PnlComplete;
        private System.Windows.Forms.Label LblResult;
    }
}
