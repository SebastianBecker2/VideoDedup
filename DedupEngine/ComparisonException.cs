namespace DedupEngine
{
    using System;

    [Serializable]
    public class ComparisonException : Exception
    {
        public VideoFile VideoFile { get; set; }

        public ComparisonException() { }

        public ComparisonException(string message) : base(message) { }

        public ComparisonException(string message, VideoFile videoFile)
            : base(message) =>
            VideoFile = videoFile;

        public ComparisonException(string message, Exception inner)
            : base(message, inner) { }

        public ComparisonException(
            string message,
            VideoFile videoFile,
            Exception inner)
            : base(message, inner) =>
            VideoFile = videoFile;

        protected ComparisonException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
