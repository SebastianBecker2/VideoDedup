namespace VideoDedupConsole.DuplicateManagement
{
    using System;
    using Wcf.Contracts.Data;

    public class DuplicateAddedEventArgs : EventArgs
    {
        public DuplicateData Duplicate { get; set; }
        public int DuplicateCount { get; set; }
    }
}
