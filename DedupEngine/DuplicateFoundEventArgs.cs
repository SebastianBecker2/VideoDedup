namespace DedupEngine
{
    using System;

    public class DuplicateFoundEventArgs : EventArgs
    {
        public VideoFile File1 { get; set; }
        public VideoFile File2 { get; set; }
        public string BasePath { get; set; }
    }
}
