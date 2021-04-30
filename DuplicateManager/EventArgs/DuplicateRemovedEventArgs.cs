namespace DuplicateManager
{
    using System;
    using Wcf.Contracts.Data;

    public class DuplicateRemovedEventArgs : EventArgs
    {
        public DuplicateData Duplicate { get; set; }
        public int DuplicateCount { get; set; }
    }
}
