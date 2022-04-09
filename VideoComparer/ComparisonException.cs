namespace VideoComparer
{
    using System;

    public class ComparisonException : Exception
    {
        public VideoFile VideoFile { get; set; }

        public ComparisonException(
            string message,
            VideoFile videoFile,
            Exception inner)
            : base(message, inner) =>
            VideoFile = videoFile;
    }
}
