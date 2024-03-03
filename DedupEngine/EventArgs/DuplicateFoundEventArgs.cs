namespace DedupEngine.EventArgs
{
    using System;
    using VideoComparer;

    public class DuplicateFoundEventArgs(
        string basePath,
        VideoFile file1,
        VideoFile file2)
        : EventArgs
    {
        public VideoFile File1 { get; set; } = file1;
        public VideoFile File2 { get; set; } = file2;
        public string BasePath { get; set; } = basePath;
    }
}
