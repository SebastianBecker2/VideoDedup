namespace VideoDedupClient.Dialogs
{
    using System;
    using System.Windows.Forms;
    using Controls.DnsTextBox;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupClient;

    public partial class ClientConfigDlg : Form
    {
        public ConfigData? Settings { get; set; }

        public ClientConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            if (Settings is not null)
            {
                TxtServerAddress.Text = Settings.ServerAddress;
                NudStatusRequestInterval.Value =
                    (decimal)Settings.StatusRequestInterval.TotalMilliseconds;
                TxtClientSourcePath.Text = Settings.ClientSourcePath;
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

            Settings ??= new ConfigData();

            Settings.ServerAddress = TxtServerAddress.Text;
            Settings.StatusRequestInterval = TimeSpan.FromMilliseconds(
                (int)NudStatusRequestInterval.Value);
            Settings.ClientSourcePath = TxtClientSourcePath.Text;

            DialogResult = DialogResult.OK;
        }

        private void TxtServerAddress_ResolveStarted(object sender,
            ResolveStartedEventArgs e)
        {
            if (PibServerAddressLoading.Image == null)
            {
                PibServerAddressLoading.Image = Properties.Resources.Loading;
            }
        }

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
