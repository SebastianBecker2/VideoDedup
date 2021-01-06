namespace VideoDedupShared
{
    public interface IDedupperSettings :
        IDurationComparisonSettings,
        IImageComparisonSettings,
        IFolderSettings,
        IThumbnailSettings
    {
        IDedupperSettings Copy();
    }
}
