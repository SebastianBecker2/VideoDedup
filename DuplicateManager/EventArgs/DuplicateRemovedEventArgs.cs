namespace DuplicateManager.EventArgs
{
    using System;
    using VideoDedupGrpc;

    public class DuplicateRemovedEventArgs(
        DuplicateData duplicate,
        int count)
        : EventArgs
    {
        public DuplicateData Duplicate { get; set; } = duplicate;
        public int DuplicateCount { get; set; } = count;
    }
}
