namespace VideoDedupShared.ImageExtension
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;

    public static class ImageExtension
    {
        public static readonly ColorMatrix GreyScaleColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        public static Image Resize(this Image original, Size size) =>
            original.Resize(size.Width, size.Height);

        public static Image Resize(this Image original, int width, int height)
        {
            var result = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(result))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(original, 0, 0, width, height);
            }
            return result;
        }

        public static Bitmap MakeGrayScale(this Image original)
        {
            var newBitmap = new Bitmap(original.Width, original.Height);

            using (var attributes = new ImageAttributes())
            using (var graphics = Graphics.FromImage(newBitmap))
            {
                attributes.SetColorMatrix(GreyScaleColorMatrix);

                graphics.DrawImage(original,
                    new Rectangle(0, 0, original.Width, original.Height),
                    0,
                    0,
                    original.Width,
                    original.Height,
                    GraphicsUnit.Pixel,
                    attributes);
            }

            return newBitmap;
        }
    }
}
