namespace Wcf.Contracts.Services
{
    using System;
    using System.ServiceModel;
    using VideoDedupShared;
    using Wcf.Contracts.Data;

    // Separate into different providers?
    // IConfigProvider?
    // IStatusProvider?
    // ILogProvider?
    // IDuplicateProvider?

    [ServiceContract]
    public interface IVideoDedupProvider
    {
        [OperationContract]
        StatusData GetCurrentStatus();

        [OperationContract]
        ConfigData GetConfig();

        [OperationContract]
        void SetConfig(ConfigData configData);

        [OperationContract]
        void ResolveDuplicate(Guid duplicateId,
            ResolveOperation resolveOperation);

        [OperationContract]
        DuplicateData GetDuplicate();

        [OperationContract]
        void DiscardDuplicates();

        [OperationContract]
        LogData GetLogEntries(Guid logToken, int start, int count);

        [OperationContract]
        CustomVideoComparisonStartData StartCustomVideoComparison(
            CustomVideoComparisonData customVideoComparisonData);

        [OperationContract]
        CustomVideoComparisonStatusData GetVideoComparisonStatus(
            Guid videoComparisonToken,
            int imageComparisonIndex = 0);

        [OperationContract]
        void CancelCustomVideoComparison(
            Guid videoComparisonToken);
    }
}
