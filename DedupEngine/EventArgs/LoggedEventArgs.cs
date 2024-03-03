namespace DedupEngine.EventArgs
{
    using System;

    public class LoggedEventArgs(string message) : EventArgs
    {
        public string Message { get; set; } = message;
    }
}
