namespace DedupEngine
{
    using VideoDedupSharedLib;
    using VideoDedupSharedLib.ExtensionMethods.DateTimeExtensions;
    using VideoDedupSharedLib.Interfaces;

    internal sealed class EngineDatastore(string filePath)
        : Datastore(filePath)
    {
        protected override void CreateTables() => CreateVideoFilesTable();

        private void CreateVideoFilesTable()
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS VideoFiles ("
                + " VideoFileId INTEGER NOT NULL UNIQUE,"
                + " FileName TEXT NOT NULL,"
                + " FileSize INTEGER NOT NULL,"
                + " Duration INTEGER NOT NULL,"
                + " LastWriteTime INTEGER NOT NULL,"
                + " UNIQUE(FileName, FileSize, LastWriteTime)"
                + " ON CONFLICT IGNORE,"
                + " PRIMARY KEY(VideoFileId AUTOINCREMENT)"
                + ")";

            _ = command.ExecuteNonQuery();
        }

        public void InsertVideoFile(IVideoFile videoFile)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO VideoFiles"
                + " (FileName, FileSize, Duration, LastWriteTime)"
                + " VALUES "
                + " (@FileName, @FileSize, @Duration, @LastWriteTime)";

            _ = command.Parameters.AddWithValue(
                "@FileName",
                Path.GetFileName(videoFile.FilePath));
            _ = command.Parameters.AddWithValue(
                "@FileSize",
                videoFile.FileSize);
            _ = command.Parameters.AddWithValue(
                "@Duration",
                videoFile.Duration.TotalSeconds);
            _ = command.Parameters.AddWithValue(
                "@LastWriteTime",
                videoFile.LastWriteTime.ToUnixDate());

            _ = command.ExecuteNonQuery();
        }

        public TimeSpan? GetVideoFileDuration(IVideoFile videoFile)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT"
                + " Duration FROM VideoFiles"
                + " WHERE FileName IS (@FileName)"
                + " AND FileSize IS (@FileSize)"
                + " AND LastWriteTime IS (@LastWriteTime)";

            _ = command.Parameters.AddWithValue(
                "@FileName",
                Path.GetFileName(videoFile.FilePath));
            _ = command.Parameters.AddWithValue(
                "@FileSize",
                videoFile.FileSize);
            _ = command.Parameters.AddWithValue(
                "@LastWriteTime",
                videoFile.LastWriteTime.ToUnixDate());

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return TimeSpan.FromSeconds(reader.GetInt64(0));
        }
    }
}
