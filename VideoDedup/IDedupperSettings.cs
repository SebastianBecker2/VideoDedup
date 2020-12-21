namespace VideoDedup
{
    public interface IDedupperSettings :
        IDurationComparisonSettings,
        IThumbnailComparisonSettings,
        IFolderSettings
    {
        IDedupperSettings Copy();
    }
}
