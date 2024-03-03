namespace DuplicateManager.EventArgs
{
    using System;
    using VideoDedupGrpc;

    public class DuplicateAddedEventArgs(
        DuplicateData duplicate,
        int count)
        : EventArgs
    {
        public DuplicateData Duplicate { get; } = duplicate;
        public int DuplicateCount { get; } = count;
    }
}
