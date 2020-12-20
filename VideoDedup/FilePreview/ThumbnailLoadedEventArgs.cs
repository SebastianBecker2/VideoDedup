namespace VideoDedup.FilePreview
{
    using System;
    using System.Drawing;

    public class ThumbnailLoadedEventArgs : EventArgs
    {
        public Image Thumbnail { get; set; }
        public int Index { get; set; }
    }
}
