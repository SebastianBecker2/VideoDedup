namespace CustomSelectFileDlg.EventArgs
{
    using System;

    public class ContentRequestedEventArgs : EventArgs
    {
        public string? Path { get; set; }
        public RequestedEntryType RequestedEntryType { get; set; } =
            RequestedEntryType.FilesAndFolders;
        public IEnumerable<Entry>? Entries { get; set; }

        public ContentRequestedEventArgs(string? path) => Path = path;

        public ContentRequestedEventArgs(
            string? path,
            RequestedEntryType requestedEntryType)
        {
            Path = path;
            RequestedEntryType = requestedEntryType;
        }
    }
}
