namespace VideoDedupConsole.DuplicateManagement
{
    using System;
    using VideoDedupShared;
    using Wcf.Contracts.Data;

    public class DuplicateResolvedEventArgs : EventArgs
    {
        public DuplicateData Duplicate { get; set; }
        public ResolveOperation Operation { get; set; }
    }
}
