namespace VideoDedupClient
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using CustomSelectFileDlg;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Grpc.Net.Client.Configuration;
    using Properties;
    using Dialogs;
    using static VideoDedupGrpc.VideoDedupGrpcService;
    using System.Collections.Concurrent;
    using CustomSelectFileDlg.Exceptions;
    using VideoDedupSharedLib;

    internal static class Program
    {
#if DEBUG
        private static readonly string RootPath = @"H:\";

        internal class EntryElement
        {
            public EntryElement(string path)
            {
                Entry = EntryFromFile(path);

                if (!Directory.Exists(path))
                {
                    return;
                }

                try
                {
                    SubEntries = Directory.GetFileSystemEntries(path).Select(p => new EntryElement(p)).ToList();
                }
                catch (IOException)
                {
                    SubEntries = new List<EntryElement>();
                }
                catch (UnauthorizedAccessException)
                {
                    SubEntries = new List<EntryElement>();
                }
            }

            public Entry Entry { get; set; }
            public List<EntryElement>? SubEntries { get; set; }

            public static Entry EntryFromFile(string entry)
            {
                var attr = File.GetAttributes(entry);
                var info = new FileInfo(entry);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    return new Entry(entry.Length <= 3 ? entry : Path.GetFileName(entry))
                    {
                        Size = null,
                        DateModified = info.LastWriteTimeUtc,
                        MimeType = "File folder",
                        Type = EntryType.Folder,
                    };
                }

                return new Entry(Path.GetFileName(entry))
                {
                    Icon = FileInfoProvider.GetIcon(entry),
                    Size = info.Length,
                    DateModified = info.LastWriteTimeUtc,
                    MimeType = FileInfoProvider.GetMimeType(entry),
                    Type = EntryType.File,
                };
            }

            public EntryElement? FindEntryElement(string path)
            {
                if (!path.StartsWith(
                        Entry.Name,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                path = path[Entry.Name.Length..];
                while (path.StartsWith(Path.DirectorySeparatorChar)
                       || path.StartsWith(Path.AltDirectorySeparatorChar))
                {
                    path = path[1..];
                }

                var sepIndex = path.IndexOfAny(new[]
                {
                    Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar
                });
                if (sepIndex == -1)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        return this;
                    }
                    return SubEntries.FirstOrDefault(e => e.Entry.Name == path);
                }

                return SubEntries
                    .FirstOrDefault(e => e.Entry.Name == path[0..sepIndex])
                    ?.FindEntryElement(path);
            }
        }

#endif

        private static GrpcChannel? grpcChannel;
        private static readonly object GrpcClientLock = new();
        private static readonly MethodConfig GrpcDefaultBackoffConfig =
            new()
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 5,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1.2,
                    RetryableStatusCodes = { StatusCode.Unavailable }
                },
            };
        private static readonly MethodConfig GrpcGetCurrentStatusBackoffConfig =
            new()
            {
                Names =
                {
                    new MethodName
                    {
                        Service = "video_dedup_grpc.VideoDedupGrpcService",
                        Method = "GetCurrentStatus",
                    },
                },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 2,
                    InitialBackoff = TimeSpan.FromMilliseconds(100),
                    MaxBackoff = TimeSpan.FromMilliseconds(100),
                    BackoffMultiplier = 1,
                    RetryableStatusCodes = { StatusCode.Unavailable }
                },
            };
        internal static VideoDedupGrpcServiceClient GrpcClient
        {
            get
            {
                lock (GrpcClientLock)
                {
                    grpcChannel ??= GrpcChannel.ForAddress(
                        $"http://{Configuration.ServerAddress}:41722",
                        new GrpcChannelOptions
                        {
                            MaxReceiveMessageSize = null,
                            ServiceConfig = new ServiceConfig
                            {
                                MethodConfigs =
                                {
                                    GrpcDefaultBackoffConfig,
                                    GrpcGetCurrentStatusBackoffConfig,
                                },
                            }
                        });
                    return new VideoDedupGrpcServiceClient(grpcChannel);
                }
            }
        }

        internal static ConfigData Configuration { get; set; } = LoadConfig();

        private static ConfigData LoadConfig() => new()
        {
            ServerAddress = Settings.Default.ServerAddress,
            StatusRequestInterval = TimeSpan.FromMilliseconds(
                    Settings.Default.StatusRequestInterval),
            ClientSourcePath = Settings.Default.ClientSourcePath,
        };

        internal static void SaveConfig() => SaveConfig(Configuration);

        private static void SaveConfig(ConfigData configuration)
        {
            Settings.Default.ServerAddress = configuration.ServerAddress;
            Settings.Default.StatusRequestInterval =
                (int)configuration.StatusRequestInterval.TotalMilliseconds;
            Settings.Default.ClientSourcePath =
                configuration.ClientSourcePath;
            Settings.Default.Save();
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

#if DEBUG
            foreach (var i in Enumerable.Range(0, 100))
            {
                EntryElement rootElement = new(RootPath);
                if (false)
                {
#pragma warning disable CS0162 // Unreachable code detected
                    var dlg2 = new OpenFileDialog()
                    {
                        Multiselect = true
                    };
#pragma warning restore CS0162 // Unreachable code detected
                    var result2 = dlg2.ShowDialog();
                    if (result2 == DialogResult.OK)
                    {
                        //MessageBox.Show("Filenames:");
                        _ = MessageBox.Show(string.Join("\n", dlg2.FileNames));
                    }
                    continue;
                }
                var dlg = new CustomSelectFileDialog
                {
                    CurrentPath = @"H:\_Test\Test6\Test5\Test4\Test3\Test2\Test1\Test1\Test2\Test3\Test4\Test5\Test6",
                    EntryIconStyle = IconStyle.FallbackToSimpleIcons,
                    EntryType = EntryType.File,
                    ButtonUpEnabled = ButtonUpEnabledWhen.Always,
                };
                dlg.ContentRequested += (s, e) =>
                {
                    Debug.Print("Content Requested");
                    if (string.IsNullOrWhiteSpace(e.Path))
                    {
                        Debug.Print(
                            "Path is null or white space," +
                            $" setting it back to {RootPath}");
                        dlg.CurrentPath = RootPath;
                        return;
                    }

                    var entryElement = rootElement.FindEntryElement(e.Path);

                    if (entryElement == null)
                    {
                        throw new InvalidContentRequestException();
                    }

                    if (e.Path == RootPath + @"_Test\Test3")
                    {
                        var testElement =
                            rootElement.FindEntryElement(RootPath + @"_Test");
                        testElement!.SubEntries.Remove(entryElement);
                    }

                    dlg.SetContent(
                        (entryElement?.SubEntries ?? new List<EntryElement>())
                        .Select(se => se.Entry));
                };

                var result = dlg.ShowDialog();

                if (result == DialogResult.OK)
                {
                    _ = MessageBox.Show(dlg.SelectedPath);
                }
            }
#endif
            Application.Run(new VideoDedupDlg());
        }
    }
}
