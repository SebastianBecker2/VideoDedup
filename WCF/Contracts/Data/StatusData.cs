namespace Wcf.Contracts.Data
{
    using System;
    using System.Runtime.Serialization;

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
        public string StatusMessage { get; set; }
        [DataMember]
        public TimeSpan Durtion { get; set; }
        [DataMember]
        public int DuplicateCount { get; set; }

        public StatusData Clone() => (StatusData)MemberwiseClone();
    }
}