namespace Wcf.Contracts.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using VideoDedupShared;

    [DataContract]
    public class CustomVideoComparisonStatusData
    {
        [DataMember]
        public Guid Token { get; set; }
        [DataMember]
        public VideoFile LeftVideoFile { get; set; }
        [DataMember]
        public VideoFile RightVideoFile { get; set; }
        [DataMember]
        public IEnumerable<Tuple<int, ImageComparisonResult>> ImageComparisons
        { get; set; }
        [DataMember]
        public VideoComparisonResult VideoCompareResult { get; set; }
    }
}
