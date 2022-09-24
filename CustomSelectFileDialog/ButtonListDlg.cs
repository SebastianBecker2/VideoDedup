namespace CustomSelectFileDlg
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;

    internal partial class ButtonListDlg : Form
    {
        public class Element
        {
            public string Text { get; set; }
            public object? Tag { get; set; }

            public Element(string text, object? tag = null)
            {
                Text = text;
                Tag = tag;
            }
        }

        internal class ElementClickEventArgs : System.EventArgs
        {
            public Element Element { get; set; }

            public ElementClickEventArgs(Element element) =>
                Element = element;
        }

        public IList<Element>? Entries { get; set; }

        [Category("Action")]
        public event EventHandler<ElementClickEventArgs>? ElementClick;
        protected virtual void OnElementClicked(Element element) =>
            ElementClick?.Invoke(this, new ElementClickEventArgs(element));

        public ButtonListDlg()
        {
            InitializeComponent();
            lsbButtonList.AutoSize = true;
            lsbButtonList.MinimumSize = new Size(0, 0);
            lsbButtonList.MaximumSize = new Size(260, 200);
            lsbButtonList.Size = new Size(1, 1);
        }

        protected override void OnVisibleChanged(System.EventArgs e)
        {
            if (Visible)
            {
                lsbButtonList.Items.Clear();
                lsbButtonList.Items.AddRange(
                    (Entries ?? Enumerable.Empty<Element>())
                    .Select(entries => entries.Text as object).ToArray());

                WindowState = FormWindowState.Minimized;
                WindowState = FormWindowState.Normal;
                BringToFront();
                Activate();
                Focus();
            }

            base.OnVisibleChanged(e);
        }

        protected override void OnDeactivate(System.EventArgs e)
        {
            Hide();
            base.OnDeactivate(e);
        }

        protected override void OnLoad(System.EventArgs e)
        {
            lsbButtonList.Click += (_, _) =>
            {
                if (Entries is not null)
                {
                    OnElementClicked(Entries[lsbButtonList.SelectedIndex]);
                }

                Hide();
            };
            base.OnLoad(e);
        }
    }
}
