namespace CustomSelectFileDlg.EventArgs
{
    using System;

    public class ContentRequestedEventArgs : EventArgs
    {
        public string? Path { get; set; }
        public RequestedEntryType RequestedEntryType { get; set; } =
            RequestedEntryType.FilesAndFolders;
        public string? SelectedFilter { get; set; }
        public IEnumerable<Entry>? Entries { get; set; }

        public ContentRequestedEventArgs(
            string? path,
            RequestedEntryType requestedEntryType,
            string? selectedFilter)
        {
            Path = path;
            RequestedEntryType = requestedEntryType;
            SelectedFilter = selectedFilter;
        }
    }
}
