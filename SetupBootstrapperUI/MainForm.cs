namespace SetupBootstrapperUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Security.Cryptography.X509Certificates;
    using System.Windows.Forms;
    using WixToolset.BootstrapperApplicationApi;

    public partial class MainForm : Form
    {
        /// <summary>Must match <c>ServerComponent</c> in Setup/Config.wxi.</summary>
        private const string ServerFolderName = "VideoDedupServer";

        private const int DefaultServerPort = 51726;
        private const int MinServerPort = 1024;
        private const int MaxServerPort = 65535;

        private enum WizardStep
        {
            Selection,
            Connectivity,
            Certificate,
            Progress,
            Complete,
            ServerCertificate,
        }

        private enum SetupMode
        {
            Install,
            Update,
            Uninstall,
        }

        private readonly IBootstrapperUiRuntime app;
        private WizardStep currentStep;
        private SetupMode selectedMode;
        private bool isUninstall;
        private bool packageInstalled;
        private bool clientRegistryPresent;
        private bool needsClientCertificateStep;
        private bool lastApplyIncludedServer;
        private bool maintenanceClientCertFlow;
        private bool maintenanceServerExportOnly;
        private bool suppressBindingItemCheckHandler;

        public MainForm(IBootstrapperUiRuntime app)
        {
            this.app = app;
            app.AttachForm(this);
            InitializeComponent();
            currentStep = WizardStep.Selection;
            SetDetectingState();
            ApplyVisualStyle();
            RenderStep();
        }

        public void OnDetectComplete(bool packageInstalled, bool isUninstall)
        {
            this.packageInstalled = packageInstalled;
            this.isUninstall = isUninstall;
            var clientMarker = app.GetVariableString("ClientInstallFolderMarker");
            clientRegistryPresent = !string.IsNullOrWhiteSpace(clientMarker);

            if (isUninstall || packageInstalled)
            {
                selectedMode = isUninstall ? SetupMode.Uninstall : SetupMode.Update;
            }
            else
            {
                selectedMode = SetupMode.Install;
            }

            RdoInstall.Checked = selectedMode == SetupMode.Install;
            RdoUpdate.Checked = selectedMode == SetupMode.Update;
            RdoUninstall.Checked = selectedMode == SetupMode.Uninstall;

            ChbClient.Enabled = selectedMode != SetupMode.Uninstall;
            ChbServer.Enabled = selectedMode != SetupMode.Uninstall;
            if (selectedMode == SetupMode.Uninstall)
            {
                ChbClient.Checked = true;
                ChbServer.Checked = true;
            }
            else if (!packageInstalled)
            {
                ChbClient.Checked = true;
                ChbServer.Checked = true;
            }

            PrefillServerConnectivity();
            currentStep = WizardStep.Selection;
            BtnNext.Enabled = true;
            UpdateSelectionSummary();
            RenderStep();
        }

        public void OnProgress(int percentage) =>
            PrgProgress.Value = Math.Min(Math.Max(percentage, 0), 100);

        public void OnApplyComplete(bool success, string message)
        {
            if (success)
            {
                app.ClearBundleCertVariables();
            }

            if (success
                && lastApplyIncludedServer
                && !maintenanceServerExportOnly)
            {
                PopulateServerExportPanel();
                currentStep = WizardStep.ServerCertificate;
                RenderStep();
                return;
            }

            LblResult.Text = success
                ? message + Environment.NewLine + Environment.NewLine + "You can now close setup."
                : message + Environment.NewLine + Environment.NewLine + "Please review the message above and try again.";
            LblResult.ForeColor = success
                ? System.Drawing.SystemColors.ControlText
                : System.Drawing.Color.Red;
            currentStep = WizardStep.Complete;
            RenderStep();
        }

        public void OnError(string message)
        {
            if (currentStep == WizardStep.Progress)
            {
                LblStatus.Text = "Error: " + message;
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            switch (currentStep)
            {
                case WizardStep.Selection:
                    MoveForwardFromSelection();
                    break;
                case WizardStep.Connectivity:
                    MoveForwardFromConnectivity();
                    break;
                case WizardStep.Certificate:
                    FinishClientCertPicker(TxtClientCertPath.Text.Trim());
                    break;
                case WizardStep.Complete:
                case WizardStep.ServerCertificate:
                    Close();
                    break;
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            switch (currentStep)
            {
                case WizardStep.Connectivity:
                    currentStep = WizardStep.Selection;
                    RenderStep();
                    break;
                case WizardStep.Certificate:
                    currentStep = NeedsConnectivityStep()
                        ? WizardStep.Connectivity
                        : WizardStep.Selection;
                    RenderStep();
                    break;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (currentStep == WizardStep.Progress)
            {
                _ = MessageBox.Show(
                    this,
                    "Setup is currently running. Please wait for it to finish.",
                    "VideoDedup Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var r = MessageBox.Show(
                this,
                "Do you want to close setup now?",
                "VideoDedup Setup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                Close();
            }
        }

        private void BtnMaintenanceImportCert_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                this,
                "This will update the trusted server certificate for the Client component. Continue?",
                "VideoDedup Setup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            maintenanceClientCertFlow = true;
            TxtClientCertPath.Text = string.Empty;
            UpdateClientCertThumbPreview();
            currentStep = WizardStep.Certificate;
            RenderStep();
        }

        private void BtnMaintenanceExportCert_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                this,
                "Open certificate export guidance now?",
                "VideoDedup Setup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            maintenanceServerExportOnly = true;
            PopulateServerExportPanel();
            currentStep = WizardStep.ServerCertificate;
            RenderStep();
        }

        private void BtnClientCertBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Filter = "Certificate (*.crt)|*.crt|All files (*.*)|*.*",
                Title = "Select VideoDedup.crt from the server",
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    TxtClientCertPath.Text = dlg.FileName;
                }
            }
        }

        private void TxtClientCertPath_TextChanged(object sender, EventArgs e) =>
            UpdateClientCertThumbPreview();

        private void BtnOpenCertFolder_Click(object sender, EventArgs e)
        {
            var path = TxtServerCertPath.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                {
                    _ = Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = "\"" + dir + "\"",
                        UseShellExecute = true,
                    });
                }
            }
            catch
            {
                // ignore
            }
        }

        private void BtnSaveCertCopy_Click(object sender, EventArgs e)
        {
            var src = TxtServerCertPath.Text.Trim();
            if (string.IsNullOrEmpty(src) || !File.Exists(src))
            {
                _ = MessageBox.Show(
                    this,
                    "Server certificate file is not available yet.",
                    "VideoDedup Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new SaveFileDialog
            {
                Filter = "Certificate (*.crt)|*.crt",
                FileName = "VideoDedup.crt",
                Title = "Save a copy of the server certificate",
            })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    File.Copy(src, dlg.FileName, overwrite: true);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(
                        this,
                        "Could not save the file:\n" + ex.Message,
                        "VideoDedup Setup",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCopyThumbprint_Click(object sender, EventArgs e)
        {
            var t = LblServerCertThumbprint.Text;
            const string prefix = "Thumbprint: ";
            if (t.StartsWith(prefix, StringComparison.Ordinal))
            {
                t = t.Substring(prefix.Length).Trim();
            }

            if (string.IsNullOrEmpty(t) || t == "—")
            {
                return;
            }

            try
            {
                Clipboard.SetText(t);
            }
            catch
            {
                // ignore
            }
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (RdoInstall.Checked)
            {
                selectedMode = SetupMode.Install;
            }
            else if (RdoUpdate.Checked)
            {
                selectedMode = SetupMode.Update;
            }
            else if (RdoUninstall.Checked)
            {
                selectedMode = SetupMode.Uninstall;
            }

            var uninstallMode = selectedMode == SetupMode.Uninstall;
            ChbClient.Enabled = !uninstallMode;
            ChbServer.Enabled = !uninstallMode;
            if (uninstallMode)
            {
                ChbClient.Checked = true;
                ChbServer.Checked = true;
            }

            UpdateSelectionSummary();
            RenderStep();
        }

        private void FinishClientCertPicker(string clientCertSource)
        {
            if (!ValidateClientCertificatePath(clientCertSource))
            {
                return;
            }

            if (maintenanceClientCertFlow)
            {
                string stagedSource;
                try
                {
                    stagedSource = ClientCertInstallStaging.PrepareForMsi(clientCertSource);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(
                        this,
                        "Could not copy the certificate to a local folder for installation."
                        + Environment.NewLine + Environment.NewLine + ex.Message,
                        "VideoDedup Setup",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                app.SetVariableString("TriggerClientCertImport", "1", false);
                app.SetVariableString(
                    "ClientCertSource",
                    stagedSource,
                    false);
                app.SetSelectedFeatures(new[] { "ClientFeature" });
                app.Plan(LaunchAction.Modify);
                StartProgress("Updating trusted client certificate...");
                lastApplyIncludedServer = false;
                return;
            }

            TxtClientCertPath.Text = clientCertSource;
            BeginPlannedOperation();
        }

        private void ApplyClientCertToEngine(
            string pickedPath,
            List<string> features)
        {
            if (features.Contains("ClientFeature")
                && features.Contains("ServerFeature"))
            {
                var samePc = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    ServerFolderName,
                    "cert",
                    "VideoDedup.crt");
                app.SetVariableString("ClientCertSource", samePc, false);
                return;
            }

            var source = pickedPath ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(source))
            {
                try
                {
                    source = ClientCertInstallStaging.PrepareForMsi(source);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        "Could not copy the certificate to a local folder for installation.",
                        ex);
                }
            }

            app.SetVariableString("ClientCertSource", source, false);
        }

        private void BeginInstallAfterCertResolved(List<string> selectedFeatures, bool updateMode)
        {
            ApplyConnectivityToEngine(selectedFeatures);
            ApplyClientCertToEngine(
                TxtClientCertPath.Text.Trim(),
                selectedFeatures);
            app.SetSelectedFeatures(selectedFeatures);
            var launchAction = LaunchAction.Install;
            if (updateMode)
            {
                launchAction = selectedFeatures.Contains("ServerFeature")
                    ? LaunchAction.Repair
                    : LaunchAction.Modify;
            }

            app.Plan(launchAction);
            StartProgress(updateMode ? "Applying changes..." : "Installing VideoDedup...");
            lastApplyIncludedServer = selectedFeatures.Contains("ServerFeature");
        }

        private void StartProgress(string statusText)
        {
            LblStatus.Text = statusText;
            PrgProgress.Value = 0;
            currentStep = WizardStep.Progress;
            RenderStep();
        }

        private void RenderStep()
        {
            PnlSelection.Visible = currentStep == WizardStep.Selection;
            PnlConnectivity.Visible = currentStep == WizardStep.Connectivity;
            PnlClientCertPicker.Visible = currentStep == WizardStep.Certificate;
            PnlServerCertExport.Visible = currentStep == WizardStep.ServerCertificate;
            PnlProgress.Visible = currentStep == WizardStep.Progress;
            PnlComplete.Visible = currentStep == WizardStep.Complete;

            BtnBack.Enabled = currentStep == WizardStep.Connectivity
                || currentStep == WizardStep.Certificate;
            BtnBack.Visible = BtnBack.Enabled;
            BtnCancel.Visible = currentStep != WizardStep.Progress && currentStep != WizardStep.Complete && currentStep != WizardStep.ServerCertificate;
            BtnNext.Visible = currentStep != WizardStep.Progress;

            switch (currentStep)
            {
                case WizardStep.Selection:
                    UpdateSelectionSummary();
                    SetHeader(
                        "Choose setup options",
                        "Pick what you want to do on this computer, then continue.",
                        FormatStepLabel(1));
                    BtnNext.Text = "Next >";
                    break;
                case WizardStep.Connectivity:
                    PopulateNetworkBindingList();
                    SetHeader(
                        "Server connectivity",
                        "Configure the TCP port and network addresses the server will listen on.",
                        FormatStepLabel(GetStepNumber(WizardStep.Connectivity)));
                    BtnNext.Text = "Next >";
                    break;
                case WizardStep.Certificate:
                    SetHeader(
                        "Client certificate",
                        "Choose the certificate file from your server PC, or continue without one.",
                        FormatStepLabel(GetStepNumber(WizardStep.Certificate)));
                    BtnNext.Text = maintenanceClientCertFlow
                        ? "Start update"
                        : "Continue";
                    break;
                case WizardStep.Progress:
                    SetHeader(
                        "Working...",
                        "Please wait while setup completes.",
                        FormatStepLabel(GetWizardStepCount()));
                    break;
                case WizardStep.Complete:
                    SetHeader(
                        "Completed",
                        "Setup finished.",
                        "Done");
                    BtnNext.Text = "Finish";
                    BtnNext.Visible = true;
                    BtnBack.Visible = false;
                    BtnCancel.Visible = false;
                    break;
                case WizardStep.ServerCertificate:
                    SetHeader(
                        "Share server certificate",
                        "Use this certificate when installing Client on other PCs.",
                        "Done");
                    BtnNext.Text = "Finish";
                    BtnNext.Visible = true;
                    BtnBack.Visible = false;
                    BtnCancel.Visible = false;
                    break;
            }
        }

        private void PopulateServerExportPanel()
        {
            var path = app.GetVariableString("ServerCertPath");
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    ServerFolderName,
                    "cert",
                    "VideoDedup.crt");
            }

            TxtServerCertPath.Text = path;

            var thumb = app.GetVariableString("ServerCertThumbprint");
            if (string.IsNullOrWhiteSpace(thumb) && File.Exists(path))
            {
                try
                {
                    using (var c = new X509Certificate2(path))
                    {
                        thumb = c.Thumbprint;
                    }
                }
                catch
                {
                    thumb = string.Empty;
                }
            }

            var sidecar = path + ".thumbprint.txt";
            if (string.IsNullOrWhiteSpace(thumb) && File.Exists(sidecar))
            {
                thumb = File.ReadAllText(sidecar).Trim();
            }

            LblServerCertThumbprint.Text = string.IsNullOrEmpty(thumb)
                ? "Thumbprint: —"
                : "Thumbprint: " + thumb;
        }

        private void UpdateClientCertThumbPreview()
        {
            var path = TxtClientCertPath.Text.Trim();
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                LblClientCertThumbPreview.Text = "Thumbprint: —";
                return;
            }

            try
            {
                using (var c = new X509Certificate2(path))
                {
                    LblClientCertThumbPreview.Text =
                        "Thumbprint: " + c.Thumbprint;
                }
            }
            catch
            {
                LblClientCertThumbPreview.Text = "Thumbprint: (invalid file)";
            }
        }

        private void MoveForwardFromSelection()
        {
            maintenanceClientCertFlow = false;
            maintenanceServerExportOnly = false;

            if (NeedsConnectivityStep())
            {
                currentStep = WizardStep.Connectivity;
                RenderStep();
                return;
            }

            needsClientCertificateStep = NeedsCertificateStep();
            if (needsClientCertificateStep)
            {
                currentStep = WizardStep.Certificate;
                RenderStep();
                return;
            }

            BeginPlannedOperation();
        }

        private void MoveForwardFromConnectivity()
        {
            if (!ValidateServerConnectivity())
            {
                return;
            }

            needsClientCertificateStep = NeedsCertificateStep();
            if (needsClientCertificateStep)
            {
                currentStep = WizardStep.Certificate;
                RenderStep();
                return;
            }

            BeginPlannedOperation();
        }

        private void BeginPlannedOperation()
        {
            var selectedFeatures = CollectSelectedFeatures();
            var uninstallRequested = selectedMode == SetupMode.Uninstall
                || (selectedMode == SetupMode.Update && selectedFeatures.Count == 0);

            if (uninstallRequested)
            {
                var confirmUninstall = MessageBox.Show(
                    this,
                    "This will remove VideoDedup from this computer. Continue?",
                    "VideoDedup Setup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (confirmUninstall != DialogResult.Yes)
                {
                    return;
                }

                ClearConnectivityEngineVariables();
                app.SetSelectedFeatures(selectedFeatures);
                app.Plan(LaunchAction.Uninstall);
                StartProgress("Removing VideoDedup...");
                lastApplyIncludedServer = false;
                return;
            }

            if (NeedsConnectivityStep() && !ValidateServerConnectivity())
            {
                return;
            }

            ApplyConnectivityToEngine(selectedFeatures);

            try
            {
                BeginInstallAfterCertResolved(
                    selectedFeatures,
                    selectedMode == SetupMode.Update);
            }
            catch (InvalidOperationException ex)
            {
                _ = MessageBox.Show(
                    this,
                    ex.Message,
                    "VideoDedup Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private List<string> CollectSelectedFeatures()
        {
            var selectedFeatures = new List<string>();
            if (ChbClient.Checked)
            {
                selectedFeatures.Add("ClientFeature");
            }

            if (ChbServer.Checked)
            {
                selectedFeatures.Add("ServerFeature");
            }

            return selectedFeatures;
        }

        private bool ValidateClientCertificatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return true;
            }

            if (!File.Exists(path))
            {
                _ = MessageBox.Show(
                    this,
                    "The selected certificate file was not found.",
                    "VideoDedup Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                using (var _ = new X509Certificate2(path))
                {
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(
                    this,
                    "The selected file is not a valid certificate:\n" + ex.Message,
                    "VideoDedup Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SetHeader(string title, string subtitle, string step)
        {
            LblHeaderTitle.Text = title;
            LblHeaderSubtitle.Text = subtitle;
            LblStep.Text = step;
        }

        private bool NeedsConnectivityStep() =>
            selectedMode != SetupMode.Uninstall && ChbServer.Checked;

        private bool NeedsCertificateStep() =>
            selectedMode != SetupMode.Uninstall
            && ChbClient.Checked
            && !ChbServer.Checked;

        private int GetWizardStepCount()
        {
            var count = 1;
            if (NeedsConnectivityStep())
            {
                count++;
            }

            if (NeedsCertificateStep())
            {
                count++;
            }

            return count;
        }

        private int GetStepNumber(WizardStep step)
        {
            var number = 1;
            if (step == WizardStep.Selection)
            {
                return number;
            }

            if (NeedsConnectivityStep())
            {
                number++;
                if (step == WizardStep.Connectivity)
                {
                    return number;
                }
            }

            if (NeedsCertificateStep())
            {
                number++;
                if (step == WizardStep.Certificate)
                {
                    return number;
                }
            }

            return number;
        }

        private string FormatStepLabel(int stepNumber) =>
            $"Step {stepNumber} of {GetWizardStepCount()}";

        private void PrefillServerConnectivity()
        {
            var portStr = app.GetVariableString("ServerListenPort");
            if (int.TryParse(portStr, out var port)
                && port >= MinServerPort
                && port <= MaxServerPort)
            {
                NudServerPort.Value = port;
            }
            else
            {
                NudServerPort.Value = DefaultServerPort;
            }
        }

        private void PopulateNetworkBindingList()
        {
            PrefillServerConnectivity();

            var savedBindings = NetworkBindingHelper.ParseBindings(
                app.GetVariableString("ServerListenBindings"));

            suppressBindingItemCheckHandler = true;
            ClbNetworkBindings.Items.Clear();

            var allNetworksEntry = new NetworkBindingEntry(
                NetworkBindingHelper.AllNetworksToken,
                NetworkBindingHelper.AllNetworksDisplay,
                isAllNetworks: true);
            var allIndex = ClbNetworkBindings.Items.Add(allNetworksEntry);

            var useAllNetworks = savedBindings.Count == 1
                && savedBindings[0] == NetworkBindingHelper.AllNetworksToken;
            ClbNetworkBindings.SetItemChecked(
                allIndex,
                useAllNetworks || savedBindings.Count == 0);

            foreach (var adapterEntry in NetworkBindingHelper.EnumerateAdapterAddresses())
            {
                var index = ClbNetworkBindings.Items.Add(adapterEntry);
                if (!useAllNetworks
                    && savedBindings.Contains(adapterEntry.BindToken))
                {
                    ClbNetworkBindings.SetItemChecked(index, true);
                }
            }

            suppressBindingItemCheckHandler = false;
        }

        private void ClbNetworkBindings_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (suppressBindingItemCheckHandler)
            {
                return;
            }

            if (e.Index < 0 || e.Index >= ClbNetworkBindings.Items.Count)
            {
                return;
            }

            var entry = ClbNetworkBindings.Items[e.Index] as NetworkBindingEntry;
            if (entry == null)
            {
                return;
            }

            suppressBindingItemCheckHandler = true;
            try
            {
                if (entry.IsAllNetworks)
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        for (var i = 1; i < ClbNetworkBindings.Items.Count; i++)
                        {
                            ClbNetworkBindings.SetItemChecked(i, false);
                        }
                    }
                }
                else if (e.NewValue == CheckState.Checked)
                {
                    ClbNetworkBindings.SetItemChecked(0, false);
                }
            }
            finally
            {
                suppressBindingItemCheckHandler = false;
            }
        }

        private IReadOnlyList<string> GetSelectedBindings()
        {
            var tokens = new List<string>();
            for (var i = 0; i < ClbNetworkBindings.Items.Count; i++)
            {
                if (!ClbNetworkBindings.GetItemChecked(i))
                {
                    continue;
                }

                if (ClbNetworkBindings.Items[i] is NetworkBindingEntry entry)
                {
                    tokens.Add(entry.BindToken);
                }
            }

            if (tokens.Count > 0)
            {
                return tokens;
            }

            return new List<string> { NetworkBindingHelper.AllNetworksToken };
        }

        private void ClearConnectivityEngineVariables()
        {
            app.SetVariableString("ServerListenPort", string.Empty, false);
            app.SetVariableString("ServerListenBindings", string.Empty, false);
            app.SetVariableString("SyncClientListenPort", string.Empty, false);
        }

        private void ApplyConnectivityToEngine(IReadOnlyList<string> selectedFeatures)
        {
            if (!selectedFeatures.Contains("ServerFeature"))
            {
                ClearConnectivityEngineVariables();
                return;
            }

            var port = (int)NudServerPort.Value;
            var bindings = GetSelectedBindings();
            app.SetVariableString("ServerListenPort", port.ToString(), false);
            app.SetVariableString(
                "ServerListenBindings",
                NetworkBindingHelper.SerializeBindings(bindings),
                false);
            var syncClient = selectedFeatures.Contains("ClientFeature");
            app.SetVariableString(
                "SyncClientListenPort",
                syncClient ? "1" : string.Empty,
                false);
        }

        private bool ValidateServerConnectivity()
        {
            if (!NeedsConnectivityStep())
            {
                return true;
            }

            var port = (int)NudServerPort.Value;
            if (port < MinServerPort || port > MaxServerPort)
            {
                _ = MessageBox.Show(
                    this,
                    $"Port must be between {MinServerPort} and {MaxServerPort}.",
                    "VideoDedup Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (TcpPortProbe.ShouldWarnPortInUse(port))
            {
                var r = MessageBox.Show(
                    this,
                    $"TCP port {port} appears to be in use by another program on this computer. " +
                    "The server may fail to start unless the port is free. Continue anyway?",
                    "VideoDedup Setup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                return r == DialogResult.Yes;
            }

            if (GetSelectedBindings().Count == 0)
            {
                _ = MessageBox.Show(
                    this,
                    "Select at least one network to listen on.",
                    "VideoDedup Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void UpdateSelectionSummary()
        {
            var modeText = selectedMode == SetupMode.Install
                ? "Install"
                : selectedMode == SetupMode.Update
                    ? "Update"
                    : "Uninstall";

            var selected = CollectSelectedFeatures();
            var componentsText = selected.Count == 0
                ? "No components selected"
                : string.Join(" + ", selected);

            LblReview.Text =
                "Summary\n"
                + "Mode: " + modeText + "\n"
                + "Components: " + componentsText;

            var maintenance = packageInstalled && selectedMode != SetupMode.Uninstall;
            LblMaintenanceHint.Visible = maintenance;
            BtnMaintenanceImportCert.Visible = maintenance && clientRegistryPresent;
            BtnMaintenanceExportCert.Visible =
                maintenance
                && !string.IsNullOrWhiteSpace(app.GetVariableString("ServerCertPath"));

            RdoInstall.Enabled = !isUninstall;
            RdoUpdate.Enabled = packageInstalled && !isUninstall;
            RdoUninstall.Enabled = packageInstalled;
        }

        private void SetDetectingState()
        {
            ChbClient.Enabled = false;
            ChbServer.Enabled = false;
            RdoInstall.Enabled = false;
            RdoUpdate.Enabled = false;
            RdoUninstall.Enabled = false;
            BtnNext.Enabled = false;
            SetHeader(
                "Preparing setup",
                "Please wait while setup checks your current installation.",
                "Loading");
        }

        private void ApplyVisualStyle()
        {
            BtnNext.BackColor = System.Drawing.Color.FromArgb(35, 88, 145);
            BtnNext.ForeColor = System.Drawing.Color.White;
            BtnNext.FlatStyle = FlatStyle.Flat;
            BtnNext.FlatAppearance.BorderSize = 0;
        }
    }
}
