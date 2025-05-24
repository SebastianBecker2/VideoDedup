namespace FfmpegLib.Exceptions
{
    [Serializable]
    public class FfmpegFileNotFoundException : FfmpegException
    {
        private static readonly string ErrorText =
            "Operation failed. File not found.";

        public FfmpegFileNotFoundException() : base(ErrorText) { }

        public FfmpegFileNotFoundException(string videoFilePath)
            : base(ErrorText, videoFilePath) { }

        public FfmpegFileNotFoundException(string message, string videoFilePath)
            : base(message, videoFilePath) { }

        public FfmpegFileNotFoundException(string videoFilePath, Exception inner)
            : base(ErrorText, videoFilePath, inner) { }

        public FfmpegFileNotFoundException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, videoFilePath, inner) { }
    }
}
