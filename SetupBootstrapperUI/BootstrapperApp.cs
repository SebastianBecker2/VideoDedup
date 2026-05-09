namespace SetupBootstrapperUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using WixToolset.BootstrapperApplicationApi;

    public class BootstrapperApp : BootstrapperApplication, IBootstrapperUiRuntime
    {
        private static readonly string BootstrapperLogPath = Path.Combine(
            Path.GetTempPath(),
            "SetupBootstrapperUI.ba.log");

        private MainForm mainForm;
        private int exitCode;
        private bool uninstallPlanned;
        private bool packageInstalled;
        private HashSet<string> selectedFeatures =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static void Log(string message)
        {
            try
            {
                File.AppendAllText(
                    BootstrapperLogPath,
                    $"[{DateTime.UtcNow:O}] {message}{Environment.NewLine}");
            }
            catch
            {
                // ignore logging failures
            }
        }

        protected override void Run()
        {
            DetectPackageComplete += OnDetectPackageComplete;
            DetectComplete += OnDetectComplete;
            PlanComplete += OnPlanComplete;
            PlanMsiFeature += OnPlanMsiFeature;
            ApplyComplete += OnApplyComplete;
            ExecuteProgress += OnExecuteProgress;
            Error += (s, e) =>
            {
                if (mainForm != null && !mainForm.IsDisposed)
                {
                    _ = mainForm.BeginInvoke(new Action(() =>
                        mainForm.OnError(e.ErrorMessage)));
                }
            };

            using (var uiClosed = new ManualResetEvent(false))
            using (var uiReady = new ManualResetEvent(false))
            {
                var uiThread = new Thread(() =>
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    mainForm = new MainForm(this);
                    _ = mainForm.Handle;
                    _ = uiReady.Set();
                    mainForm.FormClosed += (s, e) => uiClosed.Set();

                    Application.Run(mainForm);
                    _ = uiClosed.Set();
                })
                {
                    Name = "VideoDedupBootstrapperUI"
                };
                uiThread.SetApartmentState(ApartmentState.STA);
                uiThread.Start();

                _ = uiReady.WaitOne();
                engine.Detect();
                _ = uiClosed.WaitOne();
            }

            engine.Quit(exitCode);
        }

        private void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId == "VideoDedupMsi")
            {
                packageInstalled = e.State == PackageState.Present;
            }
        }

        private void OnDetectComplete(object sender, DetectCompleteEventArgs e)
        {
            try
            {
                if (e.Status < 0)
                {
                    exitCode = e.Status;
                    _ = mainForm.BeginInvoke(new Action(() => mainForm.Close()));
                    return;
                }

                var isUninstall = false;
                try
                {
                    // Prefer command-line action during detect because WixBundleAction
                    // can be unavailable this early in some runs.
                    var action = engine.GetVariableNumeric("WixBundleCommandLineAction");
                    isUninstall = action == (long)LaunchAction.Uninstall;
                }
                catch (Exception ex)
                {
                    Log("Reading WixBundleCommandLineAction failed: " + ex);
                }

                _ = mainForm.BeginInvoke(new Action(() =>
                    mainForm.OnDetectComplete(packageInstalled, isUninstall)));
            }
            catch (Exception ex)
            {
                Log("OnDetectComplete failed: " + ex);
                _ = (mainForm?.BeginInvoke(new Action(() =>
                    mainForm.OnError("Detection failed. See %TEMP%\\SetupBootstrapperUI.ba.log"))));
            }
        }

        private void OnPlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (e.Status >= 0)
            {
                engine.Apply(mainForm.Handle);
            }
            else
            {
                exitCode = e.Status;
                _ = mainForm.Invoke(new Action(() =>
                    mainForm.OnApplyComplete(
                        false,
                        $"Planning failed: 0x{e.Status:X8}")));
            }
        }

        private void OnPlanMsiFeature(object sender, PlanMsiFeatureEventArgs e)
        {
            if (!string.Equals(e.PackageId, "VideoDedupMsi", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (uninstallPlanned)
            {
                e.State = e.RecommendedState;
                return;
            }

            if (selectedFeatures.Contains(e.FeatureId))
            {
                e.State = FeatureState.Local;
                return;
            }

            // Install and Modify: unselected features must be removed. RecommendedState
            // would keep already-installed features during Modify ("change" would not uninstall).
            e.State = FeatureState.Absent;
        }

        private void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            exitCode = e.Status;
            var success = e.Status >= 0;
            var message = success
                ? (uninstallPlanned
                    ? "Uninstallation complete."
                    : "Installation complete.")
                : $"Operation failed: 0x{e.Status:X8}";
            _ = mainForm.Invoke(new Action(() =>
                mainForm.OnApplyComplete(success, message)));
        }

        private void OnExecuteProgress(object sender, ExecuteProgressEventArgs e) =>
            mainForm.BeginInvoke(new Action(() =>
                mainForm.OnProgress(e.OverallPercentage)));

        public void Plan(LaunchAction action)
        {
            uninstallPlanned = action == LaunchAction.Uninstall;
            engine.Plan(action);
        }

        public void AttachForm(MainForm form)
        {
            mainForm = form;
        }

        public void SetSelectedFeatures(IEnumerable<string> features)
            => selectedFeatures = new HashSet<string>(features, StringComparer.OrdinalIgnoreCase);

        public void SetVariableString(string name, string value, bool formatted = false)
            => engine.SetVariableString(name, value, formatted);

        /// <summary>
        /// Reads a Burn string variable. Missing or unreadable variables return empty string
        /// (Engine.GetVariableString throws Win32Exception "Element not found" otherwise).
        /// </summary>
        public string GetVariableString(string name)
        {
            try
            {
                return engine.GetVariableString(name) ?? string.Empty;
            }
            catch (Win32Exception ex)
            {
                Log($"GetVariableString({name}) Win32Exception: {ex.Message} (0x{ex.NativeErrorCode:X})");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Log($"GetVariableString({name}) failed: {ex}");
                return string.Empty;
            }
        }

        public void ClearBundleCertVariables()
        {
            engine.SetVariableString("TriggerClientCertImport", string.Empty, false);
            engine.SetVariableString("ClientCertSource", string.Empty, false);
        }
    }
}
