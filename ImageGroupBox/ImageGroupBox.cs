namespace ImageGroupBox
{
    using System;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Windows.Forms.VisualStyles;
    using System.ComponentModel;

    public class ImageGroupBox : GroupBox
    {
        /// <summary>Contain a reference to the icon to be painted in the header area.</summary>
        private Image icon = null;
        /// <summary>Contain a reference to the VisualStyleRenderer object that will help in drawing visual styles effects.</summary>
        private VisualStyleRenderer renderer = null;

        /// <summary>Get or set the icon to be painted in the header area.</summary>
        [Description("Icon before the text"),
            AmbientValue(null),
            Category("Appearance"),
            Localizable(true)]
        public Image Icon
        {
            get => icon;
            set
            {
                if (icon != value)
                {
                    icon = value;
                    Invalidate(false);
                }
            }
        }

        private int fontHeight = -1;
        private Font cachedFont;
        /// <summary>Gets a rectangle that represents the dimensions fo the GroupBox.</summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                var size = ClientSize;

                if (fontHeight == -1)
                {
                    fontHeight = Font.Height;
                    cachedFont = Font;
                }
                else if (!ReferenceEquals(cachedFont, Font))
                {
                    // Must also cache font identity here because
                    // we need to provide an accurate DisplayRectangle
                    // picture even before the OnFontChanged event bubbles
                    // through.
                    fontHeight = Font.Height;
                    cachedFont = Font;
                }

                // For efficiency, so that we don't need to read property store four times.
                var padding = Padding;
                var headerHeight = Math.Max(fontHeight, Icon.Height);
                return new Rectangle(
                    padding.Left,
                    headerHeight + padding.Top,
                    Math.Max(size.Width - padding.Horizontal, 0),
                    Math.Max(size.Height - headerHeight - padding.Vertical, 0));
            }
        }

        public Rectangle HeaderRectangle =>
            new Rectangle(
                0,
                0,
                ClientSize.Width,
                Math.Max(fontHeight, Icon.Height));

        [Category("Action"), Description("Occurres when the header has been clicked.")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public event MouseEventHandler HeaderClicked;
        protected void OnHeaderClick(MouseEventArgs mouseEventArgs) =>
            HeaderClicked?.Invoke(this, mouseEventArgs);

        /// <summary>Override the GroupBox OnPaint method for customized drawing.</summary>
        /// <param name="e">The PaintEventArgs associated object.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // This override only draws the control if the icon is not null. Otherwise, the base method is called
            if (icon != null
                && Application.RenderWithVisualStyles
                && (Width >= 10)
                && (Height >= 10))
            {
                // Draw the entire control
                DrawGroupBox(e.Graphics);
                e.Graphics.Dispose();
            }
            else
            {
                base.OnPaint(e);
            }
        }

        /// <summary>Draw the entire control with visual styles effects.</summary>
        /// <param name="graphics">The Graphics object in which the control must painted to.</param>
        private void DrawGroupBox(Graphics graphics)
        {
            // Set the flags of TextFormat
            var textFlags = TextFormatFlags.PreserveGraphicsTranslateTransform
                | TextFormatFlags.PreserveGraphicsClipping
                | TextFormatFlags.TextBoxControl
                | TextFormatFlags.WordBreak;
            if (!ShowKeyboardCues)
            {
                textFlags |= TextFormatFlags.HidePrefix;
            }
            if (RightToLeft == RightToLeft.Yes)
            {
                textFlags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }

            // Initialize the renderer for visual styles
            var state = Enabled ? GroupBoxState.Normal : GroupBoxState.Disabled;
            InitializeRenderer((int)state);

            var textSize = TextRenderer.MeasureText(
                graphics,
                base.Text,
                base.Font,
                new Size(Width - 14, Height));

            var headerRectangle = new Rectangle(
                0,
                0,
                Width,
                Math.Max(icon.Height, textSize.Height));

            var iconRectangle = new Rectangle(
                9,
                (headerRectangle.Height - icon.Height) / 2,
                icon.Width,
                icon.Height);

            var textRactangle = new Rectangle(
                iconRectangle.Right,
                (headerRectangle.Height - textSize.Height) / 2,
                textSize.Width,
                textSize.Height);

            // Move the rectangles if needed by the textFlags
            if ((textFlags & TextFormatFlags.Right) == TextFormatFlags.Right)
            {
                iconRectangle.X = Width - iconRectangle.Right - 1;
                textRactangle.X = Width - textRactangle.Right - 1;
            }

            var displayRectangle = new Rectangle(
                0,
                headerRectangle.Height / 2,
                Width,
                Height - (headerRectangle.Height / 2));

            DrawIcon(graphics, icon, iconRectangle, state);

            TextRenderer.DrawText(
                graphics,
                Text,
                Font,
                textRactangle,
                renderer.GetColor(ColorProperty.TextColor),
                BackColor,
                textFlags);

            DrawBackground(
                graphics,
                displayRectangle,
                headerRectangle,
                icon.Width,
                textFlags);
        }

        /// <summary>Draw an icon in a enabled or disabled state.</summary>
        /// <param name="grfx">The Graphics object in which the icon must painted to.</param>
        /// <param name="icon">The icon to be painted</param>
        /// <param name="rc">The rectangle that bounds the icon.</param>
        /// <param name="state">Specifies whether the icon must be painted in a disabled or endabled state.</param>
        private void DrawIcon(
            Graphics grfx,
            Image icon,
            Rectangle rc,
            GroupBoxState state)
        {
            if (state == GroupBoxState.Disabled)
            {
                // Draw the disabled icon
                ControlPaint.DrawImageDisabled(
                    grfx,
                    icon,
                    rc.Left,
                    rc.Top,
                    Color.Empty);
            }
            else
            {
                // Draw the enabled icon
                grfx.DrawImage(icon, rc);
            }
        }

        /// <summary>Draw the rounded rectangle of the control.</summary>
        /// <param name="grfx">The Graphics object in which the rectangle must drawn to.</param>
        /// <param name="bounds">The rectangle that bounds the rounded rectangle.</param>
        /// <param name="headerrect">The rectangle that bounds the header area.</param>
        /// <param name="iconwidth">The width of the icon of the header.</param>
        /// <param name="txtflags"></param>
        private void DrawBackground(
            Graphics grfx,
            Rectangle bounds,
            Rectangle headerrect,
            int iconwidth,
            TextFormatFlags txtflags)
        {
            var leftrect = bounds;
            leftrect.Width = 7;
            var middlerect = bounds;
            middlerect.Width = Math.Max(0, headerrect.Width + iconwidth);
            var rightrect = bounds;
            if ((txtflags & TextFormatFlags.Right) == TextFormatFlags.Right)
            {
                leftrect.X = bounds.Right - 7;
                middlerect.X = leftrect.Left - middlerect.Width;
                rightrect.Width = middlerect.X - bounds.X;
            }
            else
            {
                middlerect.X = leftrect.Right;
                rightrect.X = middlerect.Right;
                rightrect.Width = bounds.Right - rightrect.X;
            }
            middlerect.Y = headerrect.Bottom;
            middlerect.Height -= headerrect.Bottom - bounds.Top;
            // Left part
            renderer.DrawBackground(grfx, bounds, leftrect);
            // Middle part
            renderer.DrawBackground(grfx, bounds, middlerect);
            // Right part
            renderer.DrawBackground(grfx, bounds, rightrect);
        }

        /// <summary>Initialize the renderer.</summary>
        /// <param name="state">The state of the control.</param>
        private void InitializeRenderer(int state)
        {
            var visualstyleelement = VisualStyleElement.Button.GroupBox.Normal;
            if (renderer == null)
            {
                renderer = new VisualStyleRenderer(
                    visualstyleelement.ClassName,
                    visualstyleelement.Part,
                    state);
            }
            else
            {
                renderer.SetParameters(
                    visualstyleelement.ClassName,
                    visualstyleelement.Part,
                    state);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!DisplayRectangle.Contains(e.Location))
            {
                OnHeaderClick(e);
                return;
            }
            base.OnMouseClick(e);
        }
    }
}
