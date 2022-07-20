namespace CustomSelectFileDlg.EventArgs
{
    using System;

    public class ContentRequestedEventArgs : EventArgs
    {
        public string? Path { get; set; }

        public ContentRequestedEventArgs(string? path) => Path = path;
    }
}
