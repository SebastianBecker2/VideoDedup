namespace DedupEngine
{
    using System.Data.SQLite;
    using System.IO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using VideoDedupSharedLib;
    using VideoDedupSharedLib.Interfaces;
    using VideoDedupSharedLib.ExtensionMethods.SqlDataReaderExtensions;
    using VideoDedupSharedLib.ExtensionMethods.DateTimeExtensions;
    using VideoDedupGrpc;

    internal class EngineDatastore : Datastore
    {
        public EngineDatastore(string filePath) : base(filePath) { }

        protected override void CreateTables()
        {
            CreateVideoFilesTable();
            CreateImagesTable();
        }

        private void CreateVideoFilesTable()
        {
            using var connection = OpenConnection();
            using var command = new SQLiteCommand(connection);
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
            using var command = new SQLiteCommand(connection);
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
            using var command = new SQLiteCommand(connection);
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

        private void CreateImagesTable()
        {
            using var connection = OpenConnection();
            using var command = new SQLiteCommand(connection);
            command.CommandText = "CREATE TABLE IF NOT EXISTS Images ("
                + " Numerator INTEGER NOT NULL,"
                + " Denominator INTEGER NOT NULL,"
                + " VideoFileId INTEGER NOT NULL,"
                + " ImageSize INTEGER NOT NULL,"
                + " Data BLOB,"
                + " PRIMARY KEY(Numerator, Denominator, VideoFileId)"
                + " ON CONFLICT IGNORE,"
                + " FOREIGN KEY(VideoFileId) REFERENCES VideoFiles(VideoFileId)"
                + " ON DELETE CASCADE"
                + ")";

            _ = command.ExecuteNonQuery();
        }

        public void InsertImage(
            ImageIndex imageId,
            byte[]? bytes,
            IVideoFile videoFile)
        {
            using var connection = OpenConnection();
            using var command = new SQLiteCommand(connection);
            command.CommandText = "INSERT INTO Images"
                    + " (Numerator, Denominator, ImageSize, Data, VideoFileId)"
                    + " VALUES"
                    + " (@Numerator, @Denominator, @ImageSize, @Data, ("
                    + " SELECT"
                    + "   VideoFileId FROM VideoFiles"
                    + "   WHERE FileName IS (@FileName)"
                    + "   AND FileSize IS (@FileSize)"
                    + "   AND LastWriteTime IS (@LastWriteTime)"
                    + "))";

            _ = command.Parameters.AddWithValue(
                "@Numerator",
                imageId.Numerator);
            _ = command.Parameters.AddWithValue(
                "@Denominator",
                imageId.Denominator);
            _ = command.Parameters.AddWithValue(
                "@ImageSize",
                bytes?.Length ?? 0);
            _ = command.Parameters.AddWithValue(
                "@Data",
                bytes);
            _ = command.Parameters.AddWithValue(
                "@FileName",
                Path.GetFileName(videoFile.FilePath));
            _ = command.Parameters.AddWithValue(
                "@FileSize",
                videoFile.FileSize);
            _ = command.Parameters.AddWithValue(
                "@LastWriteTime",
                videoFile.LastWriteTime.ToUnixDate());

            _ = command.ExecuteNonQuery();
        }

        public IEnumerable<(ImageIndex index, byte[]? bytes)> GetImages(
            IEnumerable<ImageIndex> imageIndices,
            IVideoFile videoFile)
        {
            if (!imageIndices.Any())
            {
                yield break;
            }

            using var connection = OpenConnection();
            using var command = new SQLiteCommand(connection);
            command.CommandText = "SELECT"
                + " Numerator, Denominator, ImageSize, Data"
                + " FROM Images"
                + " INNER JOIN VideoFiles"
                + " ON Images.VideoFileId IS VideoFiles.VideoFileId"
                + " AND VideoFiles.FileName IS (@FileName)"
                + " AND VideoFiles.FileSize IS (@FileSize)"
                + " AND VideoFiles.LastWriteTime IS (@LastWriteTime)"
                + " AND ("
                + string.Join(
                    " OR ",
                    imageIndices.Select((_, i) =>
                        $"(Numerator IS (@Numerator{i})"
                        + $" AND Denominator IS (@Denominator{i}))"))
                + ")";

            _ = command.Parameters.AddWithValue(
                "@FileName",
                Path.GetFileName(videoFile.FilePath));
            _ = command.Parameters.AddWithValue(
                "@FileSize",
                videoFile.FileSize);
            _ = command.Parameters.AddWithValue(
                "@LastWriteTime",
                videoFile.LastWriteTime.ToUnixDate());

            var counter = 0;
            foreach (var index in imageIndices)
            {
                _ = command.Parameters.AddWithValue(
                    $"@Numerator{counter}",
                    index.Numerator);
                _ = command.Parameters.AddWithValue(
                    $"@Denominator{counter}",
                    index.Denominator);
                counter++;
            }

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var index = new ImageIndex
                {
                    Numerator = reader.GetInt32(0),
                    Denominator = reader.GetInt32(1)
                };
                var size = reader.GetInt32(2);
                var bytes = size > 0 ? reader.GetBytes(3, size) : null;
                yield return (index, bytes);
            }
        }
    }
}
