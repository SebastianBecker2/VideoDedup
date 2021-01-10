namespace Wcf.Contracts.Data
{
    using System;
    using System.Runtime.Serialization;
    using VideoDedupShared;

    [DataContract]
    public class StatusData
    {
        [DataMember]
        public OperationInfo Operation { get; set; }
        [DataMember]
        public int DuplicateCount { get; set; }
        [DataMember]
        public int LogCount { get; set; }
        [DataMember]
        public Guid LogToken { get; set; }
    }
}
