namespace VideoDedupShared.SqlDataReaderExtension
{
    using System.Data.SQLite;

    public static class SqlDataReaderExtension
    {
        public static byte[] GetBytes(
            this SQLiteDataReader reader,
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
