namespace VideoComparer
{
    using System;
    using System.Globalization;
    using FfmpegLib;
    using Microsoft.Data.Sqlite;
    using VideoDedupSharedLib;
    using VideoDedupSharedLib.ExtensionMethods.DateTimeExtensions;
    using VideoDedupSharedLib.ExtensionMethods.SqliteDataReaderExtensions;
    using VideoDedupSharedLib.ExtensionMethods.SqliteParameterCollectionExtensions;
    using VideoDedupSharedLib.Interfaces;

    internal sealed class ComparerDatastore(string filePath)
        : Datastore(filePath)
    {
        private static readonly int DatastoreVersion = 1;
        private static readonly Dictionary<Tuple<FrameIndex, IVideoFile>, byte[]?>
            FrameCache = [];
        private static readonly object FrameCacheMutex = new();

        protected override void Initialize()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            CreateComparerMetaTable(connection);
            UpgradeDatastore(connection);
            CreateFramesTable(connection);
            CreateFramesIndexes(connection);
        }

        private static void UpgradeDatastore(SqliteConnection connection)
        {
            // Version check
            if (DatastoreVersion <= GetDatastoreVersion(connection))
            {
                return;
            }

            // Upgrade process
            {
                using var command = connection.CreateCommand();
                command.CommandText = "DROP TABLE IF EXISTS Frames";
                _ = command.ExecuteNonQuery();
            }

            // Update version number
            {
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT OR REPLACE INTO ComparerMeta" +
                    $" (id, Version) VALUES (1, {DatastoreVersion})";
                _ = command.ExecuteNonQuery();
            }
        }

        private static void CreateComparerMetaTable(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS ComparerMeta (" +
                " id INTEGER PRIMARY KEY CHECK (id = 1)," +
                " Version INTEGER NOT NULL" +
                ")";
            _ = command.ExecuteNonQuery();
        }

        private static void CreateFramesTable(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS Frames ("
                + " Numerator INTEGER NOT NULL,"
                + " Denominator INTEGER NOT NULL,"
                + " VideoFileId INTEGER NOT NULL,"
                + " FrameSize INTEGER NOT NULL,"
                + " Data BLOB,"
                + " PRIMARY KEY(Numerator, Denominator, VideoFileId)"
                + " ON CONFLICT IGNORE,"
                + " FOREIGN KEY(VideoFileId) REFERENCES VideoFiles(VideoFileId)"
                + " ON DELETE CASCADE"
                + ")";
            _ = command.ExecuteNonQuery();
        }

        private static void CreateFramesIndexes(SqliteConnection connection)
        {
            {
                using var command = connection.CreateCommand();
                command.CommandText = "CREATE INDEX IF NOT EXISTS " +
                    "idx_Framess_VideoFileId " +
                    "ON Frames (VideoFileId);";
                _ = command.ExecuteNonQuery();
            }

            {
                using var command = connection.CreateCommand();
                command.CommandText = "CREATE INDEX IF NOT EXISTS " +
                    "idx_Frames_Numerator_Denominator_FrameSize_Data " +
                    "ON Frames (Numerator, Denominator, FrameSize, Data);";
                _ = command.ExecuteNonQuery();
            }
        }

        private static int GetDatastoreVersion(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT"
                + " Version"
                + " FROM ComparerMeta"
                + " WHERE id = 1";

            var result = command.ExecuteScalar();
            return result != null
                ? Convert.ToInt32(result, CultureInfo.InvariantCulture)
                : 0;
        }

        public void InsertFrame(
            FrameIndex frameId,
            byte[]? bytes,
            IVideoFile videoFile)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Frames"
                    + " (Numerator, Denominator, FrameSize, Data, VideoFileId)"
                    + " VALUES"
                    + " (@Numerator, @Denominator, @FrameSize, @Data, ("
                    + " SELECT"
                    + "   VideoFileId FROM VideoFiles"
                    + "   WHERE FileName IS (@FileName)"
                    + "   AND FileSize IS (@FileSize)"
                    + "   AND LastWriteTime IS (@LastWriteTime)"
                    + "))";

            _ = command.Parameters.AddWithValue(
                "@Numerator",
                frameId.Numerator);
            _ = command.Parameters.AddWithValue(
                "@Denominator",
                frameId.Denominator);
            _ = command.Parameters.AddWithValue(
                "@FrameSize",
                bytes?.Length ?? 0);
            _ = command.Parameters.AddWithOptionalValue(
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

            lock (FrameCacheMutex)
            {
                _ = FrameCache.TryAdd(Tuple.Create(frameId, videoFile), bytes);
            }
        }

        public IEnumerable<(FrameIndex index, byte[]? bytes)> GetFrames(
            IEnumerable<FrameIndex> frameIndices,
            IVideoFile videoFile)
        {
            if (!frameIndices.Any())
            {
                yield break;
            }

            List<FrameIndex> uncachedFrameIndices = [];
            lock (FrameCacheMutex)
            {
                foreach (var frameIndex in frameIndices)
                {
                    if (FrameCache.TryGetValue(
                        Tuple.Create(frameIndex, videoFile),
                        out var data))
                    {
                        yield return (frameIndex, data);
                    }
                    else
                    {
                        uncachedFrameIndices.Add(frameIndex);
                    }
                }
            }

            if (uncachedFrameIndices.Count == 0)
            {
                yield break;
            }

            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT"
                + " Numerator, Denominator, FrameSize, Data"
                + " FROM Frames"
                + " INNER JOIN VideoFiles"
                + " ON Frames.VideoFileId IS VideoFiles.VideoFileId"
                + " AND VideoFiles.FileName IS (@FileName)"
                + " AND VideoFiles.FileSize IS (@FileSize)"
                + " AND VideoFiles.LastWriteTime IS (@LastWriteTime)"
                + " AND ("
                + string.Join(
                    " OR ",
                    uncachedFrameIndices.Select((_, i) =>
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
            foreach (var index in uncachedFrameIndices)
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
            lock (FrameCacheMutex)
            {
                while (reader.Read())
                {
                    var index = new FrameIndex(
                        reader.GetInt32(0),
                        reader.GetInt32(1));
                    var size = reader.GetInt32(2);
                    var bytes = size > 0 ? reader.GetBytes(3, size) : null;
                    _ = FrameCache.TryAdd(Tuple.Create(index, videoFile), bytes);
                    yield return (index, bytes);
                }
            }
        }
    }
}
