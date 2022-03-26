namespace DuplicateManager.EventArgs
{
    using System;
    using VideoDedupGrpc;

    public class DuplicateRemovedEventArgs : EventArgs
    {
        public DuplicateRemovedEventArgs(DuplicateData duplicate, int count)
        {
            Duplicate = duplicate;
            DuplicateCount = count;
        }

        public DuplicateData Duplicate { get; set; }
        public int DuplicateCount { get; set; }
    }
}
