namespace DedupEngine.MpvLib
{
    using System;

    internal class MpvException : Exception
    {
        public MpvException() : base() { }
        public MpvException(string message) : base(message) { }
    }
}
