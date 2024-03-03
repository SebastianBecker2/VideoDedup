namespace VideoDedupClient
{
    internal sealed class LogEntry(string message, LogEntryStatus status)
    {
        public LogEntry() : this("", LogEntryStatus.Requested)
        {
        }

        public LogEntry(string message) : this(message, LogEntryStatus.Present)
        {
        }

        public string Message { get; set; } = message;
        public LogEntryStatus Status { get; set; } = status;
    }
}
