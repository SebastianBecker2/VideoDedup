namespace DedupEngine
{
    using System.Drawing;
    using System.IO;

    public class ImageSet
    {
        public MemoryStream Stream { get; set; }
        public Image Orignal { get; set; }
        public Image Cropped { get; set; }
        public Image Resized { get; set; }
        public Image Greyscaled { get; set; }
        public byte[] Bytes { get; set; }
    }
}
