namespace CustomSelectFileDlg
{
    using System.ComponentModel;
    using System.Diagnostics;
    using EventArgs;
    using System.Linq;

    [ToolboxItem(true)]
    internal partial class PathBox : UserControl
    {
        private string? currentPath = "C:/User/Control";
        private bool controlLoaded;
        private PathDisplayStyle displayStyle = PathDisplayStyle.Buttons;

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue("C:/User/Control")]
        public string? CurrentPath
        {
            get => currentPath; set
            {
                DisplayStyle = PathDisplayStyle.Buttons;
                if (currentPath == value)
                {
                    return;
                }
                currentPath = value;
                UpdatePathDisplay();
                OnCurrentPathChanged(CurrentPath);
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(typeof(PathDisplayStyle), "Buttons")]
        public PathDisplayStyle DisplayStyle
        {
            get => displayStyle;
            set
            {
                displayStyle = value;
                RbaButtons.Visible = displayStyle == PathDisplayStyle.Buttons;
                TxtPath.Visible = displayStyle == PathDisplayStyle.TextBox;
                TxtPath.Text = CurrentPath;
            }
        }

        /// <summary>
        /// Event raised when path has been changed.<br/>When the user confirms
        /// the path that is displayed in the TextBox using Enter or Return key.
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler<CurrentPathChangedEventArgs>? CurrentPathChanged;
        protected virtual void OnCurrentPathChanged(string? path) =>
            CurrentPathChanged?.Invoke(this, new CurrentPathChangedEventArgs(path));

        public event EventHandler<SubFoldersRequestedEventArgs>?
            SubFoldersRequested;
        protected virtual IEnumerable<string>? OnSubFolderRequested(string path)
        {
            var eventArgs = new SubFoldersRequestedEventArgs(path);
            SubFoldersRequested?.Invoke(this, eventArgs);
            return eventArgs.SubFolders;
        }

        public event EventHandler<RootFoldersRequestedEventArgs>?
            RootFoldersRequested;
        protected virtual IEnumerable<string>? OnRootFolderRequested()
        {
            var eventArgs = new RootFoldersRequestedEventArgs();
            RootFoldersRequested?.Invoke(this, eventArgs);
            return eventArgs.RootFolders;
        }

        public PathBox() => InitializeComponent();

        protected override void OnLoad(System.EventArgs e)
        {
            RbaButtons.RootElementsRequested += (_, args) =>
                args.RootElements = OnRootFolderRequested()
                    ?.Select(f => new ResizableButtonArray.Element(f, f));
            RbaButtons.SubElementRequested += (_, args) =>
            {
                if (args.Element?.Tag is not string path)
                {
                    return;
                }

                var parentPath = Path.GetDirectoryName(path);
                if (parentPath is null)
                {
                    return;
                }

                args.SubElements = OnSubFolderRequested(parentPath)
                    ?.Select(f =>
                        new ResizableButtonArray.Element(
                            f,
                            Path.Combine(parentPath, f)));
            };
            controlLoaded = true;
            UpdatePathDisplay();
            base.OnLoad(e);
        }

        private void HandleRbaButtonsElementClick(
            object sender,
            ElementClickEventArgs e)
        {
            Debug.Assert(
                RbaButtons.Elements != null,
                "RbaButtons.Elements != null");

            CurrentPath = e.Element.Tag as string;
        }

        private void UpdatePathDisplay()
        {
            if (!controlLoaded)
            {
                return;
            }

            var rbaElements = new List<ResizableButtonArray.Element>();
            if (CurrentPath is null)
            {
                RbaButtons.Elements = rbaElements;
                return;
            }

            var path = CurrentPath
                .StartsWith("\\\\", StringComparison.InvariantCulture)
                ? "\\\\"
                : string.Empty;

            foreach (var folderName in CurrentPath.Replace('\\', '/')
                         .Split('/')
                         .Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                path = Path.Combine(path, folderName);
                rbaElements.Add(new ResizableButtonArray.Element(
                    folderName,
                    path));
            }

            RbaButtons.Elements = rbaElements;
        }

        private void HandleTxtPathKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Return or Keys.Enter)
            {
                CurrentPath = TxtPath.Text;
            }
        }

        private void HandleTxtPathLeave(object sender, System.EventArgs e) =>
            DisplayStyle = PathDisplayStyle.Buttons;

        private void HandleRbaButtonsClick(object sender, System.EventArgs e)
        {
            DisplayStyle = PathDisplayStyle.TextBox;
            _ = TxtPath.Focus();
            TxtPath.SelectAll();
        }
    }
}
