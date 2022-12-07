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

            public (Button quickSelectButton, Button folderButton) CreateButton()
            {
                var quickSelectButton = new Button
                {
                    Image = Resources.bullet_arrow_right_2,
                    Tag = this,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                    Padding = new Padding(0, 0, 0, 0),
                    Margin = new Padding(0, 0, 0, 0),
                    FlatStyle = FlatStyle.Popup,
                };
                var folderButton = new Button
                {
                    Text = Text,
                    Tag = this,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Padding = new Padding(0, 0, 0, 0),
                    Margin = new Padding(0, 0, 0, 0),
                    FlatStyle = FlatStyle.Popup,
                };
                return (quickSelectButton, folderButton);
            }
        }

        // Add a little spare space to have a click-able area
        private const int SpareSpaceWidth = 5;
        private readonly Button iconButton = new()
        {
            Image = Resources.bullet_folder,
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

        //public IEnumerable<Element>? RootElements { get; set; }

        [Category("Action")]
        public event EventHandler<ElementClickEventArgs>? ElementClick;
        protected virtual void OnElementClicked(Element element) =>
            ElementClick?.Invoke(this, new ElementClickEventArgs(element));

        public event EventHandler<SubElementsRequestedEventArgs>?
            SubElementRequested;
        protected virtual IEnumerable<Element>? OnSubElementRequested(
            Element element)
        {
            var eventArgs = new SubElementsRequestedEventArgs(element);
            SubElementRequested?.Invoke(this, eventArgs);
            return eventArgs.SubElements;
        }

        public event EventHandler<RootElementsRequestedEventArgs>?
            RootElementsRequested;
        protected virtual IEnumerable<Element>? OnRootElementRequested()
        {
            var eventArgs = new RootElementsRequestedEventArgs();
            RootElementsRequested?.Invoke(this, eventArgs);
            return eventArgs.RootElements;
        }

        public ResizableButtonArray()
        {
            iconButton.Click += (_, args) => OnClick(args);

            buttonList.ElementClick += (_, args) =>
                OnElementClicked((args.Element.Tag as Element)!);

            ResizeRedraw = true;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!TlpArray.Controls.Contains(iconButton))
            {
                TlpArray.Controls.Add(iconButton, 0, 0);
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
                    && TlpArray.Controls[index + 1] is { } folderButton
                    && TlpArray.Controls[index] is Button quickSelectButton)
                {
                    folderButton.Text = element.Text;
                    folderButton.Tag = element;
                    quickSelectButton.Image = Resources.bullet_arrow_right_2;
                    quickSelectButton.Tag = element;
                    index += 2;
                    continue;
                }

                (quickSelectButton, folderButton) = element.CreateButton();
                quickSelectButton.Click += (sender, _) =>
                    ShowDropDownList((sender as Button)!);
                folderButton.Click += (_, _) =>
                    OnElementClicked((folderButton.Tag as Element)!);
                TlpArray.Controls.Add(quickSelectButton, index++, 0);
                TlpArray.Controls.Add(folderButton, index++, 0);
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
                .Where(b => b != iconButton)
                .ToList();
            if (!buttons.Any())
            {
                return;
            }

            var collectiveButtonWidth = buttons
                .Where(b => b.Visible)
                .Sum(b => b.Width)
                + iconButton.Width
                // Add a little spare space to have a click-able area
                + SpareSpaceWidth;

            foreach (var buttonIndex in
                     Enumerable.Range(0, (buttons.Count - 2) / 2)
                         .Select(i => i * 2))
            {
                if (collectiveButtonWidth <= Width)
                {
                    break;
                }

                var quickSelectButton = buttons[buttonIndex];
                var folderButton = buttons[buttonIndex + 1];
                if (!quickSelectButton!.Visible)
                {
                    continue;
                }

                collectiveButtonWidth -=
                    quickSelectButton.Width + folderButton.Width;
                quickSelectButton.Visible = false;
                folderButton.Visible = false;
            }

            foreach (var buttonIndex in
                     Enumerable.Range(0, buttons.Count / 2)
                         .Select(x => x * 2)
                         .Reverse())
            {
                var quickSelectButton = buttons[buttonIndex];
                var folderButton = buttons[buttonIndex + 1];
                if (quickSelectButton!.Visible)
                {
                    continue;
                }

                collectiveButtonWidth +=
                    quickSelectButton.Width + folderButton.Width;

                if (collectiveButtonWidth >= Width)
                {
                    break;
                }

                quickSelectButton.Visible = true;
                folderButton.Visible = true;
            }

            // The first visible quickSelectButton gets a left arrow,
            // if at least one button is hidden.
            if (!buttons.First(b => string.IsNullOrEmpty(b.Text)).Visible)
            {
                buttons.First(b => b.Visible && string.IsNullOrEmpty(b.Text)).Image =
                    Resources.bullet_arrow_left_2;
            }
        }

        private void HandleTlpArrayClick(object sender, System.EventArgs e) =>
            OnClick(e);

        private void ShowDropDownList(Button button)
        {
            var allEntries = new List<IList<ButtonListDlg.Element>>();

            var buttons = TlpArray.Controls
                .Cast<Control>()
                .Where(b => b != iconButton)
                .ToList();

            if (button == buttons.First(b => b.Visible))
            {
                var hiddenButtons = buttons
                    .Where(b => !b.Visible)
                    .Where(b => !string.IsNullOrEmpty(b.Text))
                    .ToList();

                if (hiddenButtons.Any())
                {
                    allEntries.Add(hiddenButtons
                        .Select(b => new ButtonListDlg.Element(b.Text, b.Tag))
                        .Reverse()
                        .ToList());
                }

                var rootElements = OnRootElementRequested();
                if (rootElements is not null)
                {
                    allEntries.Add(rootElements
                        .Select(e => new ButtonListDlg.Element(e.Text, e))
                        .ToList());
                }
            }
            else
            {
                var subElements = OnSubElementRequested((button.Tag as Element)!);
                if (subElements is not null)
                {
                    allEntries.Add(subElements
                        .Select(e => new ButtonListDlg.Element(e.Text, e))
                        .ToList());
                }
            }

            var location = button.PointToScreen(Point.Empty);
            location.Y += button.Size.Height;
            buttonList.Location = location;
            buttonList.Entries = allEntries;
            buttonList.Show();
        }
    }
}
