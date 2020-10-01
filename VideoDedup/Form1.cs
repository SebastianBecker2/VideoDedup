﻿using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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

        private TimeSpan ElapsedSeconds { get; set; } = new TimeSpan();

        private CancellationTokenSource CancellationTokenSource { get; set; }

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

        public static IEnumerable<string> GetAllAccessibleFilesIn(
            string rootDirectory,
            IEnumerable<string> excludedDirectories = null,
            string searchPattern = "*.*")
        {
            List<string> files = new List<string>();
            if (excludedDirectories == null)
            {
                excludedDirectories = new List<string>();
            }

            try
            {
                files.AddRange(Directory.GetFiles(rootDirectory, searchPattern, SearchOption.TopDirectoryOnly));

                IEnumerable<string> subDirectories = Directory
                    .GetDirectories(rootDirectory);
                subDirectories = subDirectories.Where(d => !excludedDirectories.Contains(d));

                foreach (string directory in subDirectories)
                {
                    files.AddRange(GetAllAccessibleFilesIn(directory, excludedDirectories, searchPattern));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Don't do anything if we cannot access a file.
            }

            return files;
        }

        private void SaveVideoFilesCache(IEnumerable<VideoFile> videoFiles, string cache_path)
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

        private IOrderedEnumerable<VideoFile> LoadVideoFileList(string sourcePath)
        {
            var timer = Stopwatch.StartNew();
            var cached_files = LoadVideoFilesCache(CacheFilePath);
            if (cached_files == null)
            {
                cached_files = new HashSet<VideoFile>();
            }

            // Get all video files in source path.
            var found_files = GetAllAccessibleFilesIn(sourcePath, ExcludedDirectories)
                .Where(f => FileExtensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => new VideoFile(f));

            // Basically overwrite the found files with cached files
            // and make sure we don't take cached files that don't exist
            // anymore.
            cached_files.UnionWith(found_files);
            cached_files.RemoveWhere(f => !found_files.Contains(f));

            // Output damaged files for which we can't read the MediaInfo.
            foreach (var invalid_file in cached_files
                .Where(f => f.Duration == new TimeSpan()))
            {
                Debug.Print($"Invalid file: {invalid_file.FilePath}");
            }

            // Removed files that are damaged and don't have valid MediaInfo
            // and sort the remaining files.
            var video_files = cached_files
                .Where(f => f.Duration != new TimeSpan())
                .OrderBy(f => f.Duration);

            // Save the data we have gathered.
            SaveVideoFilesCache(video_files, CacheFilePath);
            timer.Stop();

            Debug.Print($"Found {video_files.Count()} video files in {timer.ElapsedMilliseconds} ms");
            return video_files;
        }

        private IEnumerable<Tuple<VideoFile, VideoFile>> FindDuplicates(
            IOrderedEnumerable<VideoFile> videoFiles, CancellationToken cancelToken)
        {
            var videoFileList = videoFiles.ToList();

            var duplicates = new List<Tuple<VideoFile, VideoFile>>();

            var timer = Stopwatch.StartNew();
            for (int index = 0; index < videoFileList.Count - 1; index++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }

                var file = videoFileList[index];

                Func<string> createStatusInfo = () => $"Comparing {index + 1}/{videoFileList.Count()}" +
                    $"{Environment.NewLine}Duplicates found: {duplicates.Count()}" +
                    $"{Environment.NewLine}{file.FilePath}" +
                    $"{Environment.NewLine}Duration: {file.Duration}";
                this.Invoke(new Action(() =>
                {
                    LblStatusInfo.Text = createStatusInfo();
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = index + 1;
                }));

                for (int next_index = index + 1; next_index < videoFileList.Count; next_index++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var next_video = videoFileList[next_index];

                    this.Invoke(new Action(() =>
                    {
                        LblStatusInfo.Text = createStatusInfo() + $"{Environment.NewLine}{next_video.FilePath}";
                    }));

                    if (!file.IsDurationEqual(next_video))
                    {
                        break;
                    }

                    if (file.AreThumbnailsEqual(next_video))
                    {
                        duplicates.Add(Tuple.Create(file, next_video));
                        Debug.Print(file.FilePath);
                        Debug.Print($" - equal to: {next_video.FilePath}");
                    }
                }

                file.DisposeThumbnails();
            }
            timer.Stop();
            Debug.Print($"Found {duplicates.Count()} in {timer.ElapsedMilliseconds} ms");

            return duplicates;
        }

        private void ResolveDuplicates(IEnumerable<Tuple<VideoFile, VideoFile>> duplicates)
        {
            Debug.Print($"Comparing {duplicates.Count()} duplicates.");
            foreach ((var left, var right) in duplicates)
            {
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
                    dlg.ShowDialog();
                    left.DisposeThumbnails();
                    right.DisposeThumbnails();
                }
            }
        }

        private void BtnDedup_Click(object sender, EventArgs e)
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Dispose();
            }
            CancellationTokenSource = new CancellationTokenSource();

            var token = CancellationTokenSource.Token;
            IEnumerable<Tuple<VideoFile, VideoFile>> duplicates = null;
            BtnDedup.Enabled = false;
            LblStatusInfo.Text = "Searching for files and loading media information...";
            progressBar1.Style = ProgressBarStyle.Marquee;
            ElapsedSeconds = new TimeSpan();
            timer1.Start();

            Task.Run(() =>
            {
                var video_files = LoadVideoFileList(SourcePath);

                this.Invoke(new Action(() =>
                {
                    LblStatusInfo.Text = $"Comparing 0/{video_files.Count()}";
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Maximum = video_files.Count();
                    progressBar1.Value = 0;
                    BtnCancel.Enabled = true;
                }));

                duplicates = FindDuplicates(video_files, token);

                this.Invoke(new Action(() =>
                {
                    LblStatusInfo.Text = $"Found {duplicates.Count()} duplicates";
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 0;
                    BtnDedup.Enabled = true;
                    BtnCancel.Enabled = false;
                    timer1.Stop();
                }));
                this.Invoke(new Action(() =>
                {
                    if (duplicates != null)
                    {
                        ResolveDuplicates(duplicates);
                    }
                }));
            }, token);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            ElapsedSeconds = ElapsedSeconds.Add(TimeSpan.FromSeconds(1));
            LblTimer.Text = ElapsedSeconds.ToString();
        }
    }
}
