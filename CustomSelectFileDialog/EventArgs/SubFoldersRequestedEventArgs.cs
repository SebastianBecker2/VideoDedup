namespace CustomSelectFileDlg.EventArgs
{
    using System;
    using System.Collections.Generic;

    internal class SubFoldersRequestedEventArgs : EventArgs
    {
        public string? Path { get; set; }
        public IEnumerable<string>? SubFolders { get; set; }

        public SubFoldersRequestedEventArgs(string? path) =>
            Path = path;
    }
}
