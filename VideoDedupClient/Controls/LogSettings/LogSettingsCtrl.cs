namespace VideoDedupClient.Controls.LogSettings
{
    using System;
    using System.Windows.Forms;
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
    }
}
