namespace CustomSelectFileDlg
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using EventArgs;
    using Exceptions;
    using Properties;

    public partial class CustomSelectFileDialog : Form
    {
        private string? currentPath;
        private bool updatingSelectedPath;
        private bool applyingHistory;
        private IEnumerable<Entry>? content;
        private readonly History pathHistory = new();
        private IconStyle entryIconStyle = IconStyle.NoFallbackOnNull;
        private ButtonUpEnabledWhen buttonUpEnabled =
            ButtonUpEnabledWhen.NotInRootDirectory;

        public IconStyle EntryIconStyle
        {
            get => entryIconStyle;
            set
            {
                entryIconStyle = value;
                DgvContent.Columns[0].Visible =
                    entryIconStyle != IconStyle.NoIcon;
            }
        }
        public bool IsFolderSelector { get; set; }
        public ButtonUpEnabledWhen ButtonUpEnabled
        {
            get => buttonUpEnabled;
            set
            {
                buttonUpEnabled = value;
                UpdateButtonUp();
            }
        }
        public string? CurrentPath
        {
            get => currentPath;
            set
            {
                currentPath = value;
                if (!applyingHistory && currentPath is not null)
                {
                    pathHistory.SetSelection(GetSelectedEntry());
                    pathHistory.Add(currentPath);
                    UpdateHistoryButtons();
                }
                PabCurrentPath.CurrentPath = value;
                UpdateButtonUp();
                SetContent(OnContentRequested());
            }
        }
        public string SelectedPath { get; set; } = string.Empty;
        public IEnumerable<Entry>? RootFolders { get; set; }

        /// <summary>
        /// Event raised when content is requested.<br/>Provide content for
        /// currently for provided path or throw InvalidContentRequestException
        /// to display an error to the user.
        /// </summary>
        public event EventHandler<ContentRequestedEventArgs>? ContentRequested;

        protected virtual void OnContentRequested(ContentRequestedEventArgs args)
        {
            try
            {
                ContentRequested?.Invoke(this, args);
            }
            catch (InvalidContentRequestException exc)
            {
                _ = MessageBox.Show(
                    exc.Message,
                    Resources.InvalidContentRequestExceptionTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        protected virtual IEnumerable<Entry>? OnContentRequested(string? path)
        {
            var args = new ContentRequestedEventArgs(path);
            OnContentRequested(args);
            return args.Entries;
        }
        protected virtual IEnumerable<Entry>? OnContentRequested() =>
            OnContentRequested(CurrentPath);
        protected virtual IEnumerable<Entry>? OnSubFolderRequested(string? path)
        {
            var args = new ContentRequestedEventArgs(path, true);
            OnContentRequested(args);
            if (args.Entries is not null && !args.Entries.Any())
            {
                return null;
            }
            return args.Entries;
        }

        public CustomSelectFileDialog() => InitializeComponent();

        protected override void OnLoad(System.EventArgs e)
        {
            SetContent(OnContentRequested());
            DgvContent.Sort(DgvContent.Columns[1], ListSortDirection.Ascending);
            PabCurrentPath.RootFoldersRequested += (_, args) =>
                args.RootFolders = RootFolders?.Select(entry => entry.Name);
            PabCurrentPath.SubFoldersRequested += (_, args) =>
                args.SubFolders =
                    OnSubFolderRequested(args.Path)?.Select(entry => entry.Name);
            base.OnLoad(e);
        }

        private DataGridViewRow EntryToRow(Entry entry)
        {
            var row = new DataGridViewRow { Tag = entry, };

#pragma warning disable CA1305 // Specify IFormatProvider
            row.Cells.AddRange(
                new DataGridViewImageCell
                {
                    Value = entry.GetIcon(EntryIconStyle),
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                },
                new DataGridViewTextBoxCell { Value = entry.Name, },
                new DataGridViewTextBoxCell
                {
                    Value = entry.DateModified?.ToString(),
                    ToolTipText = entry.DateModified?.ToString(),
                },
                new DataGridViewTextBoxCell
                {
                    Value = entry.MimeType,
                    ToolTipText = entry.MimeType,
                },
                new DataGridViewTextBoxCell
                {
                    Value = entry.Size,
                    ToolTipText = entry.Size?.ToString()
                });
#pragma warning restore CA1305 // Specify IFormatProvider

            return row;
        }

        public void SetContent(IEnumerable<Entry>? entries)
        {
            if (entries is null)
            {
                return;
            }

            updatingSelectedPath = true;

            try
            {
                content = entries.ToList();

                SelectedPath = string.Empty;
                TxtSelectedFileName.Text = string.Empty;
                DgvContent.Rows.Clear();

                DgvContent.Rows.AddRange(content
                    .OrderBy(i => (int)i.Type)
                    .ThenBy(i => i.Name)
                    .Select(EntryToRow)
                    .ToArray());

                DgvContent.ClearSelection();
            }
            finally
            {
                updatingSelectedPath = false;
            }
        }

        /// <summary>
        /// Set the CurrentPath to a new, proposed path.
        /// </summary>
        /// <param name="proposedPath">
        /// The path to be set as the CurrentPath.
        /// </param>
        /// <returns>
        /// True - When the proposed path was accepted by the content provider.
        /// <br/> False - When the proposed path was overwritten by the content
        /// provider.
        /// </returns>
        public bool SetCurrentPath(string? proposedPath)
        {
            CurrentPath = proposedPath;
            return CurrentPath == proposedPath;
        }

        private void UpdateButtonUp() =>
            BtnUp.Enabled = ButtonUpEnabled == ButtonUpEnabledWhen.Always ||
                Path.GetDirectoryName(currentPath) != null;

        private void UpdateHistoryButtons()
        {
            BtnBack.Enabled = pathHistory.CanMoveBackward();
            BtnForward.Enabled = pathHistory.CanMoveForward();
        }

        private void SelectEntry(string name)
        {
            var rowToSelect = DgvContent.Rows
                .OfType<DataGridViewRow>()
                .FirstOrDefault(row =>
                {
                    var entry = row.Tag as Entry;
                    Debug.Assert(entry is not null);
                    return entry.Name == name;
                });
            if (rowToSelect is not null)
            {
                rowToSelect.Selected = true;
            }
        }

        private Entry? GetSelectedEntry()
        {
            if (DgvContent.SelectedRows.Count == 0)
            {
                return null;
            }

            var entry = DgvContent.SelectedRows[0].Tag as Entry;
            Debug.Assert(entry is not null);
            return entry;
        }

        private void HandleDgvContentCellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= DgvContent.RowCount)
            {
                return;
            }

            if (e.ColumnIndex < 0 || e.ColumnIndex >= DgvContent.ColumnCount)
            {
                return;
            }

            var selectedEntry = GetSelectedEntry();
            Debug.Assert(selectedEntry is not null);

            if (selectedEntry.Type == EntryType.Folder)
            {
                CurrentPath =
                    Path.Combine(CurrentPath ?? "", selectedEntry.Name);
                return;
            }

            BtnOk.PerformClick();
        }

        private void HandleDgvContentSelectionChanged(
            object sender,
            System.EventArgs e)
        {
            if (updatingSelectedPath)
            {
                return;
            }
            updatingSelectedPath = true;

            try
            {
                var entry = GetSelectedEntry();
                if (entry is null)
                {
                    return;
                }

                TxtSelectedFileName.Text = entry.Name;
                SelectedPath = Path.Combine(CurrentPath ?? "", entry.Name);
            }
            finally
            {
                updatingSelectedPath = false;
            }
        }

        private void HandleDgvContentSortCompare(
            object sender,
            DataGridViewSortCompareEventArgs e)
        {
            var entry1 = DgvContent.Rows[e.RowIndex1].Tag as Entry;
            Debug.Assert(entry1 is not null);
            var entry2 = DgvContent.Rows[e.RowIndex2].Tag as Entry;
            Debug.Assert(entry2 is not null);

            e.Handled = entry1.Type != entry2.Type;
            e.SortResult = entry1.Type == EntryType.Folder ? -1 : 1;
        }

        private void HandleDgvContentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Return or Keys.Enter)
            {
                var selectedEntry = GetSelectedEntry();
                if (selectedEntry is null)
                {
                    return;
                }

                if (selectedEntry.Type == EntryType.Folder)
                {
                    CurrentPath =
                        Path.Combine(CurrentPath ?? "", selectedEntry.Name);
                    return;
                }

                BtnOk.PerformClick();
                return;
            }

            if (e.KeyCode is Keys.Back)
            {
                BtnUp.PerformClick();
            }
        }

        private void HandleDgvContentMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.XButton1)
            {
                BtnBack.PerformClick();
                return;
            }

            if (e.Button == MouseButtons.XButton2)
            {
                BtnForward.PerformClick();
                return;
            }
        }

        private void HandleBtnOkClick(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtSelectedFileName.Text))
            {
                return;
            }

            var selectedEntry = content?
                .FirstOrDefault(c => c.Name == TxtSelectedFileName.Text);

            if (selectedEntry is not null)
            {
                if (!IsFolderSelector
                    && selectedEntry.Type == EntryType.Folder)
                {
                    CurrentPath =
                        Path.Combine(CurrentPath ?? "", selectedEntry.Name);
                    return;
                }

                if (IsFolderSelector
                    && selectedEntry.Type != EntryType.Folder)
                {
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void HandleBtnUpClick(object sender, System.EventArgs e)
        {
            var previousPath = CurrentPath;
            if (!SetCurrentPath(Path.GetDirectoryName(CurrentPath) ?? ""))
            {
                return;
            }

            var previousFolderName = Path.GetFileName(previousPath);
            if (string.IsNullOrWhiteSpace(previousFolderName))
            {
                return;
            }

            SelectEntry(previousFolderName);
        }

        private void HandleBtnRefreshClick(object sender, System.EventArgs e) =>
            OnContentRequested();

        private void HandleTxtSelectedFileNameKeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Return or Keys.Enter)
            {
                BtnOk.PerformClick();
            }
        }

        private void HandleTxtSelectedFileNameTextChanged(
            object sender,
            System.EventArgs e)
        {
            if (updatingSelectedPath)
            {
                return;
            }
            updatingSelectedPath = true;

            try
            {
                DgvContent.ClearSelection();

                SelectEntry(TxtSelectedFileName.Text);
                if (CurrentPath is not null)
                {
                    SelectedPath =
                        Path.Combine(CurrentPath, TxtSelectedFileName.Text);
                }
            }
            finally
            {
                updatingSelectedPath = false;
            }
        }

        private void HandleBtnBackClick(object sender, System.EventArgs e)
        {
            if (!pathHistory.CanMoveBackward())
            {
                return;
            }

            applyingHistory = true;
            try
            {
                pathHistory.SetSelection(GetSelectedEntry());
                var (path, selectedEntry) = pathHistory.MoveBackward();
                if (!SetCurrentPath(path))
                {
                    return;
                }
                if (selectedEntry is not null)
                {
                    SelectEntry(selectedEntry.Name);
                }
            }
            finally
            {
                UpdateHistoryButtons();
                applyingHistory = false;
            }
        }

        private void HandleBtnForwardClick(object sender, System.EventArgs e)
        {
            if (!pathHistory.CanMoveForward())
            {
                return;
            }

            applyingHistory = true;
            try
            {
                pathHistory.SetSelection(GetSelectedEntry());
                var (path, selectedEntry) = pathHistory.MoveForward();
                if (!SetCurrentPath(path))
                {
                    return;
                }
                if (selectedEntry is not null)
                {
                    SelectEntry(selectedEntry.Name);
                }
            }
            finally
            {
                UpdateHistoryButtons();
                applyingHistory = false;
            }
        }

        private void HandlePabCurrentPathCurrentPathChanged(
            object sender,
            CurrentPathChangedEventArgs e)
        {
            if (CurrentPath != e.Path)
            {
                CurrentPath = e.Path;
            }

        }

        private void HandleCustomSelectFileDialogKeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape)
            {
                return;
            }

            if (PabCurrentPath.DisplayStyle == PathDisplayStyle.TextBox)
            {
                PabCurrentPath.DisplayStyle = PathDisplayStyle.Buttons;
                return;
            }

            DialogResult = BtnCancel.DialogResult;
        }
    }
}
