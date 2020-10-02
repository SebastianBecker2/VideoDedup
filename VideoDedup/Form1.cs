using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToDoManager;
using VideoDedup.Properties;

namespace VideoDedup
{
    public partial class Form1 : Form
    {
        private readonly static string CacheFolderName = "VideoDedupCache";
        private readonly static string CacheFileName = "video_files.cache";

        private readonly static string StatusInfoComparing = "Comparing: {0}/{1}";
        private readonly static string StatusInfoLoading = "Loading media info: {0}/{1}";
        private readonly static string StatusInfoSearching = "Searching for files...";

        private readonly static string StatusInfoDuplicateCount = "Duplicates found {0}";
        private readonly static string StatusInfoChecking = "Checking: ";

        private string CurrentStatusInfo { get; set; }

        public string CacheFilePath
        {
            get
            {
                var cache_folder = Path.Combine(SourcePath, CacheFolderName);
                Directory.CreateDirectory(cache_folder);
                return Path.Combine(cache_folder, CacheFileName);
            }
        }

        public string SourcePath
        {
            get { return Settings.Default.SourcePath; }
            set
            {
                Settings.Default.SourcePath = value;
                Settings.Default.Save();
            }
        }

        public IList<string> ExcludedDirectories
        {
            get
            {
                return JsonConvert.DeserializeObject<List<string>>(Settings.Default.ExcludedDirectories);
            }
            set
            {
                Settings.Default.ExcludedDirectories = JsonConvert.SerializeObject(value);
                Settings.Default.Save();
            }
        }

        public IList<string> FileExtensions
        {
            get
            {
                var file_extensions = JsonConvert.DeserializeObject<List<string>>(Settings.Default.FileExtensions);
                if (file_extensions == null || !file_extensions.Any())
                {
                    return new List<string>
                        {
                            ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mpeg", ".rm", ".mts", ".3gp"
                        };
                }
                return file_extensions;
            }
            set
            {
                Settings.Default.FileExtensions = JsonConvert.SerializeObject(value);
                Settings.Default.Save();
            }
        }

        private TimeSpan ElapsedTime { get; set; } = new TimeSpan();

        private IList<Tuple<VideoFile, VideoFile>> Duplicates { get; set; }
            = new List<Tuple<VideoFile, VideoFile>>();

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private TimeSpan? SelectedMinimumDuration { get; set; } = null;
        private TimeSpan? SelectedMaximumDuration { get; set; } = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void BtnToDoManager_Click(object sender, EventArgs e)
        {
            using (var dlg = new ToDoManager.ToDoManager())
            {
                dlg.ShowDialog();
            }
        }

        private void UpdateProgress(
            string statusInfo,
            int counter,
            int maxCount)
        {
            this.InvokeIfRequired(() =>
            {
                if (!string.IsNullOrWhiteSpace(statusInfo))
                {
                    CurrentStatusInfo = statusInfo;
                }
                LblStatusInfo.Text = string.Format(
                    CurrentStatusInfo, 
                    counter, 
                    maxCount);

                if (maxCount > 0)
                {
                    TaskbarManager.Instance.SetProgressState(
                        TaskbarProgressBarState.Normal, 
                        Handle);
                    ProgressBar.Style = ProgressBarStyle.Continuous;
                }
                else
                {
                    TaskbarManager.Instance.SetProgressState(
                        TaskbarProgressBarState.Indeterminate, 
                        Handle);
                    ProgressBar.Style = ProgressBarStyle.Marquee;
                }

                ProgressBar.Value = counter;
                ProgressBar.Maximum = maxCount == 0 ? 1 : maxCount;
                TaskbarManager.Instance.SetProgressValue(
                    counter,
                    maxCount,
                    Handle);
            });
        }

        private void RemoveDuplicate(int index)
        {
            this.InvokeIfRequired(() =>
            {
                Duplicates.RemoveAt(index);
                LblDuplicateCount.Text = string.Format(
                    StatusInfoDuplicateCount, Duplicates.Count());
                if (!Duplicates.Any())
                {
                    BtnResolveConflicts.Enabled = false;
                }
            });
        }

        private void AddDuplicate(VideoFile left, VideoFile right)
        {
            this.InvokeIfRequired(() =>
            {
                Duplicates.Add(Tuple.Create(left, right));
                LblDuplicateCount.Text = string.Format(
                    StatusInfoDuplicateCount, Duplicates.Count());
                BtnResolveConflicts.Enabled = true;
            });
        }

        public static IEnumerable<string> GetAllAccessibleFilesIn(
            string rootDirectory,
            IEnumerable<string> excludedDirectories = null,
            string searchPattern = "*.*")
        {
            IEnumerable<string> files = new List<string>();
            if (excludedDirectories == null)
            {
                excludedDirectories = new List<string>();
            }

            try
            {
                files = files.Concat(Directory.EnumerateFiles(rootDirectory, searchPattern, SearchOption.TopDirectoryOnly));

                foreach (string directory in Directory
                    .GetDirectories(rootDirectory)
                    .Where(d => !excludedDirectories.Contains(d)))
                {
                    files = files.Concat(GetAllAccessibleFilesIn(directory, excludedDirectories, searchPattern));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Don't do anything if we cannot access a file.
            }

            return files;
        }

        private void SaveVideoFilesCache(
            IEnumerable<VideoFile> videoFiles,
            string cache_path)
        {
            var timer = Stopwatch.StartNew();
            File.WriteAllText(cache_path, JsonConvert.SerializeObject(videoFiles, Formatting.Indented));
            timer.Stop();
            Debug.Print($"Writing cache file took {timer.ElapsedMilliseconds} ms");
        }

        private HashSet<VideoFile> LoadVideoFilesCache(string cache_path)
        {
            try
            {
                return JsonConvert.DeserializeObject<HashSet<VideoFile>>(File.ReadAllText(cache_path));
            }
            catch
            {
                return null;
            }
        }

        private IEnumerable<VideoFile> GetVideoFileList(string sourcePath)
        {
            var timer = Stopwatch.StartNew();

            // Get all video files in source path.
            var fileExtensions = FileExtensions.ToList();
            var found_files = GetAllAccessibleFilesIn(sourcePath, ExcludedDirectories)
                .Where(f => fileExtensions.Contains(Path.GetExtension(f), StringComparer.CurrentCultureIgnoreCase))
                .Select(f => new VideoFile(f));

            var cached_files = LoadVideoFilesCache(CacheFilePath);
            if (cached_files == null || !cached_files.Any())
            {
                cached_files = new HashSet<VideoFile>(found_files);
            }
            else
            {
                // Basically overwrite the found files with cached files
                // and make sure we don't take cached files that don't exist
                // anymore.
                cached_files.RemoveWhere(f => !File.Exists(f.FilePath));
                cached_files.UnionWith(found_files);
            }
            timer.Stop();

            Debug.Print($"Found {cached_files.Count()} video files in {timer.ElapsedMilliseconds} ms");
            return cached_files;
        }

        private void FindDuplicates(
            IOrderedEnumerable<VideoFile> videoFiles,
            CancellationToken cancelToken)
        {
            var videoFileList = videoFiles.ToList();

            for (int index = 0; index < videoFileList.Count() - 1; index++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }

                var file = videoFileList[index];

                this.InvokeIfRequired(() =>
                {
                    LblCurrentFile.Text = StatusInfoChecking + $"{file.FilePath}" +
                        $"{Environment.NewLine}Duration: {file.Duration}";
                    UpdateProgress(StatusInfoComparing, index, videoFileList.Count());
                });

                for (int nextIndex = index + 1; nextIndex < videoFileList.Count; nextIndex++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var nextFile = videoFileList[nextIndex];

                    if (!file.IsDurationEqual(nextFile))
                    {
                        break;
                    }

                    if (file.AreThumbnailsEqual(nextFile))
                    {
                        AddDuplicate(file, nextFile);
                    }
                }

                SelectedMinimumDuration = file.Duration;
                file.DisposeThumbnails();
            }
        }

        private void ResolveDuplicates()
        {
            for (var index = 0; index < Duplicates.Count();)
            {
                (var left, var right) = Duplicates[index];

                // Mostely because we might have deleted
                // this file during the previous compare
                if (!File.Exists(left.FilePath))
                {
                    Debug.Print($"{left.FilePath} doesn't exist anymore. Can't compare.");
                    continue;
                }
                if (!File.Exists(right.FilePath))
                {
                    Debug.Print($"{right.FilePath} doesn't exist anymore. Can't compare.");
                    continue;
                }

                using (var dlg = new FileComparison())
                {
                    dlg.LeftFile = left;
                    dlg.RightFile = right;
                    var result = dlg.ShowDialog();
                    left.DisposeThumbnails();
                    right.DisposeThumbnails();

                    if (result == DialogResult.Yes)
                    {
                        RemoveDuplicate(index);
                    }
                    if (result == DialogResult.Abort)
                    {
                        // Keep in list
                        return;
                    }
                    if (result == DialogResult.No)
                    {
                        index++;
                    }
                }
            }
        }

        private void PreloadFiles(
            IEnumerable<VideoFile> videoFiles,
            CancellationToken cancelToken)
        {
            int counter = 0;
            DateTime? lastUpdate = null;
            foreach (var f in videoFiles)
            {
                counter++;

                if (lastUpdate == null
                    || (lastUpdate.Value - DateTime.Now).TotalMilliseconds > 100)
                {
                    UpdateProgress(StatusInfoLoading, counter, videoFiles.Count());
                }

                // For now we only preload the duration
                // since the size is only rarely used
                // in the comparison dialog. No need
                // to preload it.
                var duration = f.Duration;
                //var size = f.FileSize;
                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private void SelectDuration(TimeSpan min, TimeSpan max)
        {
            using (var dlg = new DurationSelection())
            {
                dlg.AbsolutMinimumDuration = min;
                dlg.AbsolutMaximumDuration = max;
                dlg.SelectedMinimumDuration = SelectedMinimumDuration;
                dlg.SelectedMaximumDuration = SelectedMaximumDuration;
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    CancellationTokenSource.Cancel();
                    return;
                }
                SelectedMinimumDuration = dlg.SelectedMinimumDuration;
                SelectedMaximumDuration = dlg.SelectedMaximumDuration;
            }
        }

        private void BtnDedup_Click(object sender, EventArgs e)
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Dispose();
            }
            CancellationTokenSource = new CancellationTokenSource();

            var cancelToken = CancellationTokenSource.Token;
            BtnDedup.Enabled = false;
            BtnConfig.Enabled = false;
            BtnCancel.Enabled = false;
            LblCurrentFile.Text = StatusInfoChecking;
            UpdateProgress(StatusInfoSearching, 0, 0);
            ElapsedTime = new TimeSpan();
            ElapsedTimer.Start();

            Task.Run(() =>
            {
                var videoFiles = GetVideoFileList(SourcePath);

                this.InvokeIfRequired(() =>
                {
                    UpdateProgress(StatusInfoLoading, 0, videoFiles.Count());
                    BtnCancel.Enabled = true;
                });

                PreloadFiles(videoFiles, cancelToken);

                // Save the data we have gathered.
                SaveVideoFilesCache(videoFiles, CacheFilePath);

                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                this.Invoke(new Action(() =>
                {
                    ElapsedTimer.Stop();
                    SelectDuration(
                        videoFiles.Min(f => f.Duration),
                        videoFiles.Max(f => f.Duration));
                    ElapsedTimer.Start();
                }));

                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                // Removed files that are damaged and don't have valid MediaInfo
                // and sort the remaining files.
                var orderedVideoFiles = videoFiles
                    .Where(f => f.Duration != new TimeSpan())
                    .Where(f => f.Duration >= SelectedMinimumDuration.Value)
                    .Where(f => f.Duration <= SelectedMaximumDuration.Value)
                    .OrderBy(f => f.Duration);

                UpdateProgress(StatusInfoComparing, 0, videoFiles.Count());

                FindDuplicates(orderedVideoFiles, cancelToken);
            }, cancelToken).ContinueWith(t =>
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                ProgressBar.Stop();
                BtnDedup.Enabled = true;
                BtnConfig.Enabled = true;
                BtnCancel.Enabled = false;
                ElapsedTimer.Stop();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CancellationTokenSource.Cancel();
        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            using (var dlg = new Config())
            {
                dlg.SourcePath = SourcePath;
                dlg.FileExtensions = FileExtensions;
                dlg.ExcludedDirectories = ExcludedDirectories;
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                SourcePath = dlg.SourcePath;
                FileExtensions = dlg.FileExtensions;
                ExcludedDirectories = dlg.ExcludedDirectories;
            }
        }

        private void ElapsedTimer_Tick(object sender, EventArgs e)
        {
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
            LblTimer.Text = ElapsedTime.ToString();
        }

        private void BtnResolveConflicts_Click(object sender, EventArgs e)
        {
            ResolveDuplicates();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new About())
            {
                dlg.ShowDialog();
            }
        }
    }
}
