namespace CustomSelectFileDlg
{
    using System.Windows.Forms;

    public partial class ResizableTextBox : UserControl
    {
        public new string? Text
        {
            get => TxtInnerTextBox.Text;
            set => TxtInnerTextBox.Text = value;
        }

        public ResizableTextBox() => InitializeComponent();

        protected override void OnResize(System.EventArgs e)
        {
            TxtInnerTextBox.Width = Width;
            TxtInnerTextBox.Top = (Size.Height - TxtInnerTextBox.Height) / 2;
            TxtInnerTextBox.Left = 0;
            base.OnResize(e);
        }

        public void SelectAll() => TxtInnerTextBox.SelectAll();

        private void OnTxtInnerTextBoxKeyDown(object sender, KeyEventArgs e) =>
            OnKeyDown(e);
    }
}
