namespace VideoDedupSharedLib
{
    using System;
    using System.IO;
    using Microsoft.Data.Sqlite;

    public abstract class Datastore
    {
        public string DatastoreFilePath { get; }

        protected string ConnectionString =>
            $"Data Source={DatastoreFilePath}; Cache=Shared; Foreign Keys=true";

        private bool isInitialized;

        protected Datastore(string filePath)
        {
            // Validate argument filePath
            try
            {
                _ = Path.GetFullPath(filePath);
            }
            catch (Exception exc)
            {
                throw new AggregateException(
                    $"Validation of {nameof(filePath)} failed", exc);
            }

            DatastoreFilePath = filePath;
        }

        protected SqliteConnection OpenConnection()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                CreateTables();
            }

            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        protected abstract void CreateTables();
    }
}
