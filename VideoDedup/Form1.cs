using Microsoft.WindowsAPICodePack.Shell;
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
                var cache_folder = Path.Combine(TxtSourcePath.Text, CacheFolderName);
                Directory.CreateDirectory(cache_folder);
                return Path.Combine(cache_folder, CacheFileName);
            }
        }

        public IEnumerable<string> ExcludedDirectories
            => new List<string> {
                Path.Combine(TxtSourcePath.Text, "$RECYCLE.BIN"),
                TxtExcludePaths.Text
            }.Select(d => d.ToLower());

        public IEnumerable<string> VideoFileEndings
        {
            get => new List<string>
            {
                ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mpeg", ".rm", ".mts", ".3gp"
            };
        }

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
                subDirectories = subDirectories.Where(d => !excludedDirectories.Contains(d.ToLower()));

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

        private void CacheVideoFiles(IEnumerable<VideoFile> videoFiles, string cache_path)
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

            var found_files = GetAllAccessibleFilesIn(sourcePath, ExcludedDirectories)
                .Where(f => VideoFileEndings.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => new VideoFile(f));

            cached_files.UnionWith(found_files);

            foreach (var invalid_file in cached_files
                .Where(f => f.Duration == new TimeSpan()))
            {
                Debug.Print($"Invalid file: {invalid_file.FilePath}");
            }

            var video_files = cached_files
                .Where(f => f.Duration != new TimeSpan())
                .OrderBy(f => f.Duration);

            CacheVideoFiles(video_files, CacheFilePath);
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
                cancelToken.ThrowIfCancellationRequested();
                var file = videoFileList[index];

                var statusInfo = $"Comparing {index + 1}/{videoFileList.Count()}" +
                    $"{Environment.NewLine}Duplicates found: {duplicates.Count()}" +
                    $"{Environment.NewLine}{file.FilePath}" +
                    $"{Environment.NewLine}Duration: {file.Duration}";
                this.Invoke(new Action(() =>
                {
                    LblStatusInfo.Text = statusInfo;
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = index + 1;
                }));

                for (int next_index = index + 1; next_index < videoFileList.Count; next_index++)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    var next_video = videoFileList[next_index];

                    this.Invoke(new Action(() =>
                    {
                        LblStatusInfo.Text = statusInfo + $"{Environment.NewLine}{next_video.FilePath}";
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
            foreach ((var left, var right) in duplicates)
            {
                // Mostely because we might have deleted
                // this file during the previous compare
                if (!File.Exists(left.FilePath))
                {
                    return;
                }
                if (!File.Exists(right.FilePath))
                {
                    return;
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

            BtnDedup.Enabled = false;

            var token = CancellationTokenSource.Token;

            Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    LblStatusInfo.Text = "Searching for files and loading media information...";
                    progressBar1.Style = ProgressBarStyle.Marquee;
                }));

                var video_files = LoadVideoFileList(TxtSourcePath.Text);

                this.Invoke(new Action(() =>
                {
                    LblStatusInfo.Text = $"Comparing 0/{video_files.Count()}";
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Maximum = video_files.Count();
                    progressBar1.Value = 0;
                    BtnCancel.Enabled = true;
                }));

                token.ThrowIfCancellationRequested();

                var duplicates = FindDuplicates(video_files, token);

                this.Invoke(new Action(() =>
                {
                    LblStatusInfo.Text = $"Found {duplicates.Count()} duplicates";
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 0;
                    ResolveDuplicates(duplicates);
                    BtnDedup.Enabled = true;
                    BtnCancel.Enabled = false;
                }));
            }, token)
                .ContinueWith(t =>
            {
                this.Invoke(new Action(() =>
                {
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 0;
                    BtnDedup.Enabled = true;
                    BtnCancel.Enabled = false;
                }));
            }, TaskContinuationOptions.OnlyOnCanceled);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CancellationTokenSource.Cancel();
        }
    }
}
