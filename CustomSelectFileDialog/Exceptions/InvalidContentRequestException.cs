namespace CustomSelectFileDlg.Exceptions
{
    using System.Runtime.Serialization;
    using Properties;

    [Serializable]
    public class InvalidContentRequestException : Exception
    {
        public InvalidContentRequestException()
        : base(Resources.InvalidContentRequestExceptionDefaultMessage)
        {
        }

        public InvalidContentRequestException(string message)
            : base(message)
        {
        }

        public InvalidContentRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidContentRequestException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
