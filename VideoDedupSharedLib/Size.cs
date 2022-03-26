namespace VideoDedupGrpc
{
    public sealed partial class Size
    {
        public static implicit operator System.Drawing.Size(Size size) =>
            new(size.Width, size.Height);

        public static implicit operator Size(System.Drawing.Size size) =>
            new() { Width = size.Width, Height = size.Height };
    }
}
