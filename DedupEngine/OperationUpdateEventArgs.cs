namespace DedupEngine
{
    using System;
    using VideoDedupShared;

    public class OperationUpdateEventArgs : EventArgs
    {
        public OperationType Type { get; set; }
        public int Counter { get; set; }
        public int MaxCount { get; set; }
        public ProgressStyle Style { get; set; }
        public DateTime StartTime { get; set; }
    }
}
