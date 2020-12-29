namespace VideoDedupConsole
{
    using System;
    using VideoDedupShared;
    using Wcf.Contracts.Data;
    using Wcf.Contracts.Services;

    internal class WcfService : IVideoDedupProvider
    {
        public ConfigData GetConfig() => Program.LoadConfig();

        public StatusData GetCurrentStatus() => Program.GetCurrentStatus();

        public DuplicateData GetDuplicate() => Program.GetDuplicate();

        public int GetDuplicateCount() => Program.GetDuplicateCount();

        public LogData GetLogEvents(LogToken logToken) =>
            Program.GetLogEntries(logToken);

        public void ResolveDuplicate(Guid duplicateId,
            ResolveOperation resolveOperation) =>
            Program.ResolveDuplicate(duplicateId, resolveOperation);

        public void SetConfig(ConfigData configData)
        {
            Program.SaveConfig(configData);
            Program.UpdateConfig(configData);
        }
    }
}
