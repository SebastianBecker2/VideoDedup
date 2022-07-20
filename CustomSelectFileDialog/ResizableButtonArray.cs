namespace CustomSelectFileDlg
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Forms;
    using EventArgs;
    using Properties;

    internal partial class ResizableButtonArray : UserControl
    {
        public class Element
        {
            public string? Text { get; set; }

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
        private readonly Button squashButton = new()
        {
            Image = Resources.bullet_arrow_right_2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
            Padding = new Padding(0, 0, 0, 0),
            Margin = new Padding(0, 0, 0, 0),
            FlatStyle = FlatStyle.Popup,
            Visible = false,
        };
        private IEnumerable<Element>? elements;

        public IEnumerable<Element>? Elements
        {
            get => elements;
            set
            {
                elements = value;
                Invalidate();
            }
        }

        [Category("Action")]
        public event EventHandler<ElementClickEventArgs>? ElementClick;
        protected virtual void OnElementClicked(Element element) =>
            ElementClick?.Invoke(this, new ElementClickEventArgs(element));

        public ResizableButtonArray()
        {
            ResizeRedraw = true;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!TlpArray.Controls.Contains(squashButton))
            {
                TlpArray.Controls.Add(squashButton, 0, 0);
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
                .Where(b => b != squashButton)
                .ToList();
            if (!buttons.Any())
            {
                return;
            }

            var collectiveButtonWidth = buttons
                .Where(b => b.Visible)
                .Sum(b => b.Width);
            // Add a little spare space to have a click-able area
            collectiveButtonWidth += SpareSpaceWidth;
            if (squashButton.Visible)
            {
                collectiveButtonWidth += squashButton.Width;
            }

            Debug.Print($"{Environment.NewLine}Overall width {Width} with button " +
                $"width {collectiveButtonWidth}");

            foreach (var buttonIndex in Enumerable.Range(0, buttons.Count - 1))
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
                    collectiveButtonWidth += squashButton.Width;
                    Debug.Print($"Adding 'squashButton' width {squashButton.Width}");
                    squashButton.Visible = true;
                }
                Debug.Print($"Removing '{button.Text}' width {button.Width}");
                button.Visible = false;
            }

            Debug.Print($"New button width {collectiveButtonWidth} after removing");
            foreach (var buttonIndex in
                     Enumerable.Range(0, buttons.Count - 1).Reverse())
            {
                var button = buttons[buttonIndex];
                if (button!.Visible)
                {
                    continue;
                }

                collectiveButtonWidth += button.Width;
                if (buttonIndex == 0)
                {
                    collectiveButtonWidth -= squashButton.Width;
                }

                if (collectiveButtonWidth >= Width)
                {
                    break;
                }

                if (buttonIndex == 0)
                {
                    Debug.Print($"Removing 'squashButton' width {squashButton.Width}");
                    squashButton.Visible = false;
                }
                Debug.Print($"Adding '{button.Text}' width {button.Width}");
                button.Visible = true;
            }

            Debug.Print($"New button width {collectiveButtonWidth} after adding." +
                $" | Painting {buttons.Count(b => b.Visible)} buttons");
        }

        private void HandleTlpArrayClick(object sender, System.EventArgs e) =>
            OnClick(e);
    }
}
