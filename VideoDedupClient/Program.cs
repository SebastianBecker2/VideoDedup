namespace VideoDedupClient
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using CustomSelectFileDialog;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Grpc.Net.Client.Configuration;
    using Properties;
    using Dialogs;
    using static VideoDedupGrpc.VideoDedupGrpcService;
    using System.Collections.Concurrent;

    internal static class Program
    {
#if DEBUG
        // https://stackoverflow.com/a/1437451/2347040
#pragma warning disable IDE0079 // Remove unnecessary suppression
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        public static class FileInfoProvider
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct SHFILEINFO
            {
                private readonly IntPtr hIcon;
                private readonly int iIcon;
                private readonly uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                private readonly string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
#pragma warning disable IDE1006 // Naming Styles
                public readonly string szTypeName;
#pragma warning restore IDE1006 // Naming Styles
            };

            private static class FILE_ATTRIBUTE
            {
                public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
            }

            private static class SHGFI
            {
                public const uint SHGFI_TYPENAME = 0x000000400;
                public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            }

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
            [DllImport(
                "shell32.dll",
                CharSet = CharSet.Ansi,
                ExactSpelling = true,
                BestFitMapping = false,
                ThrowOnUnmappableChar = true,
                CallingConvention = CallingConvention.Cdecl)]
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
            private static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                out SHFILEINFO psfi,
                uint cbSizeFileInfo,
                uint uFlags);

            public static string? GetMimeType(string file)
            {
                if (SHGetFileInfo(
                        file,
                        FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL,
                        out var info,
                        (uint)Marshal.SizeOf<SHFILEINFO>(),
                        SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_USEFILEATTRIBUTES) !=
                    IntPtr.Zero)
                {
                    return info.szTypeName;
                }
                return null;
            }

            private static readonly ConcurrentDictionary<string, Bitmap?>
                IconCache = new();

            public static Bitmap? GetIcon(string file)
            {
                var extension = Path.GetExtension(file);
                if (IconCache.TryGetValue(extension, out var icon))
                {
                    return icon;
                }
                icon = Icon.ExtractAssociatedIcon(file)?.ToBitmap();
                _ = IconCache.TryAdd(extension, icon);
                return icon;
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
            static Entry EntryFromFile(string entry)
            {
                var attr = File.GetAttributes(entry);
                var info = new FileInfo(entry);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    return new Entry(Path.GetFileName(entry))
                    {
                        //Icon = Icon.ExtractAssociatedIcon(entry)?.ToBitmap(),
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

            foreach (var i in Enumerable.Range(0, 100))
            {
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
                    CurrentPath = "C:\\",
                    EntryIconStyle = IconStyle.NoFallbackOnNull
                };
                dlg.ContentRequested += (s, e) =>
                {
                    Debug.Print("Content Requested");
                    if (string.IsNullOrWhiteSpace(e.Path))
                    {
                        Debug.Print(
                            "Path is null or white space," +
                            " setting it back to C:\\");
                        dlg.CurrentPath = "C:\\";
                        return;
                    }

                    dlg.SetContent(Directory
                        .GetFileSystemEntries(e.Path)
                        .Select(EntryFromFile));
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
