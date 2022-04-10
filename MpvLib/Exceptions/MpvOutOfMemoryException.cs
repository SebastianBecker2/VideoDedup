namespace MpvLib.Exceptions
{
    using System.Runtime.Serialization;

    [Serializable]
    public class MpvOutOfMemoryException : MpvException
    {
        private static readonly string ErrorText =
            "Out of memory. Presumably due to bug in libmpv.";

        public MpvOutOfMemoryException() : base(ErrorText) { }

        public MpvOutOfMemoryException(string videoFilePath)
            : base(ErrorText, videoFilePath) { }

        public MpvOutOfMemoryException(string message, string videoFilePath)
            : base(message, videoFilePath) { }

        public MpvOutOfMemoryException(string videoFilePath, Exception inner)
            : base(ErrorText, videoFilePath, inner) { }

        public MpvOutOfMemoryException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, videoFilePath, inner) { }

        protected MpvOutOfMemoryException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}
