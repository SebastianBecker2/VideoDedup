namespace DuplicateManager
{
    using System;

    public class FileSavedEventArgs : EventArgs
    {
        public int DuplicateCount { get; set; }
        public string FilePath { get; set; }
    }
}
