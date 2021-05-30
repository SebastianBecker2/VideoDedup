namespace VideoDedupShared
{
    using System.IO;

    public class ImageSet
    {
        public MemoryStream Orignal { get; set; }
        public MemoryStream Cropped { get; set; }
        public MemoryStream Resized { get; set; }
        public MemoryStream Greyscaled { get; set; }
    }
}
