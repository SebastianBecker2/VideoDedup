namespace SetupBootstrapperUI
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using WixToolset.BootstrapperApplicationApi;

    internal sealed class MockBootstrapperRuntime : IBootstrapperUiRuntime
    {
        private readonly Dictionary<string, string> variables =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly TestUiScenario scenario;
        private MainForm mainForm;
        private bool packageInstalled;
        private bool isUninstallMode;
        private bool uninstallPlanned;
        private HashSet<string> selectedFeatures =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public MockBootstrapperRuntime(TestUiScenario scenario)
        {
            this.scenario = scenario;
            InitializeScenarioState();
        }

        public void AttachForm(MainForm form)
        {
            mainForm = form;
        }

        public void StartDetect()
        {
            mainForm?.OnDetectComplete(packageInstalled, isUninstallMode);
        }

        public void Plan(LaunchAction action)
        {
            uninstallPlanned = action == LaunchAction.Uninstall;
            _ = SimulateApplyAsync();
        }

        public void SetSelectedFeatures(IEnumerable<string> features)
        {
            selectedFeatures = new HashSet<string>(
                features ?? Array.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);
        }

        public void SetVariableString(string name, string value, bool formatted = false)
        {
            variables[name] = value ?? string.Empty;
        }

        public string GetVariableString(string name)
        {
            if (variables.TryGetValue(name, out var value))
            {
                return value;
            }

            return string.Empty;
        }

        public void ClearBundleCertVariables()
        {
            SetVariableString("TriggerClientCertImport", string.Empty, false);
            SetVariableString("ClientCertSource", string.Empty, false);
        }

        private void InitializeScenarioState()
        {
            var fakeServerCertPath =
                @"C:\Program Files\VideoDedupServer\cert\VideoDedup.crt";
            const string fakeThumbprint = "11AA22BB33CC44DD55EE66FF77889900AABBCCDD";

            switch (scenario)
            {
                case TestUiScenario.Fresh:
                    packageInstalled = false;
                    isUninstallMode = false;
                    variables["ClientInstallFolderMarker"] = string.Empty;
                    variables["ServerCertPath"] = string.Empty;
                    variables["ServerCertThumbprint"] = string.Empty;
                    break;
                case TestUiScenario.Uninstall:
                    packageInstalled = true;
                    isUninstallMode = true;
                    variables["ClientInstallFolderMarker"] =
                        @"C:\Program Files\VideoDedupClient";
                    variables["ServerCertPath"] = fakeServerCertPath;
                    variables["ServerCertThumbprint"] = fakeThumbprint;
                    break;
                case TestUiScenario.Maintenance:
                default:
                    packageInstalled = true;
                    isUninstallMode = false;
                    variables["ClientInstallFolderMarker"] =
                        @"C:\Program Files\VideoDedupClient";
                    variables["ServerCertPath"] = fakeServerCertPath;
                    variables["ServerCertThumbprint"] = fakeThumbprint;
                    break;
            }
        }

        private async Task SimulateApplyAsync()
        {
            if (mainForm == null)
            {
                return;
            }

            var progressValues = new[] { 5, 20, 40, 60, 80, 100 };
            foreach (var value in progressValues)
            {
                await Task.Delay(180).ConfigureAwait(false);
                InvokeOnUiThread(() => mainForm.OnProgress(value));
            }

            ApplyScenarioState();

            var message = uninstallPlanned
                ? "Uninstallation complete."
                : "Installation complete.";
            InvokeOnUiThread(() => mainForm.OnApplyComplete(true, message));
        }

        private void ApplyScenarioState()
        {
            if (uninstallPlanned || selectedFeatures.Count == 0)
            {
                packageInstalled = false;
                isUninstallMode = false;
                variables["ClientInstallFolderMarker"] = string.Empty;
                return;
            }

            packageInstalled = true;
            isUninstallMode = false;
            if (selectedFeatures.Contains("ClientFeature"))
            {
                variables["ClientInstallFolderMarker"] =
                    @"C:\Program Files\VideoDedupClient";
            }
        }

        private void InvokeOnUiThread(Action action)
        {
            if (mainForm.IsDisposed)
            {
                return;
            }

            if (mainForm.InvokeRequired)
            {
                _ = mainForm.BeginInvoke(action);
                return;
            }

            action();
        }
    }
}
