namespace VideoDedupShared
{
    public interface IDedupEngineSettings :
        IDurationComparisonSettings,
        IImageComparisonSettings,
        IFolderSettings
    {
        int SaveStateIntervalMinutes { get; }
    }
}
