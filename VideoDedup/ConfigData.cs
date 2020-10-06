using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoDedup.Properties;

namespace VideoDedup
{
    static class ConfigData
    {

        public static string SourcePath
        {
            get { return Settings.Default.SourcePath; }
            set
            {
                Settings.Default.SourcePath = value;
                Settings.Default.Save();
            }
        }

        public static IList<string> ExcludedDirectories
        {
            get
            {
                if (_ExcludedDirectories == null)
                {
                    _ExcludedDirectories = JsonConvert.DeserializeObject<List<string>>(Settings.Default.ExcludedDirectories);
                }
                return _ExcludedDirectories;
            }
            set
            {
                _ExcludedDirectories = value;
                Settings.Default.ExcludedDirectories = JsonConvert.SerializeObject(_ExcludedDirectories);
                Settings.Default.Save();
            }
        }
        private static IList<string> _ExcludedDirectories = null;

        public static IList<string> FileExtensions
        {
            get
            {
                if (_FileExtensions != null)
                {
                    return _FileExtensions;
                }

                _FileExtensions = JsonConvert.DeserializeObject<List<string>>(Settings.Default.FileExtensions);
                if (_FileExtensions != null || _FileExtensions.Any())
                {
                    return _FileExtensions;
                }

                _FileExtensions = new List<string>
                    {
                        ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mpeg", ".rm", ".mts", ".3gp"
                    };
                return _FileExtensions;
            }
            set
            {
                _FileExtensions = value;
                Settings.Default.FileExtensions = JsonConvert.SerializeObject(_FileExtensions);
                Settings.Default.Save();
            }
        }
        private static IList<string> _FileExtensions = null;

        public static int ThumbnailViewCount
        {
            get
            {
                return Settings.Default.ThumbnailViewCount;
            }
            set
            {
                Settings.Default.ThumbnailViewCount = value;
                Settings.Default.Save();
            }
        }

        public static int MaxThumbnailComparison
        {
            get
            {
                return Settings.Default.MaxThumbnailComparison;
            }
            set
            {
                Settings.Default.MaxThumbnailComparison = value;
                Settings.Default.Save();
            }
        }

        public static int MaxDifferentThumbnails
        {
            get
            {
                return Settings.Default.MaxDifferentThumbnails;
            }
            set
            {
                Settings.Default.MaxDifferentThumbnails = value;
                Settings.Default.Save();
            }
        }

        public static double MaxDifferencePercentage
        {
            get
            {
                return Settings.Default.MaxDifferencePercentage;
            }
            set
            {
                Settings.Default.MaxDifferencePercentage = value;
                Settings.Default.Save();
            }
        }

        public static int MaxDurationDifference
        {
            get
            {
                return Settings.Default.MaxDurationDifferernce;
            }
            set
            {
                Settings.Default.MaxDurationDifferernce = value;
                Settings.Default.Save();
            }
        }

        public static DurationDifferenceType DurationDifferenceType
        {
            get
            {
                if (_DurationDifferenceType == null)
                {
                    if (Enum.TryParse(
                        Settings.Default.DurationDifferenceType,
                        true,
                        out DurationDifferenceType value))
                    {
                        _DurationDifferenceType = value;
                    }
                    else
                    {
                        _DurationDifferenceType = DurationDifferenceType.Seconds;
                    }
                }
                return _DurationDifferenceType.Value;
            }
            set
            {
                _DurationDifferenceType = value;
                Settings.Default.DurationDifferenceType = value.ToString();
                Settings.Default.Save();
            }
        }
        private static DurationDifferenceType? _DurationDifferenceType = null;
    }
}
