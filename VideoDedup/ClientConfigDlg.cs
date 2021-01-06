namespace VideoDedup
{
    using System;
    using System.Windows.Forms;
    using VideoDedup.DnsTextBox;

    public partial class ClientConfigDlg : Form
    {
        public ConfigData Configuration { get; set; }

        public ClientConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            TxtServerAddress.Text = Configuration.ServerAddress;
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

            Configuration.ServerAddress = TxtServerAddress.Text;
            DialogResult = DialogResult.OK;
        }

        private void TxtServerAddress_ResolveStarted(object sender,
            DnsTextBox.ResolveStartedEventArgs e)
        {
            if (PibServerAddressLoading.Image == null)
            {
                PibServerAddressLoading.Image = Properties.Resources.Loading;
            }
        }

        private void TxtServerAddress_ResolveSuccessful(object sender,
            DnsTextBox.ResolveSuccessfulEventArgs e) =>
            PibServerAddressLoading.Image = null;

        private void TxtServerAddress_ResolveFailed(object sender,
            DnsTextBox.ResolveFailedEventArgs e) =>
            PibServerAddressLoading.Image = null;
    }
}
