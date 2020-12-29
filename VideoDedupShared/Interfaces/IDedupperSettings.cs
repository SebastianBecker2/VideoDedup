namespace VideoDedupShared
{
    public interface IDedupperSettings :
        IDurationComparisonSettings,
        IThumbnailComparisonSettings,
        IFolderSettings
    {
        IDedupperSettings Copy();
    }
}
