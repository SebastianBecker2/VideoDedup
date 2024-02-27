namespace VideoDedupClient.Dialogs
{
    using Controls.DnsTextBox;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using Properties;

    public partial class ClientConfigDlg : Form
    {
        public ConfigData? Configuration { get; set; }

        public ClientConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            if (Configuration is not null)
            {
                TxtServerAddress.Text = Configuration.ServerAddress;
                NudStatusRequestInterval.Value =
                    (decimal)Configuration.StatusRequestInterval.TotalMilliseconds;
                TxtClientSourcePath.Text = Configuration.ClientSourcePath;
            }
            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtServerAddress.Text))
            {
                TxtServerAddress.Text = "localhost";
            }

            if (!DnsTextBoxCtrl.IsDnsNameValid(TxtServerAddress.Text))
            {
                _ = MessageBox.Show("Server address is invalid.",
                    "Invalid Configuration");
                return;
            }

            Configuration ??= new ConfigData();

            Configuration.ServerAddress = TxtServerAddress.Text;
            Configuration.StatusRequestInterval = TimeSpan.FromMilliseconds(
                (int)NudStatusRequestInterval.Value);
            Configuration.ClientSourcePath = TxtClientSourcePath.Text;

            DialogResult = DialogResult.OK;
        }

        private void TxtServerAddress_ResolveStarted(object sender,
            ResolveStartedEventArgs e) =>
            PibServerAddressLoading.Image ??= Resources.Loading;

        private void TxtServerAddress_ResolveSuccessful(object sender,
            ResolveSuccessfulEventArgs e) =>
            PibServerAddressLoading.Image = null;

        private void TxtServerAddress_ResolveFailed(object sender,
            ResolveFailedEventArgs e) =>
            PibServerAddressLoading.Image = null;

        private void BtnSelectClientSourcePath_Click(object sender, EventArgs e)
        {
            using var dlg = new CommonOpenFileDialog();

            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = TxtClientSourcePath.Text;

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            TxtClientSourcePath.Text = dlg.FileName;
        }
    }
}
