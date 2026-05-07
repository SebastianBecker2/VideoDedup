namespace SetupBootstrapperUI
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Windows.Forms;
    using WixToolset.BootstrapperApplicationApi;

    internal static class Program
    {
        private static readonly string StartupLogPath = Path.Combine(
            Path.GetTempPath(),
            "SetupBootstrapperUI.startup.log");

        private static void Log(string message)
        {
            File.AppendAllText(
                StartupLogPath,
                $"[{DateTime.UtcNow:O}] {message}{Environment.NewLine}");
        }

        public static void Main(string[] args)
        {
            try
            {
                Log("Program.Main entered.");
                Log($"CurrentDirectory={Environment.CurrentDirectory}");
                Log($"ProcessPath={System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName}");

                if (TryGetTestUiScenario(args, out var scenario))
                {
                    Log($"Starting in --test-ui mode. Scenario={scenario}");
                    RunTestUiMode(scenario);
                    return;
                }

                var app = new BootstrapperApp();
                Log("BootstrapperApp created.");

                ManagedBootstrapperApplication.Run(app);
                Log("ManagedBootstrapperApplication.Run returned.");
            }
            catch (Exception ex)
            {
                Log($"Unhandled exception: {ex}");
                throw;
            }
        }

        private static bool TryGetTestUiScenario(string[] args, out TestUiScenario scenario)
        {
            scenario = TestUiScenario.Fresh;
            if (args == null || args.Length == 0)
            {
                return false;
            }

            var testUiIndex = Array.FindIndex(
                args,
                a => string.Equals(a, "--test-ui", StringComparison.OrdinalIgnoreCase));
            if (testUiIndex < 0)
            {
                return false;
            }

            var scenarioArg = args
                .Skip(testUiIndex + 1)
                .FirstOrDefault(a => !a.StartsWith("--", StringComparison.Ordinal));
            if (string.Equals(scenarioArg, "maintenance", StringComparison.OrdinalIgnoreCase))
            {
                scenario = TestUiScenario.Maintenance;
            }
            else if (string.Equals(scenarioArg, "uninstall", StringComparison.OrdinalIgnoreCase))
            {
                scenario = TestUiScenario.Uninstall;
            }

            return true;
        }

        private static void RunTestUiMode(TestUiScenario scenario)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mockRuntime = new MockBootstrapperRuntime(scenario);
            var form = new MainForm(mockRuntime);
            mockRuntime.StartDetect();

            Application.Run(form);
        }
    }
}
