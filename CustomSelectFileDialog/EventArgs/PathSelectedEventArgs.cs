namespace CustomSelectFileDlg.EventArgs
{
    using System;

    public class PathSelectedEventArgs : EventArgs
    {
        public string? Path { get; set; }
        public bool IsValid { get; set; } = true;

        public PathSelectedEventArgs(string? path) => Path = path;
    }
}
