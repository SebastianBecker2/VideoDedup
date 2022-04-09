namespace VideoDedupSharedLib.ExtensionMethods.SqliteParameterCollectionExtensions
{
    using Microsoft.Data.Sqlite;

    public static class SqliteParameterCollectionExtensions
    {
        public static SqliteParameter AddWithOptionalValue(
            this SqliteParameterCollection collection,
            string? parameterName,
            object? value)
        {
            if (value is null)
            {
                return collection.AddWithValue(parameterName, DBNull.Value);
            }

            return collection.AddWithValue(parameterName, value);
        }
    }
}
