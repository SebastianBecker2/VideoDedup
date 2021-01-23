namespace VideoDedupShared.MpvLib
{
    using System;

    public class MpvException : Exception
    {
        public MpvException() : base() { }
        public MpvException(string message) : base(message) { }
    }
}
