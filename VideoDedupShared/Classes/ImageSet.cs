namespace VideoDedupShared
{
    using System.IO;

    public class ImageSet
    {
        public ImageIndex Index { get; set; }
        public MemoryStream Original { get; set; }
        public MemoryStream Cropped { get; set; }
        public MemoryStream Resized { get; set; }
        public MemoryStream Greyscaled { get; set; }
        public byte[] Bytes { get; set; }
    }
}
