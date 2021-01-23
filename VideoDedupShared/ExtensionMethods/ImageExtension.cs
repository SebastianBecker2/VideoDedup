namespace VideoDedupShared.ImageExtension
{
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public static class ImageExtension
    {
        public static Image Resize(this Image org, Size size)
        {
            var result = new Bitmap(size.Width, size.Height);
            using (var g = Graphics.FromImage(result))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(org, 0, 0, size.Width, size.Height);
            }
            return result;
        }
    }
}
