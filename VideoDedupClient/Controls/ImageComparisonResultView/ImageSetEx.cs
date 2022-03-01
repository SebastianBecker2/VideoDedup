namespace VideoDedup.ImageComparisonResultView
{
    using System.Drawing;
    using System.IO;
    using VideoDedup.Properties;
    using VideoDedupShared;
    using VideoDedupShared.ImageExtension;

    public class ImageSetEx
    {
        public ImageSetEx(ImageSet imageSet, Size containerSize)
        {
            ImageSet = imageSet;
            Original = StreamToFittedImage(imageSet.Original, containerSize);
            Cropped = StreamToFittedImage(imageSet.Cropped, containerSize);
            Resized = StreamToFittedImage(imageSet.Resized, containerSize);
            Greyscaled = StreamToFittedImage(imageSet.Greyscaled, containerSize);
        }

        private static Image StreamToFittedImage(
            MemoryStream stream,
            Size containerSize) =>
            ResizeImageToFitContainer(StreamToImage(stream), containerSize);

        private static Image StreamToImage(MemoryStream stream)
        {
            if (stream != null)
            {
                return Image.FromStream(stream);
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
                return new Size(containerSize.Width, height);
            }
            else
            {
                width = (int)(width / ((double)height / containerSize.Height));
                return new Size(width, containerSize.Height);
            }
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

        public ImageSet ImageSet { get; set; }
        public ImageIndex Index => ImageSet.Index;
        public Image Original { get; set; }
        public Image Cropped { get; set; }
        public Image Resized { get; set; }
        public Image Greyscaled { get; set; }
        public byte[] Bytes => ImageSet.Bytes;
    }
}
