namespace DedupEngine.MpvLib
{
    using System;

    [Serializable]
    public class MpvOperationException : MpvException
    {
        public MpvOperationException() { }

        public MpvOperationException(string message) : base(message) { }

        public MpvOperationException(string message, string videoFilePath)
            : base(message, videoFilePath) { }

        public MpvOperationException(string message, Exception inner)
            : base(message, inner) { }

        public MpvOperationException(
            string message,
            string videoFilePath,
            Exception inner)
            : base(message, videoFilePath, inner) { }

        protected MpvOperationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
