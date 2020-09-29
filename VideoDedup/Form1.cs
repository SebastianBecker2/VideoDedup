using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToDoManager;

namespace VideoDedup
{
    public partial class Form1 : Form
    {
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

                foreach (string directory in Directory
                    .GetDirectories(rootDirectory)
                    .Except(excludedDirectories))
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

        private void BtnDedup_Click(object sender, EventArgs e)
        {
            var source_path = TxtSourcePath.Text;

            IEnumerable<string> video_file_endings = new List<string>
            {
                ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mpeg", ".rm", ".mts", ".3gp"
            };

            var video_files =
                GetAllAccessibleFilesIn(source_path, new string[] { Path.Combine(source_path, "$RECYCLE.BIN") })
                .Where(f => video_file_endings.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => new VideoFile(f))
                .OrderBy(f => f.Duration)
                .ToList();

            Debug.Print($"Found {video_files.Count} video files");

            for (int index = 0; index < video_files.Count - 1; index++)
            {
                var file = video_files[index];
                Debug.Print(file.FilePath);

                for (int next_index = index + 1; next_index < video_files.Count; next_index++)
                {
                    var next_video = video_files[next_index];
                    if (!file.IsDurationEqual(next_video))
                    {
                        break;
                    }

                    if (file.AreThumbnailsEqual(next_video))
                    {
                        Debug.Print($" - equal to: {next_video.FilePath}");
                    }
                    else
                    {
                        Debug.Print($" - NOT equal to: {next_video.FilePath}");
                    }
                }

                file.DisposeThumbnails();
            }
        }
    }
}
