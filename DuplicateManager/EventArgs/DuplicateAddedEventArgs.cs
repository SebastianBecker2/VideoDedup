namespace DuplicateManager.EventArgs
{
    using System;
    using VideoDedupGrpc;

    public class DuplicateAddedEventArgs : EventArgs
    {
        public DuplicateAddedEventArgs(DuplicateData duplicate, int count)
        {
            Duplicate = duplicate;
            DuplicateCount = count;
        }

        public DuplicateData Duplicate { get; }
        public int DuplicateCount { get; }
    }
}
