namespace MpvLib.Exceptions
{
    [Serializable]
    public class MpvFileNotFoundException : MpvException
    {
        private static readonly string ErrorText =
            "Operation failed. File not found.";

        public MpvFileNotFoundException() : base(ErrorText) { }

        public MpvFileNotFoundException(string videoFilePath)
            : base(ErrorText, videoFilePath) { }

        public MpvFileNotFoundException(string message, string videoFilePath)
            : base(message, videoFilePath) { }

        public MpvFileNotFoundException(string videoFilePath, Exception inner)
            : base(ErrorText, videoFilePath, inner) { }

        public MpvFileNotFoundException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, videoFilePath, inner) { }
    }
}
