namespace DuplicateManager
{
    using System;

    public class FileLoadedEventArgs : EventArgs
    {
        public int DuplicateCount { get; set; }
        public string FilePath { get; set; }
    }
}
