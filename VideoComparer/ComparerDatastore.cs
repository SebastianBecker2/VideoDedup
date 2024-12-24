namespace VideoComparer
{
    using System;
    using Microsoft.Data.Sqlite;
    using FfmpegLib;
    using VideoDedupSharedLib;
    using VideoDedupSharedLib.ExtensionMethods.DateTimeExtensions;
    using VideoDedupSharedLib.ExtensionMethods.SqliteDataReaderExtensions;
    using VideoDedupSharedLib.ExtensionMethods.SqliteParameterCollectionExtensions;
    using VideoDedupSharedLib.Interfaces;

    internal sealed class ComparerDatastore(string filePath)
        : Datastore(filePath)
    {
        private static readonly Dictionary<Tuple<ImageIndex, IVideoFile>, byte[]?>
            ImageCache = [];
        private static readonly object ImageCacheMutex = new();

        protected override void CreateTables()
        {
            using var connection = OpenConnection();
            CreateImagesTable(connection);
            CreateImagesIndexes(connection);
        }

        private static void CreateImagesTable(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
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

        private static void CreateImagesIndexes(SqliteConnection connection)
        {
            {
                using var command = connection.CreateCommand();
                command.CommandText = "CREATE INDEX IF NOT EXISTS " +
                    "idx_Images_VideoFileId " +
                    "ON Images (VideoFileId);";
                _ = command.ExecuteNonQuery();
            }

            {
                using var command = connection.CreateCommand();
                command.CommandText = "CREATE INDEX IF NOT EXISTS " +
                    "idx_Images_Numerator_Denominator_ImageSize_Data " +
                    "ON Images (Numerator, Denominator, ImageSize, Data);";
                _ = command.ExecuteNonQuery();
            }
        }

        public void InsertImage(
            ImageIndex imageId,
            byte[]? bytes,
            IVideoFile videoFile)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
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

            lock (ImageCacheMutex)
            {
                _ = ImageCache.TryAdd(Tuple.Create(imageId, videoFile), bytes);
            }
        }

        public IEnumerable<(ImageIndex index, byte[]? bytes)> GetImages(
            IEnumerable<ImageIndex> imageIndices,
            IVideoFile videoFile)
        {
            if (!imageIndices.Any())
            {
                yield break;
            }

            List<ImageIndex> uncachedImageIndices = [];
            lock (ImageCacheMutex)
            {
                foreach (var imageIndex in imageIndices)
                {
                    if (ImageCache.TryGetValue(
                        Tuple.Create(imageIndex, videoFile),
                        out var data))
                    {
                        yield return (imageIndex, data);
                    }
                    else
                    {
                        uncachedImageIndices.Add(imageIndex);
                    }
                }
            }

            if (uncachedImageIndices.Count == 0)
            {
                yield break;
            }

            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
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
                    uncachedImageIndices.Select((_, i) =>
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
            foreach (var index in uncachedImageIndices)
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
            lock (ImageCacheMutex)
            {
                while (reader.Read())
                {
                    var index = new ImageIndex(
                        reader.GetInt32(0),
                        reader.GetInt32(1));
                    var size = reader.GetInt32(2);
                    var bytes = size > 0 ? reader.GetBytes(3, size) : null;
                    _ = ImageCache.TryAdd(Tuple.Create(index, videoFile), bytes);
                    yield return (index, bytes);
                }
            }
        }
    }
}
