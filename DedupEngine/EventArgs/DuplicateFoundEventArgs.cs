namespace DedupEngine.EventArgs
{
    using System;
    using VideoComparer;

    public class DuplicateFoundEventArgs : EventArgs
    {
        public DuplicateFoundEventArgs(
            string basePath,
            VideoFile file1,
            VideoFile file2)
        {
            BasePath = basePath;
            File1 = file1;
            File2 = file2;
        }

        public VideoFile File1 { get; set; }
        public VideoFile File2 { get; set; }
        public string BasePath { get; set; }
    }
}
