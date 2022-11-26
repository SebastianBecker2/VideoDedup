namespace CustomSelectFileDlg
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using EventArgs;
    using Properties;

    internal partial class ResizableButtonArray : UserControl
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

            public Button CreateButton()
            {
                var newButton = new Button
                {
                    Text = Text,
                    Tag = this,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Padding = new Padding(0, 0, 0, 0),
                    Margin = new Padding(0, 0, 0, 0),
                    FlatStyle = FlatStyle.Popup,
                };
                return newButton;
            }
        }

        // Add a little spare space to have a click-able area
        private const int SpareSpaceWidth = 5;
        private readonly Button quickSelectButton = new()
        {
            Image = Resources.bullet_arrow_right_2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
            Padding = new Padding(0, 0, 0, 0),
            Margin = new Padding(0, 0, 0, 0),
            FlatStyle = FlatStyle.Popup,
        };
        private IEnumerable<Element>? elements;
        private readonly ButtonListDlg buttonList = new();

        public IEnumerable<Element>? Elements
        {
            get => elements;
            set
            {
                elements = value;
                Invalidate();
            }
        }

        public IEnumerable<Element>? RootElements { get; set; }

        [Category("Action")]
        public event EventHandler<ElementClickEventArgs>? ElementClick;
        protected virtual void OnElementClicked(Element element) =>
            ElementClick?.Invoke(this, new ElementClickEventArgs(element));

        public ResizableButtonArray()
        {
            quickSelectButton.Click += (_, _) => ShowDropDownList();

            buttonList.ElementClick += (_, args) =>
                OnElementClicked((args.Element.Tag as Element)!);

            ResizeRedraw = true;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!TlpArray.Controls.Contains(quickSelectButton))
            {
                TlpArray.Controls.Add(quickSelectButton, 0, 0);
            }

            DrawButtons();
            AdjustVisibleButtons();

            base.OnPaint(e);
        }

        private void DrawButtons()
        {
            if (Elements is null)
            {
                TlpArray.Controls.Clear();
                return;
            }

            var index = 1;
            foreach (var element in Elements)
            {
                if (TlpArray.Controls.Count > index
                    && TlpArray.Controls[index] is Button btn)
                {
                    btn.Text = element.Text;
                    btn.Tag = element;
                    index++;
                    continue;
                }

                btn = element.CreateButton();
                btn.Click += (_, _) => OnElementClicked((btn.Tag as Element)!);
                TlpArray.Controls.Add(btn, index++, 0);
            }

            while (index < TlpArray.Controls.Count)
            {
                TlpArray.Controls[index].Dispose();
            }
        }

        private void AdjustVisibleButtons()
        {
            var buttons = TlpArray.Controls
                .Cast<Button>()
                .Where(b => b != quickSelectButton)
                .ToList();
            if (!buttons.Any())
            {
                return;
            }

            var collectiveButtonWidth = buttons
                .Where(b => b.Visible)
                .Sum(b => b.Width)
                + quickSelectButton.Width
                // Add a little spare space to have a click-able area
                + SpareSpaceWidth;

            foreach (var buttonIndex in Enumerable.Range(0, buttons.Count))
            {
                if (collectiveButtonWidth <= Width)
                {
                    break;
                }

                var button = buttons[buttonIndex];
                if (!button!.Visible)
                {
                    continue;
                }

                collectiveButtonWidth -= button.Width;
                if (buttonIndex == 0)
                {
                    quickSelectButton.Image = Resources.bullet_arrow_left_2;
                }
                button.Visible = false;
            }

            foreach (var buttonIndex in
                     Enumerable.Range(0, buttons.Count).Reverse())
            {
                var button = buttons[buttonIndex];
                if (button!.Visible)
                {
                    continue;
                }

                collectiveButtonWidth += button.Width;

                if (collectiveButtonWidth >= Width)
                {
                    break;
                }

                if (buttonIndex == 0)
                {
                    quickSelectButton.Image = Resources.bullet_arrow_right_2;
                }
                button.Visible = true;
            }
        }

        private void HandleTlpArrayClick(object sender, System.EventArgs e) =>
            OnClick(e);

        private void ShowDropDownList()
        {
            var allEntries = new List<IList<ButtonListDlg.Element>>();

            var hiddenButtons = TlpArray.Controls
                    .Cast<Button>()
                    .Where(b => b != quickSelectButton)
                    .Where(b => !b.Visible)
                    .ToList();

            if (hiddenButtons.Any())
            {
                allEntries.Add(hiddenButtons
                    .Select(b => new ButtonListDlg.Element(b.Text, b.Tag))
                    .Reverse()
                    .ToList());
            }

            if (RootElements is not null)
            {
                allEntries.Add(RootElements
                    .Select(e => new ButtonListDlg.Element(e.Text, e))
                    .ToList());
            }

            var location = quickSelectButton.PointToScreen(Point.Empty);
            location.Y += quickSelectButton.Size.Height;
            buttonList.Location = location;
            buttonList.Entries = allEntries;

            buttonList.Show();
        }
    }
}
