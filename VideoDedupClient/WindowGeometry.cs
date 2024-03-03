namespace VideoDedupClient
{
    internal sealed class WindowGeometry
    {
        public Size Size { get; set; }
        public Point Location { get; set; }
        public FormWindowState WindowState { get; set; }

        public static WindowGeometry FromForm(Form form)
        {
            if (form.WindowState == FormWindowState.Normal)
            {
                return new WindowGeometry
                {
                    Size = form.Size,
                    Location = form.Location,
                    WindowState = form.WindowState,
                };
            }

            return new WindowGeometry
            {
                Size = form.RestoreBounds.Size,
                Location = form.RestoreBounds.Location,
                WindowState = form.WindowState,
            };
        }

        public void ApplyToForm(Form form)
        {
            form.Size = Size;
            form.Location = Location;
            form.WindowState = WindowState;
        }
    }
}
