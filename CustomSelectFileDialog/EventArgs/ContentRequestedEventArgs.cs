namespace CustomSelectFileDlg.EventArgs
{
    using System;

    public class ContentRequestedEventArgs : EventArgs
    {
        public string? Path { get; set; }
        public bool FoldersOnly { get; set; }
        public IEnumerable<Entry>? Entries { get; set; }

        public ContentRequestedEventArgs(string? path) => Path = path;

        public ContentRequestedEventArgs(string? path, bool foldersOnly)
        {
            Path = path;
            FoldersOnly = foldersOnly;
        }
    }
}
