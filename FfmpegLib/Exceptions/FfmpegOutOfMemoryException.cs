namespace FfmpegLib.Exceptions
{
    [Serializable]
    public class FfmpegOutOfMemoryException : FfmpegException
    {
        private static readonly string ErrorText =
            "Out of memory. Presumably due to bug in ffmpeg.";

        public FfmpegOutOfMemoryException() : base(ErrorText) { }

        public FfmpegOutOfMemoryException(string videoFilePath)
            : base(ErrorText, videoFilePath) { }

        public FfmpegOutOfMemoryException(string message, string videoFilePath)
            : base(message, videoFilePath) { }

        public FfmpegOutOfMemoryException(string videoFilePath, Exception inner)
            : base(ErrorText, videoFilePath, inner) { }

        public FfmpegOutOfMemoryException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, videoFilePath, inner) { }
    }
}
