namespace Wcf.Contracts.Data
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class LogData
    {
        [DataMember]
        public IEnumerable<string> LogEntries { get; set; } = new List<string>();
    }
}
