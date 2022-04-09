namespace DedupEngine.EventArgs
{
    using System;
    using static VideoDedupGrpc.OperationInfo.Types;

    public class OperationUpdateEventArgs : EventArgs
    {
        public OperationType Type { get; set; }
        public int Counter { get; set; }
        public int MaxCount { get; set; }
        public ProgressStyle Style { get; set; }
        public DateTime StartTime { get; set; }
    }
}
