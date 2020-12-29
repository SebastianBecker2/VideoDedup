namespace VideoDedupConsole
{
    using System;
    using Wcf.Contracts.Data;

    internal class DuplicateWrapper
    {
        public DuplicateData InnerDuplicate { get; set; }
        public DateTime LastRequest { get; set; }
    }
}
