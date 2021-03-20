namespace VideoDedupConsole.DuplicateManagement
{
    using System;
    using Wcf.Contracts.Data;

    public class FileLoadedProgressEventArgs : EventArgs
    {
        public DuplicateData Duplicate { get; set; }
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public string FilePath { get; set; }
        public DateTime StartTime { get; set; }
    }
}
