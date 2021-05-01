namespace VideoDedup
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using VideoDedupShared;
    using Wcf.Contracts.Data;
    using Wcf.Contracts.Services;

    public class WcfProxy
       : ClientBase<IVideoDedupProvider>,
         IVideoDedupProvider
    {
        public WcfProxy(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public Wcf.Contracts.Data.ConfigData GetConfig() => Channel.GetConfig();
        public StatusData GetCurrentStatus() => Channel.GetCurrentStatus();
        public DuplicateData GetDuplicate() => Channel.GetDuplicate();
        public void ResolveDuplicate(Guid duplicateId,
            ResolveOperation resolveOperation) =>
            Channel.ResolveDuplicate(duplicateId, resolveOperation);
        public void SetConfig(Wcf.Contracts.Data.ConfigData configData) =>
            Channel.SetConfig(configData);
        public void DiscardDuplicates() => Channel.DiscardDuplicates();
        public LogData GetLogEntries(Guid logToken, int start, int count) =>
            Channel.GetLogEntries(logToken, start, count);
        public CustomVideoComparisonStartData StartCustomVideoComparison(
            CustomVideoComparisonData customVideoCompareData) =>
            Channel.StartCustomVideoComparison(customVideoCompareData);
        public CustomVideoComparisonStatusData GetVideoComparisonStatus(
            Guid videoCompareToken, int imageComparisonIndex = 0) =>
            Channel.GetVideoComparisonStatus(
                videoCompareToken,
                imageComparisonIndex);
        public void CancelCustomVideoComparison(Guid videoCompareToken) =>
            Channel.CancelCustomVideoComparison(videoCompareToken);
    }
}
