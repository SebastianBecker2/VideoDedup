namespace VideoDedupClient
{
    internal class LogEntry
    {
        public LogEntry() : this("", LogEntryStatus.Requested)
        {
        }

        public LogEntry(string message) : this(message, LogEntryStatus.Present)
        {
        }

        public LogEntry(string message, LogEntryStatus status)
        {
            Message = message;
            Status = status;
        }

        public string Message { get; set; }
        public LogEntryStatus Status { get; set; }
    }
}
