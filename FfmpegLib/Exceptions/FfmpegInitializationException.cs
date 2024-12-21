namespace FfmpegLib.Exceptions
{
    [Serializable]
    public class FfmpegInitializationException : FfmpegException
    {
        private static readonly string ErrorText =
            "Unable to initialize libmpv handle.";

        public FfmpegInitializationException() : base(ErrorText) { }

        public FfmpegInitializationException(string videoFilePath)
            : base(ErrorText, videoFilePath) { }

        public FfmpegInitializationException(string message, string videoFilePath)
            : base(message, videoFilePath) { }

        public FfmpegInitializationException(string videoFilePath, Exception inner)
            : base(ErrorText, videoFilePath, inner) { }

        public FfmpegInitializationException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, videoFilePath, inner) { }
    }
}
