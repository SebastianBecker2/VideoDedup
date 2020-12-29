namespace Wcf.Contracts.Data
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using VideoDedupShared;

    [DataContract]
    public class LogData
    {
        [DataMember]
        public LogToken LogToken { get; set; }

        [DataMember]
        public IEnumerable<string> LogItems { get; set; }
    }
}
