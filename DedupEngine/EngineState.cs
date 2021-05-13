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
        private static readonly string StatePostfix = ".state";

        private string IndexFilePath =>
            Path.Combine(StateFolderPath, IndexFilename);
        private string GetStateFilePath(string stateId) =>
            Path.Combine(StateFolderPath, stateId + StatePostfix);

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

            EngineState savedState;
            try
            {
                savedState = JsonConvert.DeserializeObject<EngineState>(
                    File.ReadAllText(GetStateFilePath(stateId)));
            }
            catch (Exception)
            {
                return;
            }

            VideoFiles = savedState.VideoFiles;
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
                File.WriteAllText(
                    GetStateFilePath(stateId),
                    JsonConvert.SerializeObject(
                        this,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                        }));
            }
            catch (Exception) { }
        }
    }
}
