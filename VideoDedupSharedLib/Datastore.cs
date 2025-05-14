namespace VideoDedupSharedLib
{
    using Microsoft.Data.Sqlite;

    public abstract class Datastore
    {
        public string DatastoreFilePath { get; }

        protected string ConnectionString =>
            $"Data Source={DatastoreFilePath}; Cache=Shared; Foreign Keys=true";

        // Thread safe lazy initialization
        private readonly Lazy<bool> lazyInitializer;

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

            lazyInitializer = new Lazy<bool>(() =>
            {
                Initialize();
                return true;
            }, isThreadSafe: true);
        }

        protected SqliteConnection OpenConnection()
        {
            _ = lazyInitializer.Value;

            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        protected abstract void Initialize();
    }
}
