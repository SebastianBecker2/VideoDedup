namespace CustomSelectFileDlg.EventArgs
{
    using System;
    using System.Collections.Generic;

    internal class RootFoldersRequestedEventArgs : EventArgs
    {
        public IEnumerable<string>? RootFolders { get; set; }
    }
}
