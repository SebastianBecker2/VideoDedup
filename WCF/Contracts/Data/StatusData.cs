namespace Wcf.Contracts.Data
{
    using System;
    using System.Runtime.Serialization;
    using VideoDedupShared;

    [DataContract]
    public class StatusData
    {
        [DataMember]
        public Guid StatusId { get; set; }
        [DataMember]
        public int CurrentProgress { get; set; }
        [DataMember]
        public int MaximumProgress { get; set; }
        [DataMember]
        public ProgressStyle ProgressStyle { get; set; }
        [DataMember]
        public string StatusMessage { get; set; }
        [DataMember]
        public TimeSpan Durtion { get; set; }
        [DataMember]
        public int DuplicateCount { get; set; }
        [DataMember]
        public int LogCount { get; set; }
        [DataMember]
        public Guid LogToken { get; set; }

        public StatusData Clone() => (StatusData)MemberwiseClone();
    }
}
