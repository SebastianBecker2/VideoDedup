namespace VideoDedupClient.Controls.LogSettings
{
    using System;
    using System.Windows.Forms;
    using System.Windows.Navigation;
    using VideoDedupGrpc;

    public partial class LogSettingsCtrl : UserControl
    {
        public LogSettingsCtrl()
        {
            InitializeComponent();

            var logLevel = Enum.GetNames<LogSettings.Types.LogLevel>();
            CmbVideoDedupServiceLogLevel.Items.AddRange(logLevel);
            CmbComparisonManagerLogLevel.Items.AddRange(logLevel);
            CmbDedupEngineLogLevel.Items.AddRange(logLevel);
        }

        public void ShowSettings(LogSettings? logSettings)
        {
            if (logSettings is null)
            {
                return;
            }

            CmbVideoDedupServiceLogLevel.SelectedIndex =
                    (int)logSettings.VideoDedupServiceLogLevel;
            CmbComparisonManagerLogLevel.SelectedIndex =
                (int)logSettings.ComparisonManagerLogLevel;
            CmbDedupEngineLogLevel.SelectedIndex =
                (int)logSettings.DedupEngineLogLevel;
        }

        public LogSettings GetSettings() =>
            new()
            {
                VideoDedupServiceLogLevel =
                    (LogSettings.Types.LogLevel)
                        CmbVideoDedupServiceLogLevel.SelectedIndex,
                ComparisonManagerLogLevel =
                    (LogSettings.Types.LogLevel)
                        CmbComparisonManagerLogLevel.SelectedIndex,
                DedupEngineLogLevel =
                    (LogSettings.Types.LogLevel)
                        CmbDedupEngineLogLevel.SelectedIndex,
            };
    }
}
