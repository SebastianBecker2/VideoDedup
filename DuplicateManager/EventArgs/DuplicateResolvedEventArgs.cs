namespace DuplicateManager.EventArgs
{
    using System;
    using VideoDedupGrpc;

    public class DuplicateResolvedEventArgs : EventArgs
    {
        public DuplicateResolvedEventArgs(
            DuplicateData duplicate,
            ResolveOperation operation)
        {
            Duplicate = duplicate;
            Operation = operation;
        }

        public DuplicateData Duplicate { get; set; }
        public ResolveOperation Operation { get; set; }
    }
}
