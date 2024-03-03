namespace VideoComparer
{
    public class ComparisonException(
        string message,
        VideoFile videoFile,
        Exception inner)
        : Exception(message, inner)
    {
        public VideoFile VideoFile { get; set; } = videoFile;
    }
}
