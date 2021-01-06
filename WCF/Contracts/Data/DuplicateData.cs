namespace Wcf.Contracts.Data
{
    using System;
    using System.Runtime.Serialization;
    using VideoDedupShared;

    [DataContract]
    public class DuplicateData
    {
        [DataMember]
        public Guid DuplicateId { get; set; }
        [DataMember]
        public VideoFile File1 { get; set; }
        [DataMember]
        public VideoFile File2 { get; set; }
    }
}
