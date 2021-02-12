namespace VideoDedupShared
{
    using System.Collections.Generic;

    public interface IFolderSettings
    {
        string BasePath { get; }
        IEnumerable<string> ExcludedDirectories { get; }
        IEnumerable<string> FileExtensions { get; }
        bool Recursive { get; }
        bool MonitorChanges { get; }
    }
}
