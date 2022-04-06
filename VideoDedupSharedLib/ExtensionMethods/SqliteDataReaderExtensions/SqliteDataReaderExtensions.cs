namespace VideoDedupSharedLib.ExtensionMethods.SqliteDataReaderExtensions
{
    using Microsoft.Data.Sqlite;

    public static class SqliteDataReaderExtensions
    {
        public static byte[] GetBytes(
            this SqliteDataReader reader,
            int index,
            int length)
        {
            var data = new byte[length];
            var readCount = 0L;
            while (readCount < length)
            {
                readCount += reader.GetBytes(
                    index,
                    readCount,
                    data,
                    (int)readCount,
                    length);
            }
            return data;
        }
    }
}
