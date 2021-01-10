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

        public DuplicateData GetDuplicate() =>
            Program.GetDuplicate();

        public void ResolveDuplicate(Guid duplicateId,
            ResolveOperation resolveOperation) =>
            Program.ResolveDuplicate(duplicateId, resolveOperation);

        public void SetConfig(ConfigData configData)
        {
            Program.SaveConfig(configData);
            Program.UpdateConfig(configData);
        }

        public void DiscardDuplicates() =>
            Program.DiscardDuplicates();

        public LogData GetLogEntries(Guid logToken, int start, int count) =>
            Program.GetLogEntries(logToken, start, count);
    }
}
