namespace VideoDedupShared.ImageExtension
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public static class ImageExtension
    {
        private static readonly ColorMatrix GreyScaleColorMatrix =
            new ColorMatrix(new float[][]
            {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

        private const int DefaultBlacknessThreshold = 30;

        public static Image Resize(
            this Image original,
            Size size,
            InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
            SmoothingMode smoothingMode = SmoothingMode.HighQuality,
            PixelOffsetMode pixelOffsetMode = PixelOffsetMode.HighQuality) =>
            original.Resize(
                size.Width,
                size.Height,
                interpolationMode,
                smoothingMode,
                pixelOffsetMode);

        public static Image Resize(
            this Image original,
            int width,
            int height,
            InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
            SmoothingMode smoothingMode = SmoothingMode.HighQuality,
            PixelOffsetMode pixelOffsetMode = PixelOffsetMode.HighQuality)
        {
            var result = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(result))
            {
                graphics.SmoothingMode = smoothingMode;
                graphics.InterpolationMode = interpolationMode;
                graphics.PixelOffsetMode = pixelOffsetMode;
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

        public static Image CropBlackBars(
            this Bitmap image,
            int blacknessThreshold = DefaultBlacknessThreshold)
        {
            // Using ITU 709 to calculate luma and compare it with
            // a provided reference to determine blackness in bars.
            // We take the avarage of luma of a line (either vertical or
            // horizontal, depending on the bar we inspecting) and
            // compare with the provided reference.
            // Using unsafe code to access bitmap to increase performance.
            // Tried using linq to clean up code but it hits performance.
            // Tried moving pixel extraction and luma calculation to
            // a lambda or local function but it hits performance as well.
            // Using tasks to search for left bar and right bar at the same
            // time and top bar and bottom bar at the same time.

            unsafe
            {
                var bitmapData = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    image.PixelFormat);
                var bytesPerPixel =
                    Image.GetPixelFormatSize(image.PixelFormat) / 8;
                var width = image.Width;
                var height = image.Height;

                var leftBarTask = Task.Factory.StartNew(() =>
                {
                    int? maybeLeftBar = null;
                    foreach (var x in Enumerable.Range(0, width / 2))
                    {
                        var luma = 0.0;
                        foreach (var y in Enumerable.Range(0, height))
                        {
                            var pixel = (byte*)bitmapData.Scan0
                                + (y * bitmapData.Stride)
                                + (x * bytesPerPixel);
                            luma += (pixel[2] * 0.2126)
                                + (pixel[1] * 0.7152)
                                + (pixel[0] * 0.0722);
                        }
                        if (luma / height > blacknessThreshold)
                        {
                            maybeLeftBar = x;
                            break;
                        }
                    }
                    return maybeLeftBar ?? width / 2;
                });

                var rightBarTask = Task.Factory.StartNew(() =>
                {
                    int? maybeRightBar = null;
                    foreach (var x in Enumerable
                        .Range(width / 2, width / 2)
                        .Reverse())
                    {
                        var luma = 0.0;
                        foreach (var y in Enumerable.Range(0, height))
                        {
                            var pixel = (byte*)bitmapData.Scan0
                                + (y * bitmapData.Stride)
                                + (x * bytesPerPixel);
                            luma += (pixel[2] * 0.2126)
                                + (pixel[1] * 0.7152)
                                + (pixel[0] * 0.0722);
                        }
                        if (luma / height > blacknessThreshold)
                        {
                            maybeRightBar = x;
                            break;
                        }
                    }
                    return maybeRightBar ?? (width / 2) - 1;
                });

                var leftBar = leftBarTask.Result;
                var rightBar = rightBarTask.Result;

                width = rightBar - leftBar + 1;

                var topBarTask = Task.Factory.StartNew(() =>
                {
                    int? maybeTopBar = null;
                    foreach (var y in Enumerable.Range(0, height / 2))
                    {
                        var luma = 0.0;
                        foreach (var x in Enumerable.Range(leftBar, width))
                        {
                            var pixel = (byte*)bitmapData.Scan0
                                + (y * bitmapData.Stride)
                                + (x * bytesPerPixel);
                            luma += (pixel[2] * 0.2126)
                                + (pixel[1] * 0.7152)
                                + (pixel[0] * 0.0722);
                        }
                        if (luma / width > blacknessThreshold)
                        {
                            maybeTopBar = y;
                            break;
                        }
                    }
                    return maybeTopBar ?? height / 2;
                });

                var bottomBarTask = Task.Factory.StartNew(() =>
                {
                    int? maybeBottomBar = null;
                    foreach (var y in Enumerable
                        .Range(height / 2, height / 2)
                        .Reverse())
                    {
                        var luma = 0.0;
                        foreach (var x in Enumerable.Range(leftBar, width))
                        {
                            var pixel = (byte*)bitmapData.Scan0
                                + (y * bitmapData.Stride)
                                + (x * bytesPerPixel);
                            luma += (pixel[2] * 0.2126)
                                + (pixel[1] * 0.7152)
                                + (pixel[0] * 0.0722);
                        }
                        if (luma / width > blacknessThreshold)
                        {
                            maybeBottomBar = y;
                            break;
                        }
                    }
                    return maybeBottomBar ?? (height / 2) - 1;
                });

                var topBar = topBarTask.Result;
                var bottomBar = bottomBarTask.Result;

                image.UnlockBits(bitmapData);

                height = bottomBar - topBar + 1;

                if (width == 0 || height == 0)
                {
                    return null;
                }
                if (width == image.Width && height == image.Height)
                {
                    return image;
                }

                var croppedImage = new Bitmap(width, height);
                using (var graphics = Graphics.FromImage(croppedImage))
                {
                    graphics.DrawImage(
                        image,
                        0,
                        0,
                        new Rectangle(leftBar, topBar, width, height),
                        GraphicsUnit.Pixel);
                }

                return croppedImage;
            }
        }

        public static MemoryStream ToMemoryStream(
            this Image image,
            ImageFormat format)
        {
            var ms = new MemoryStream();
            image.Save(ms, format);
            return ms;
        }

        public static MemoryStream ToMemoryStream(this Image image) =>
            image.ToMemoryStream(ImageFormat.Jpeg);
    }
}
