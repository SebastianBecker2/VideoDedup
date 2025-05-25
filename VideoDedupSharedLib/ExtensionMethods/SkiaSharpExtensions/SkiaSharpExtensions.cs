namespace VideoDedupSharedLib.ExtensionMethods.SkiaSharpExtensions
{
    using SkiaSharp;

    public static class SkiaSharpExtensions
    {
        private const int DefaultBlacknessThreshold = 30;

        private static readonly SKColorFilter GreyScaleColorMatrix =
            SKColorFilter.CreateColorMatrix(
            [
                0.2126f, 0.7152f, 0.0722f, 0, 0,  // Red
                0.2126f, 0.7152f, 0.0722f, 0, 0,  // Green
                0.2126f, 0.7152f, 0.0722f, 0, 0,  // Blue
                0,       0,       0,       1, 0   // Alpha
            ]);

        public static SKBitmap FromBytes(byte[] bytes)
        {
            using var stream = new SKMemoryStream(bytes);
            return SKBitmap.Decode(stream);
        }

        public static SKBitmap Resize(
            this SKBitmap original,
            int newWidth,
            int newHeight,
            SKFilterMode filterMode = SKFilterMode.Nearest) =>
            original.Resize(new SKSizeI(newWidth, newHeight), filterMode);

        public static SKBitmap Resize(
            this SKBitmap original,
            SKSizeI newSize,
            SKFilterMode filterMode = SKFilterMode.Nearest) =>
            original.Resize(
                newSize,
                new SKSamplingOptions(filterMode, SKMipmapMode.None));

        public static void MakeGrayScale(this SKBitmap original)
        {
            using var canvas = new SKCanvas(original);
            using var paint = new SKPaint { ColorFilter = GreyScaleColorMatrix };
            canvas.DrawBitmap(original, 0, 0, paint);
        }

        public static void CropBlackBars(
            this SKBitmap original,
            int blacknessThreshold = DefaultBlacknessThreshold)
        {
            var width = original.Width;
            var height = original.Height;
            var left = width;
            var top = height;

            // Function to calculate luma (brightness) of a pixel
            static float GetLuma(SKColor pixel) =>
                (pixel.Red * 0.2126f) +
                (pixel.Green * 0.7152f) +
                (pixel.Blue * 0.0722f);

            // Scan for left border
            using var leftBarTask = Task.Run(() =>
            {
                for (var x = 0; x < width / 2; x++)
                {
                    float lumaSum = 0;
                    for (var y = 0; y < height; y++)
                    {
                        lumaSum += GetLuma(original.GetPixel(x, y));
                    }
                    if (lumaSum / height > blacknessThreshold)
                    {
                        return x;
                    }
                }
                return width / 2;
            });

            // Scan for right border
            using var rightBarTask = Task.Run(() =>
            {
                for (var x = width - 1; x >= width / 2; x--)
                {
                    float lumaSum = 0;
                    for (var y = 0; y < height; y++)
                    {
                        lumaSum += GetLuma(original.GetPixel(x, y));
                    }
                    if (lumaSum / height > blacknessThreshold)
                    {
                        return x;
                    }
                }
                return (width / 2) - 1;
            });

            var leftBar = leftBarTask.Result;
            var rightBar = rightBarTask.Result;
            width = rightBar - leftBar + 1;

            // Scan for top border
            using var topBarTask = Task.Run(() =>
            {
                for (var y = 0; y < height / 2; y++)
                {
                    float lumaSum = 0;
                    for (var x = leftBar; x <= rightBar; x++)
                    {
                        lumaSum += GetLuma(original.GetPixel(x, y));
                    }
                    if (lumaSum / width > blacknessThreshold)
                    {
                        return y;
                    }
                }
                return height / 2;
            });

            // Scan for bottom border
            using var bottomBarTask = Task.Run(() =>
            {
                for (var y = height - 1; y >= height / 2; y--)
                {
                    float lumaSum = 0;
                    for (var x = leftBar; x <= rightBar; x++)
                    {
                        lumaSum += GetLuma(original.GetPixel(x, y));
                    }
                    if (lumaSum / width > blacknessThreshold)
                    {
                        return y;
                    }
                }
                return (height / 2) - 1;
            });

            var topBar = topBarTask.Result;
            var bottomBar = bottomBarTask.Result;
            height = bottomBar - topBar + 1;

            // Ensure a valid cropping region
            if (width <= 0 || height <= 0)
            {
                return;
            }

            if (width == original.Width && height == original.Height)
            {
                return;
            }

            // Crop the image using ExtractSubset()
            _ = original.ExtractSubset(
                original,
                new SKRectI(leftBar, topBar, rightBar + 1, bottomBar + 1));
        }

        public static MemoryStream ToMemoryStream(
            this SKBitmap bitmap,
            SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg,
            int quality = 100)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(format, quality);
            return new MemoryStream(data.ToArray());
        }
    }
}
