namespace VideoDedupClient.Controls.FrameComparisonResultView
{
    using System.Drawing;
    using Google.Protobuf;
    using Properties;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.ByteStringExtensions;
    using VideoDedupSharedLib.ExtensionMethods.ImageExtensions;
    using Size = Size;

    public class FrameSet(VideoDedupGrpc.FrameSet frameSet, Size containerSize)
        : IDisposable
    {
        private bool disposedValue;

        private static Image StreamToFittedImage(
            ByteString stream,
            Size containerSize) =>
            ResizeImageToFitContainer(StreamToImage(stream), containerSize);

        private static Image StreamToImage(ByteString stream)
        {
            if (stream.IsEmpty)
            {
                return Resources.BrokenImageIcon;
            }
            return stream.ToImage();
        }

        private static Size GetSize(
            Size originalSize,
            Size containerSize)
        {
            var width = originalSize.Width;
            var height = originalSize.Height;

            if (width > height)
            {
                height = (int)(height / ((double)width / containerSize.Width));
                return containerSize with { Height = height };
            }

            width = (int)(width / ((double)height / containerSize.Height));
            return containerSize with { Width = width };
        }

        private static Image ResizeImageToFitContainer(
            Image image,
            Size containerSize)
        {
            if (image.Size.Height > containerSize.Height
                || image.Size.Width > containerSize.Width)
            {
                return image.Resize(GetSize(image.Size, containerSize));
            }
            return image.Resize(
                GetSize(image.Size, containerSize),
                System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);
        }

        public FrameIndex Index => frameSet.Index;
        public Image Original { get; set; } =
            StreamToFittedImage(frameSet.Original, containerSize);
        public Image Cropped { get; set; } =
            StreamToFittedImage(frameSet.Cropped, containerSize);
        public Image Resized { get; set; } =
            StreamToFittedImage(frameSet.Resized, containerSize);
        public Image Greyscaled { get; set; } =
            StreamToFittedImage(frameSet.Greyscaled, containerSize);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Original.Dispose();
                    Cropped.Dispose();
                    Resized.Dispose();
                    Greyscaled.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
