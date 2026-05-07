namespace SetupBootstrapperUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Windows.Forms;
    using WixToolset.BootstrapperApplicationApi;

    public partial class MainForm : Form
    {
        /// <summary>Must match <c>ServerComponent</c> in Setup/Config.wxi.</summary>
        private const string ServerFolderName = "VideoDedupServer";

        private enum WizardStep
        {
            Selection,
            Certificate,
            Review,
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
                case WizardStep.Certificate:
                    FinishClientCertPicker(TxtClientCertPath.Text.Trim());
                    break;
                case WizardStep.Review:
                    BeginPlannedOperation();
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
                case WizardStep.Certificate:
                case WizardStep.Review:
                    currentStep = WizardStep.Selection;
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
                app.SetVariableString("TriggerClientCertImport", "1", false);
                app.SetVariableString(
                    "ClientCertSource",
                    clientCertSource,
                    false);
                app.SetSelectedFeatures(new[] { "ClientFeature" });
                app.Plan(LaunchAction.Modify);
                StartProgress("Updating trusted client certificate...");
                lastApplyIncludedServer = false;
                return;
            }

            TxtClientCertPath.Text = clientCertSource;
            currentStep = WizardStep.Review;
            RenderStep();
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

            app.SetVariableString(
                "ClientCertSource",
                pickedPath ?? string.Empty,
                false);
        }

        private void BeginInstallAfterCertResolved(List<string> selectedFeatures, bool updateMode)
        {
            ApplyClientCertToEngine(
                TxtClientCertPath.Text.Trim(),
                selectedFeatures);
            app.SetSelectedFeatures(selectedFeatures);
            app.Plan(updateMode ? LaunchAction.Modify : LaunchAction.Install);
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
            PnlSelection.Visible = currentStep == WizardStep.Selection || currentStep == WizardStep.Review;
            PnlClientCertPicker.Visible = currentStep == WizardStep.Certificate;
            PnlServerCertExport.Visible = currentStep == WizardStep.ServerCertificate;
            PnlProgress.Visible = currentStep == WizardStep.Progress;
            PnlComplete.Visible = currentStep == WizardStep.Complete;

            BtnBack.Enabled = currentStep == WizardStep.Review || currentStep == WizardStep.Certificate;
            BtnBack.Visible = currentStep != WizardStep.Progress && currentStep != WizardStep.Complete && currentStep != WizardStep.ServerCertificate;
            BtnCancel.Visible = currentStep != WizardStep.Progress && currentStep != WizardStep.Complete && currentStep != WizardStep.ServerCertificate;
            BtnNext.Visible = currentStep != WizardStep.Progress;

            switch (currentStep)
            {
                case WizardStep.Selection:
                    UpdateSelectionSummary();
                    SetHeader(
                        "Choose setup options",
                        "Pick what you want to do on this computer, then continue.",
                        "Step 1 of 4");
                    BtnNext.Text = "Next >";
                    break;
                case WizardStep.Certificate:
                    SetHeader(
                        "Client certificate",
                        "Choose the certificate file from your server PC, or continue without one.",
                        "Step 2 of 4");
                    BtnNext.Text = maintenanceClientCertFlow
                        ? "Start update"
                        : "Continue";
                    break;
                case WizardStep.Review:
                    SetHeader(
                        "Ready to start",
                        "Review your choices and start setup.",
                        "Step 3 of 4");
                    BtnNext.Text = selectedMode == SetupMode.Uninstall
                        ? "Start removal"
                        : "Start setup";
                    break;
                case WizardStep.Progress:
                    SetHeader(
                        "Working...",
                        "Please wait while setup completes.",
                        "Step 4 of 4");
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
            needsClientCertificateStep =
                selectedMode != SetupMode.Uninstall
                && ChbClient.Checked
                && !ChbServer.Checked;

            currentStep = needsClientCertificateStep
                ? WizardStep.Certificate
                : WizardStep.Review;
            RenderStep();
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

                app.SetSelectedFeatures(selectedFeatures);
                app.Plan(LaunchAction.Uninstall);
                StartProgress("Removing VideoDedup...");
                lastApplyIncludedServer = false;
                return;
            }

            BeginInstallAfterCertResolved(
                selectedFeatures,
                selectedMode == SetupMode.Update);
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
