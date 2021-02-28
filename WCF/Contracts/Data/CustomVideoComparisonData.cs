namespace Wcf.Contracts.Data
{
    using System.Runtime.Serialization;
    using VideoDedupShared;

    [DataContract]
    public class CustomVideoComparisonData : IImageComparisonSettings
    {
        [DataMember]
        public int MaxImageCompares { get; set; }
        [DataMember]
        public int MaxDifferentImages { get; set; }
        [DataMember]
        public int MaxImageDifferencePercent { get; set; }
        [DataMember]
        public string LeftFilePath { get; set; }
        [DataMember]
        public string RightFilePath { get; set; }
        [DataMember]
        public bool AlwaysLoadAllImages { get; set; }
    }
}
