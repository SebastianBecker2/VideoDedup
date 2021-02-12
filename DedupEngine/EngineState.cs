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
        [JsonProperty]
        public IList<VideoFile> VideoFiles { get; set; }
        [JsonProperty]
        public int CurrentIndex { get; set; } = 0;
        [JsonProperty]
        public EngineSettings Settings { get; set; }
        [JsonIgnore]
        private string FilePath { get; set; }

        [JsonConstructor]
        private EngineState()
        {
        }

        public EngineState(IDedupEngineSettings settings, string filePath)
        {
            Settings = new EngineSettings(settings);
            FilePath = filePath;
            LoadState();
        }

        private void LoadState()
        {
            EngineState savedState;
            try
            {
                savedState = JsonConvert.DeserializeObject<EngineState>(
                    File.ReadAllText(FilePath));
            }
            catch (Exception)
            {
                return;
            }

            if (!AreSettingsCompatible(Settings, savedState.Settings))
            {
                return;
            }

            VideoFiles = savedState.VideoFiles;
            CurrentIndex = savedState.CurrentIndex;
        }

        public void SaveState() =>
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                }));

        private static bool AreSettingsCompatible(
            IDedupEngineSettings lhs,
            IDedupEngineSettings rhs)
        {
            // Folder Settings
            if (lhs.BasePath != rhs.BasePath)
            {
                return false;
            }
            if (!lhs.ExcludedDirectories.SequenceEqual(rhs.ExcludedDirectories))
            {
                return false;
            }
            if (!lhs.FileExtensions.SequenceEqual(rhs.FileExtensions))
            {
                return false;
            }
            if (lhs.Recursive != rhs.Recursive)
            {
                return false;
            }

            // Image Comparison Settings
            if (lhs.MaxCompares != rhs.MaxCompares)
            {
                return false;
            }
            if (lhs.MaxDifferentImages != rhs.MaxDifferentImages)
            {
                return false;
            }
            var lhsImageComparison = lhs as IImageComparisonSettings;
            var rhsImageComparison = rhs as IImageComparisonSettings;
            if (lhsImageComparison.MaxDifferencePercent
                != rhsImageComparison.MaxDifferencePercent)
            {
                return false;
            }

            // Duration Comparison Settings
            if (lhs.DifferenceType != rhs.DifferenceType)
            {
                return false;
            }
            if (lhs.DifferenceType == DurationDifferenceType.Percent)
            {
                var lhsDurationComparison = lhs as IDurationComparisonSettings;
                var rhsDurationComparison = rhs as IDurationComparisonSettings;
                if (lhsDurationComparison.MaxDifferencePercent
                    != rhsDurationComparison.MaxDifferencePercent)
                {
                    return false;
                }
            }
            else
            {
                if (lhs.MaxDifferenceSeconds != rhs.MaxDifferenceSeconds)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
