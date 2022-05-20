namespace VideoDedupClient.Controls.ImageComparisonResultView
{
    using System.Drawing;
    using Google.Protobuf;
    using Properties;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.ByteStringExtensions;
    using VideoDedupSharedLib.ExtensionMethods.ImageExtensions;
    using Size = Size;

    public class ImageSet : IDisposable
    {
        private bool disposedValue;

        public ImageSet(VideoDedupGrpc.ImageSet imageSet, Size containerSize)
        {
            InnerImageSet = imageSet;
            Original = StreamToFittedImage(imageSet.Original, containerSize);
            Cropped = StreamToFittedImage(imageSet.Cropped, containerSize);
            Resized = StreamToFittedImage(imageSet.Resized, containerSize);
            Greyscaled = StreamToFittedImage(imageSet.Greyscaled, containerSize);
        }

        private static Image StreamToFittedImage(
            ByteString? stream,
            Size containerSize) =>
            ResizeImageToFitContainer(StreamToImage(stream), containerSize);

        private static Image StreamToImage(ByteString? stream)
        {
            if (stream is not null)
            {
                return stream.ToImage();
            }
            return Resources.BrokenImageIcon;
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

        private VideoDedupGrpc.ImageSet InnerImageSet { get; set; }
        public ImageIndex Index => InnerImageSet.Index;
        public Image Original { get; set; }
        public Image Cropped { get; set; }
        public Image Resized { get; set; }
        public Image Greyscaled { get; set; }

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
