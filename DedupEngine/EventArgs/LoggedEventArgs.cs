namespace DedupEngine.EventArgs
{
    using System;

    public class LoggedEventArgs : EventArgs
    {
        public LoggedEventArgs(string message) => Message = message;

        public string Message { get; set; }
    }
}
