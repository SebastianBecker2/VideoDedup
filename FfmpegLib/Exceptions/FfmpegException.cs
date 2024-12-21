namespace FfmpegLib.Exceptions
{
    [Serializable]
    public class FfmpegException : Exception
    {
        public string? VideoFilePath { get; set; }

        public FfmpegException() { }

        public FfmpegException(string message) : base(message) { }

        public FfmpegException(string message, string videoFilePath)
            : base(message) =>
            VideoFilePath = videoFilePath;

        public FfmpegException(string message, Exception inner)
            : base(message, inner) { }

        public FfmpegException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, inner) =>
            VideoFilePath = videoFilePath;
    }
}
