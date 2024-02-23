namespace CustomSelectFileDlg
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using CustomSelectFileDlg.EventArgs;
    using Vanara.PInvoke;

    internal partial class RefreshableTreeView : UserControl
    {
        private struct Node
        {
            public string Text { get; set; }
            public IEnumerable<Node> Nodes { get; set; }
        }

        private struct ImageComparer : IEqualityComparer<byte[]>
        {
            [DllImport("msvcrt.dll", CallingConvention =
                CallingConvention.Cdecl)]
            private static extern int memcmp(byte[] x, byte[] y, long count);

            public readonly bool Equals(byte[]? x, byte[]? y)
            {
                if (x is null || y is null || x.Length != y.Length)
                {
                    return false;
                }

                return memcmp(x, y, x.Length) == 0;
            }

            public readonly int GetHashCode(byte[] obj)
            {
                unchecked
                {
                    var result = 0;
                    foreach (var b in obj)
                    {
                        result = (result * 31) ^ b;
                    }
                    return result;
                }
            }
        }

        private static readonly Size TreeViewIconSize = new(20, 20);

        public IEnumerable<Entry>? RootFolders { get; set; }
        public IconStyle EntryIconStyle { get; set; }

        // Cache for images working in conjunction with Trv.ImageList.
        // The byte[] allows for quick comparison of image content.
        // The GetImageIndex(Image) function gets the index of the image in
        // Trv.ImageList if it already exists and makes sure the ImageCache is
        // aware of it. If it doesn't exists, it adds the image to the
        // Trv.ImageList and the ImageCache with the respective index.
        private Dictionary<byte[], int> ImageCache { get; set; } =
            new(new ImageComparer());

        /// <summary>
        /// Event raised when path has been changed.<br/>When the user confirms
        /// the path that is displayed in the TextBox using Enter or Return key.
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler<CurrentPathChangedEventArgs>?
            CurrentPathChanged;

        protected virtual void OnCurrentPathChanged(string? path) =>
            CurrentPathChanged?.Invoke(
                this,
                new CurrentPathChangedEventArgs(path));

        /// <summary>
        /// Event raised when sub folders is requested.
        /// </summary>
        public event EventHandler<ContentRequestedEventArgs>?
            SubFoldersRequested;

        protected virtual IEnumerable<Entry>? OnSubFolderRequested(string? path)
        {
            var args = new ContentRequestedEventArgs(
                path,
                RequestedEntryType.Folders,
                null);
            SubFoldersRequested?.Invoke(this, args);
            if (args.Entries is not null && !args.Entries.Any())
            {
                return null;
            }
            return args.Entries;
        }

        public RefreshableTreeView()
        {
            InitializeComponent();

            Trv.ImageList = new()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = TreeViewIconSize,
            };

            Trv.BeforeExpand += HandleTrv_BeforeExpand;
            Trv.AfterSelect += HandleTrv_AfterSelect;

        }

        protected override void OnLoad(System.EventArgs e)
        {
            // Make image at index 0 an empty image.
            // So we can is it to not show any image.
            Bitmap emptyBmp = new(
                TreeViewIconSize.Width,
                TreeViewIconSize.Height);
            var emptyBytes = GetImageBytes(emptyBmp);
            Trv.ImageList.Images.Add(emptyBmp);
            ImageCache.Add(emptyBytes, 0);

            UpdateContent();

            base.OnLoad(e);
        }

        private void UpdateTreeNodes(
            TreeNodeCollection oldNodes,
            IEnumerable<Entry> newEntries)
        {
            var nodes = oldNodes
                .Cast<TreeNode>()
                .OrderBy(n => n.Text)
                .ToList();
            var entries = newEntries
                .OrderBy(n => n.Name)
                .ToList();

            var nodesIndex = 0;
            var entriesIndex = 0;
            while (true)
            {
                if (nodes.Count <= nodesIndex
                    && entries.Count <= entriesIndex)
                {
                    return;
                }

                if (nodes.Count <= nodesIndex)
                {
                    oldNodes.Add(EntryToTreeNode(entries[entriesIndex++]));
                    continue;
                }

                if (entries.Count <= entriesIndex)
                {
                    oldNodes.Remove(nodes[nodesIndex++]);
                    continue;
                }

                var node = nodes[nodesIndex];
                var entry = entries[entriesIndex];

                var diff = string.Compare(
                    node.Text,
                    entry.Name,
                    StringComparison.OrdinalIgnoreCase);

                if (diff > 0)
                {
                    oldNodes.Add(EntryToTreeNode(entry));
                    entriesIndex++;
                    continue;
                }

                if (diff < 0)
                {
                    oldNodes.Remove(node);
                    nodesIndex++;
                    continue;
                }

                if (node.IsExpanded)
                {
                    var subFolders = OnSubFolderRequested(
                        node.FullPath);
                    if (subFolders is not null)
                    {
                        UpdateTreeNodes(node.Nodes, subFolders);
                    }
                }

                nodesIndex++;
                entriesIndex++;
            }
        }

        public void UpdateContent()
        {
            var scrollPos = GetScrollPos(Trv.Handle);

            if (RootFolders is null)
            {
                Trv.Nodes.Clear();
                return;
            }
            UpdateTreeNodes(Trv.Nodes, RootFolders);

            SetScrollPos(Trv.Handle, scrollPos);
        }

        private int GetImageIndex(Image? image)
        {
            if (image is null)
            {
                // The first cached image has to be an empty bitmap.
                return 0;
            }

            var bytes = GetImageBytes(image);
            if (!ImageCache.TryGetValue(bytes, out var index))
            {
                Trv.ImageList.Images.Add(image);
                index = Trv.ImageList.Images.Count - 1;
                ImageCache.Add(bytes, index);
            }
            return index;
        }

        private TreeNode EntryToTreeNode(Entry entry)
        {
            var node = new TreeNode
            {
                Text = entry.Name,
            };
            var imageIndex = GetImageIndex(entry.GetIcon(EntryIconStyle));
            node.ImageIndex = imageIndex;
            node.SelectedImageIndex = imageIndex;
            node.Nodes.Add(new TreeNode("..."));
            return node;
        }

        private void HandleTrv_BeforeExpand(object? _, TreeViewCancelEventArgs e)
        {
            if (e.Node is null || e.Node.Tag is not null)
            {
                return;
            }
            e.Node.Tag = true;

            e.Node.Nodes.Clear();

            var entries = OnSubFolderRequested(e.Node.FullPath);
            if (entries is null)
            {
                return;
            }

            e.Node.Nodes.AddRange(
                entries.Select(EntryToTreeNode).ToArray());
        }

        private void HandleTrv_AfterSelect(object? _, TreeViewEventArgs e)
        {
            if (e.Node is null)
            {
                return;
            }

            OnCurrentPathChanged(e.Node.FullPath);
        }

        private static Point GetScrollPos(HWND handle) =>
            new(User32.GetScrollPos(handle, (int)User32.SB.SB_HORZ),
                User32.GetScrollPos(handle, (int)User32.SB.SB_VERT));

        private static void SetScrollPos(HWND handle, Point scrollPos)
        {
            _ = User32.SetScrollPos(
                handle,
                (int)User32.SB.SB_HORZ,
                scrollPos.X,
                false);
            _ = User32.SetScrollPos(
                handle,
                (int)User32.SB.SB_VERT,
                scrollPos.Y,
                true);
        }

        private static byte[] GetImageBytes(Image image)
        {
            unsafe
            {
                var bmp = new Bitmap(image, TreeViewIconSize);

                var bitmapData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly,
                    bmp.PixelFormat);
                var numBytes = bitmapData.Stride * bmp.Height;
                var bytes = new byte[numBytes];
                Marshal.Copy(bitmapData.Scan0, bytes, 0, numBytes);
                return bytes;
            }
        }
    }
}
