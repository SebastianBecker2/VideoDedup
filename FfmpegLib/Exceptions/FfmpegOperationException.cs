namespace FfmpegLib.Exceptions
{
    [Serializable]
    public class FfmpegOperationException : FfmpegException
    {
        public FfmpegOperationException() { }

        public FfmpegOperationException(string message) : base(message) { }

        public FfmpegOperationException(string message, string videoFilePath)
            : base(message, videoFilePath) { }

        public FfmpegOperationException(string message, Exception inner)
            : base(message, inner) { }

        public FfmpegOperationException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, videoFilePath, inner) { }
    }
}
