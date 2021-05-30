namespace VideoDedupShared.ImageExtension
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    public static class ImageExtension
    {
        private static readonly ColorMatrix GreyScaleColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        private const int DefaultPercentToCheck = 30;
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

        private static IEnumerable<int> Range(int start, int count, int percent)
        {
            if (count <= percent)
            {
                return Enumerable.Range(start, count);
            }
            return Enumerable
                .Range(1, percent)
                .Select(index => (int)(count / (double)(percent + 1) * index));
        }

        private static bool IsPixelBlack(
            Image image,
            int x,
            int y,
            int thresblacknessThresholdold = DefaultBlacknessThreshold)
        {
            var pixel = ((Bitmap)image).GetPixel(x, y);
            var color = pixel.R + pixel.G + pixel.B;
            return color <= thresblacknessThresholdold;
        }

        private static bool IsColumnBlack(
            Image image,
            int x,
            int yMin,
            int yMax,
            int percentToCheck = DefaultPercentToCheck,
            int blacknessThreshold = DefaultBlacknessThreshold)
        {
            foreach (var y in Range(yMin, yMax, percentToCheck))
            {
                if (!IsPixelBlack(image, x, y, blacknessThreshold))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsLineBlack(
            Image image,
            int y,
            int xMin,
            int xMax,
            int percentToCheck = DefaultPercentToCheck,
            int blacknessThreshold = DefaultBlacknessThreshold)
        {
            foreach (var x in Range(xMin, xMax, percentToCheck))
            {
                if (!IsPixelBlack(image, x, y, blacknessThreshold))
                {
                    return false;
                }
            }
            return true;
        }

        public static Image CropBlackBars(
            this Image image,
            int percentToCheck = DefaultPercentToCheck,
            int blacknessThreshold = DefaultBlacknessThreshold)
        {
            bool isColumnBlack(int? x) =>
                IsColumnBlack(
                    image,
                    x.Value,
                    0,
                    image.Height,
                    percentToCheck,
                    blacknessThreshold);

            var leftBar = Enumerable
                .Range(0, image.Width / 2)
                .Select(x => (int?)x)
                .FirstOrDefault(x => !isColumnBlack(x))
                ?? image.Width / 2;

            var rightBar = Enumerable
                .Range(image.Width / 2, image.Width / 2)
                .Reverse()
                .Select(x => (int?)x)
                .FirstOrDefault(x => !isColumnBlack(x))
                ?? image.Width / 2;

            bool isLineBlack(int? y) =>
                IsLineBlack(
                    image,
                    y.Value,
                    leftBar,
                    rightBar,
                    percentToCheck,
                    blacknessThreshold);

            var topBar = Enumerable
                .Range(0, image.Height / 2)
                .Select(y => (int?)y)
                .FirstOrDefault(y => !isLineBlack(y))
                ?? image.Height / 2;

            var bottomBar = Enumerable
                .Range(image.Height / 2, image.Height / 2)
                .Reverse()
                .Select(y => (int?)y)
                .FirstOrDefault(y => !isLineBlack(y))
                ?? image.Height / 2;

            var croppedWidth = 1 + rightBar - leftBar;
            var croppedHeight = 1 + bottomBar - topBar;

            if (image.Width == croppedWidth && image.Height == croppedHeight)
            {
                return image;
            }

            var croppedImage = new Bitmap(croppedWidth, croppedHeight);

            using (var graphics = Graphics.FromImage(croppedImage))
            {
                graphics.DrawImage(
                    image,
                    0,
                    0,
                    new Rectangle(leftBar, topBar, croppedWidth, croppedHeight),
                    GraphicsUnit.Pixel);
            }

            return croppedImage;
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
