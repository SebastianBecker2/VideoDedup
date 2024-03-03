namespace MpvLib.Exceptions
{
    [Serializable]
    public class MpvException : Exception
    {
        public string? VideoFilePath { get; set; }

        public MpvException() { }

        public MpvException(string message) : base(message) { }

        public MpvException(string message, string videoFilePath)
            : base(message) =>
            VideoFilePath = videoFilePath;

        public MpvException(string message, Exception inner)
            : base(message, inner) { }

        public MpvException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, inner) =>
            VideoFilePath = videoFilePath;
    }
}
