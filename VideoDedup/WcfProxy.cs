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
        public DuplicateData GetDuplicate(int thumbnailCount) =>
            Channel.GetDuplicate(thumbnailCount);
        public LogData GetLogEvents(LogToken logToken) =>
            Channel.GetLogEvents(logToken);
        public void ResolveDuplicate(Guid duplicateId,
            ResolveOperation resolveOperation) =>
            Channel.ResolveDuplicate(duplicateId, resolveOperation);
        public void SetConfig(Wcf.Contracts.Data.ConfigData configData) =>
            Channel.SetConfig(configData);
    }

}
