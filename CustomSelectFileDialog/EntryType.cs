namespace CustomSelectFileDlg
{
    public enum EntryType
    {
        Folder = 0,
        File = 1,
    }

    public static class EntryTypeExtensions
    {
        public static bool Matches(
            this EntryType entryType,
            RequestedEntryType requestedEntryType)
        {
            if (requestedEntryType == RequestedEntryType.FilesAndFolders)
            {
                return true;
            }

            if (requestedEntryType == RequestedEntryType.Folders)
            {
                return entryType == EntryType.Folder;
            }

            return entryType == EntryType.File;
        }
    }
}
