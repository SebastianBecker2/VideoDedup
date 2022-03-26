namespace VideoDedupSharedLib
{
    using System;
    using System.Data.SQLite;
    using System.IO;

    public abstract class Datastore
    {
        // The Entity Framework provider type
        // 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'
        // for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. 
        // Make sure the provider assembly is available to the running application. 
        // See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.
#pragma warning disable IDE0051 // Remove unused private members
        private static void FixEfProviderServicesProblem() =>
#pragma warning restore IDE0051 // Remove unused private members
            _ = System.Data.Entity.SqlServer.SqlProviderServices.Instance;

        protected string DatastoreFilePath { get; private set; }

        protected string ConnectionString =>
            $"URI=file:{DatastoreFilePath};foreign keys=true";

        private bool isInitialized;

        public Datastore(string filePath)
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

        protected SQLiteConnection OpenConnection()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                CreateTables();
            }

            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        protected abstract void CreateTables();
    }
}
