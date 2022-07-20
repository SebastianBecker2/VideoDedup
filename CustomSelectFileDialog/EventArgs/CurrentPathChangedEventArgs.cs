namespace CustomSelectFileDlg.EventArgs
{
    using System;

    internal class CurrentPathChangedEventArgs : EventArgs
    {
        public string? Path { get; set; }

        public CurrentPathChangedEventArgs(string? path) => Path = path;
    }
}
