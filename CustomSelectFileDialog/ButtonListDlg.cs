namespace CustomSelectFileDlg
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
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

        public IEnumerable<IList<Element>>? Entries { get; set; }

        [Category("Action")]
        public event EventHandler<ElementClickEventArgs>? ElementClick;
        protected virtual void OnElementClicked(Element element) =>
            ElementClick?.Invoke(this, new ElementClickEventArgs(element));

        public ButtonListDlg()
        {
            InitializeComponent();
            TlpButtonLists.AutoSize = true;
        }

        private void HandleListBoxClick(object? sender, System.EventArgs e)
        {
            var lsb = sender as ListBox;
            Debug.Assert(lsb is not null);
            var elementList = lsb.Tag as IList<Element>;
            Debug.Assert(elementList is not null);
            OnElementClicked(elementList[lsb.SelectedIndex]);

            Hide();
        }

        private ListBox CreateButtonList(IList<Element> entries)
        {
            var lsb = new ListBox
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                MinimumSize = new Size(0, 0),
                MaximumSize = new Size(260, 200),
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                Size = new Size(1, 1),
                HorizontalScrollbar = true,
            };
            lsb.Items.AddRange(entries.Select(e => e.Text as object).ToArray());
            lsb.Tag = entries;
            lsb.Click += HandleListBoxClick;
            return lsb;
        }

        private void PopulateTlpButtonList(IEnumerable<IList<Element>>? entries)
        {
            TlpButtonLists.Controls.Clear();
            if (entries is null)
            {
                return;
            }
            foreach (var elementList in entries)
            {
                TlpButtonLists.Controls.Add(CreateButtonList(elementList));
            }

        }

        protected override void OnVisibleChanged(System.EventArgs e)
        {
            if (Visible)
            {
                PopulateTlpButtonList(Entries);

                WindowState = FormWindowState.Minimized;
                WindowState = FormWindowState.Normal;
                BringToFront();
                Activate();
                _ = Focus();
            }
            else
            {
                TlpButtonLists.Controls.Clear();
            }

            base.OnVisibleChanged(e);
        }

        protected override void OnDeactivate(System.EventArgs e)
        {
            Hide();
            base.OnDeactivate(e);
        }
    }
}
