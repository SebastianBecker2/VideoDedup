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
        private IEnumerable<string>? filter;

        public IconStyle EntryIconStyle
        {
            get => entryIconStyle;
            set
            {
                entryIconStyle = value;
                RtvFolderTree.EntryIconStyle = value;
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
                UpdateDgvContent();
            }
        }
        public string SelectedPath { get; set; } = string.Empty;
        public IEnumerable<Entry>? RootFolders
        {
            get => RtvFolderTree.RootFolders;
            set => RtvFolderTree.RootFolders = value;
        }
        public IEnumerable<string>? Filter
        {
            get => filter;
            set
            {
                filter = value;
                CmbFilter.Items.Clear();
                if (value is null)
                {
                    CmbFilter.Visible = false;
                    return;
                }
                CmbFilter.Items.AddRange(value.ToArray());
                CmbFilter.SelectedIndex = value.Count() - 1;
            }
        }

        /// <summary>
        /// Event raised when content is requested.<br/>Provide content for
        /// currently selected path or throw InvalidContentRequestException
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
        protected virtual IEnumerable<Entry>? OnContentRequested(
            string? path,
            string? filter)
        {
            var args = new ContentRequestedEventArgs(
                path,
                RequestedEntryType.FilesAndFolders,
                filter);
            OnContentRequested(args);
            return args.Entries;
        }
        protected virtual IEnumerable<Entry>? OnSubFolderRequested(string? path)
        {
            var args = new ContentRequestedEventArgs(
                path,
                RequestedEntryType.Folders,
                null);
            OnContentRequested(args);
            if (args.Entries is not null && !args.Entries.Any())
            {
                return null;
            }
            return args.Entries;
        }

        /// <summary>
        /// Event raised when the user selected and confirmed a file or folder.
        /// <br/>The owner of the dialog can verify if the selection is valid.
        /// Setting the IsValid member to false will cancel the users
        /// confirmation.
        /// </summary>
        public event EventHandler<PathSelectedEventArgs>? PathSelected;

        protected virtual void OnPathSelected(PathSelectedEventArgs args) =>
            PathSelected?.Invoke(this, args);
        protected virtual bool OnPathSelected(string path)
        {
            var args = new PathSelectedEventArgs(path);
            OnPathSelected(args);
            return args.IsValid;
        }

        public CustomSelectFileDialog() => InitializeComponent();

        protected override void OnLoad(System.EventArgs e)
        {
            UpdateDgvContent();

            DgvContent.Sort(
                DgvContent.Columns[1],
                ListSortDirection.Ascending);

            DgvContent.ContextMenuStrip = new();
            var refresh = new ToolStripMenuItem
            {
                Text = "Refresh",
            };
            refresh.Click += (s, e) => RefreshContent();
            DgvContent.ContextMenuStrip.Items.Add(refresh);

            PabCurrentPath.RootFoldersRequested += (_, args) =>
                args.RootFolders = RootFolders?.Select(entry => entry.Name);

            PabCurrentPath.SubFoldersRequested += (_, args) =>
                args.SubFolders =
                    OnSubFolderRequested(args.Path)
                        ?.Select(entry => entry.Name);

            CmbFilter.Visible = filter is not null;

            RtvFolderTree.CurrentPathChanged += (_, e) => CurrentPath = e.Path;
            RtvFolderTree.SubFoldersRequested += (_, e) => OnContentRequested(e);
            RtvFolderTree.UpdateContent();

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

            row.ContextMenuStrip = new();

            var select = new ToolStripMenuItem
            {
                Text = "Select",
            };
            select.Click += (s, e) => ApplySelection();
            row.ContextMenuStrip.Items.Add(select);

            row.ContextMenuStrip.Opening += (s, e) =>
            {

                DgvContent.ClearSelection();
                row.Selected = true;
            };

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

        private void RefreshContent()
        {
            UpdateDgvContent();
            RtvFolderTree.UpdateContent();
        }

        private void UpdateDgvContent() =>
            SetContent(OnContentRequested(CurrentPath, GetSelectedFilter()));

        private void ApplySelection()
        {
            var selectedEntry = content?
                    .FirstOrDefault(c => c.Name == TxtSelectedFileName.Text);

            bool IsValidFolderSelection()
            {
                if (string.IsNullOrEmpty(TxtSelectedFileName.Text))
                {
                    Debug.Assert(CurrentPath is not null);
                    SelectedPath = CurrentPath;
                    return true;
                }

                if (selectedEntry is null
                    || selectedEntry.Type == EntryType.Folder)
                {
                    return true;
                }

                return false;
            }

            bool IsValidFileSelection()
            {
                if (string.IsNullOrEmpty(TxtSelectedFileName.Text))
                {
                    return false;
                }

                if (selectedEntry is not null
                    && selectedEntry.Type == EntryType.Folder)
                {
                    CurrentPath =
                        Path.Combine(CurrentPath ?? "", selectedEntry.Name);
                    return false;
                }

                return true;
            }

            if ((IsFolderSelector && !IsValidFolderSelection())
                || (!IsFolderSelector && !IsValidFileSelection()))
            {
                return;
            }

            if (PathSelected is null || OnPathSelected(SelectedPath))
            {
                DialogResult = DialogResult.OK;
            }
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

        private string? GetSelectedFilter()
        {
            if (Filter is null)
            {
                return null;
            }

            return CmbFilter.GetItemText(CmbFilter.SelectedItem);
        }

        private void HandleDgvContent_CellDoubleClick(
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

            ApplySelection();
        }

        private void HandleDgvContent_SelectionChanged(
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

        private void HandleDgvContent_SortCompare(
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

        private void HandleDgvContent_KeyDown(object sender, KeyEventArgs e)
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

                ApplySelection();
                return;
            }

            if (e.KeyCode is Keys.Back)
            {
                BtnUp.PerformClick();
            }
        }

        private void HandleDgvContent_MouseDown(object sender, MouseEventArgs e)
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

        private void HandleBtnOk_Click(object sender, System.EventArgs e) =>
            ApplySelection();

        private void HandleBtnUp_Click(object sender, System.EventArgs e)
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

        private void HandleBtnRefresh_Click(object sender, System.EventArgs e) =>
            RefreshContent();

        private void HandleTxtSelectedFileName_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Return or Keys.Enter)
            {
                ApplySelection();
            }
        }

        private void HandleTxtSelectedFileName_TextChanged(
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

        private void HandleBtnBack_Click(object sender, System.EventArgs e)
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

        private void HandleBtnForward_Click(object sender, System.EventArgs e)
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

        private void HandlePabCurrentPath_CurrentPathChanged(
            object sender,
            CurrentPathChangedEventArgs e)
        {
            if (CurrentPath != e.Path)
            {
                CurrentPath = e.Path;
            }

        }

        private void HandleCustomSelectFileDialog_KeyDown(
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

        private void HandleCmbFilter_SelectedIndexChanged(
            object sender,
            System.EventArgs e) =>
            UpdateDgvContent();
    }
}
