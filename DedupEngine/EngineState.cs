namespace DedupEngine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using VideoDedupShared;

    internal class EngineState
    {
        private static readonly string IndexFilename = "DedupEngine.index";
        private static readonly string PartPostfix = ".part";
        private static readonly int BatchSize = 1000;

        private string IndexFilePath =>
            Path.Combine(StateFolderPath, IndexFilename);
        private string GetPartFilePath(string stateId, int partIndex) =>
            Path.Combine(StateFolderPath, $"{stateId}{PartPostfix}{partIndex:D5}");

        private string StateFolderPath { get; }
        private DateTime LastSave { get; set; } = DateTime.MinValue;

        [JsonProperty]
        public IList<VideoFile> VideoFiles { get; set; }
        [JsonIgnore]
        public EngineSettings Settings { get; set; }

        [JsonConstructor]
        private EngineState()
        {
        }

        public EngineState(IDedupEngineSettings settings, string stateFolderPath)
        {
            Settings = new EngineSettings(settings);
            StateFolderPath = stateFolderPath;
            LoadState();
        }

        private void LoadState()
        {
            Dictionary<EngineSettings, string> engineStateIndex;
            try
            {
                engineStateIndex = JsonConvert.DeserializeObject
                    <List<KeyValuePair<EngineSettings, string>>>
                    (File.ReadAllText(IndexFilePath))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch (Exception)
            {
                return;
            }

            if (!engineStateIndex.TryGetValue(Settings, out var stateId))
            {
                return;
            }

            var partIndex = 0;
            var videoFiles = Enumerable.Empty<VideoFile>();
            while (true)
            {
                var filePath = GetPartFilePath(stateId, partIndex++);
                if (!File.Exists(filePath))
                {
                    break;
                }

                var part = JsonConvert.DeserializeObject<List<VideoFile>>(
                    File.ReadAllText(filePath));
                videoFiles = (videoFiles ?? Enumerable.Empty<VideoFile>())
                    .Concat(part);
            }
            VideoFiles = videoFiles.ToList();
        }

        public void SaveState(bool force = true)
        {
            if (!force
                && DateTime.Now - LastSave < Settings.SaveStateInterval)
            {
                return;
            }
            LastSave = DateTime.Now;

            Dictionary<EngineSettings, string> engineStateIndex;
            try
            {
                engineStateIndex = JsonConvert.DeserializeObject
                    <List<KeyValuePair<EngineSettings, string>>>
                    (File.ReadAllText(IndexFilePath))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch (Exception)
            {
                engineStateIndex = new Dictionary<EngineSettings, string>();
            }

            if (!engineStateIndex.TryGetValue(Settings, out var stateId))
            {
                stateId = Guid.NewGuid().ToString();
                engineStateIndex.Add(Settings, stateId);

                try
                {
                    File.WriteAllText(
                        IndexFilePath,
                        JsonConvert.SerializeObject(
                            engineStateIndex.ToList(),
                            Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.All,
                            }));
                }
                catch (Exception) { }
            }

            try
            {
                var batchCount = (int)Math.Ceiling(
                    VideoFiles.Count() / (double)BatchSize);
                foreach (var batchIndex in Enumerable.Range(0, batchCount))
                {
                    var part = VideoFiles.Skip(batchIndex * BatchSize).Take(BatchSize);

                    File.WriteAllText(
                        GetPartFilePath(stateId, batchIndex),
                        JsonConvert.SerializeObject(
                            part,
                            Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.All,
                            }));
                }
            }
            catch (Exception) { }
        }
    }
}
