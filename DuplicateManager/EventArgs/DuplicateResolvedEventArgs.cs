namespace DuplicateManager.EventArgs
{
    using System;
    using VideoDedupGrpc;

    public class DuplicateResolvedEventArgs(
        DuplicateData duplicate,
        ResolveOperation operation)
        : EventArgs
    {
        public DuplicateData Duplicate { get; set; } = duplicate;
        public ResolveOperation Operation { get; set; } = operation;
    }
}
