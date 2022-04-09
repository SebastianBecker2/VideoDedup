namespace VideoComparer.MpvLib
{
    using System.Runtime.Serialization;

    [Serializable]
    public class MpvInitializationException : MpvException
    {
        private static readonly string ErrorText =
            "Unable to initialize libmpv handle.";

        public MpvInitializationException() : base(ErrorText) { }

        public MpvInitializationException(string videoFilePath)
            : base(ErrorText, videoFilePath) { }

        public MpvInitializationException(string message, string videoFilePath)
            : base(message, videoFilePath) { }

        public MpvInitializationException(string videoFilePath, Exception inner)
            : base(ErrorText, videoFilePath, inner) { }

        public MpvInitializationException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, videoFilePath, inner) { }

        protected MpvInitializationException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}
