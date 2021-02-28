namespace Wcf.Contracts.Data
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class CustomVideoComparisonStartData
    {
        [DataMember]
        public Guid? ComparisonToken { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
