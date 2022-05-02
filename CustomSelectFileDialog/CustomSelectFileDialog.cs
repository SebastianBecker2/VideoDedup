namespace CustomSelectFileDialog
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Forms;
    using EventArgs;

    public partial class CustomSelectFileDialog : Form
    {
        private string? currentPath;
        private bool updatingSelectedPath;
        private bool applyingHistory;
        private IEnumerable<Entry>? content;
        private List<string> pathHistory = new();
        private int pathHistoryIndex = -1;
        private IconStyle entryIconStyle = IconStyle.NoFallbackOnNull;

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
        public EntryType EntryType { get; set; } = EntryType.File;
        public string? CurrentPath
        {
            get => currentPath;
            set
            {
                currentPath = value;
                if (!applyingHistory && value is not null)
                {
                    if (pathHistoryIndex < pathHistory.Count)
                    {
                        pathHistory = pathHistory
                            .Take(pathHistoryIndex + 1)
                            .ToList();
                    }
                    pathHistory.Add(value);
                    pathHistoryIndex++;
                    UpdateHistoryButtons();
                }
                TxtCurrentPath.Text = value;
                OnContentRequested();
            }
        }
        public string SelectedPath { get; set; } = string.Empty;

        public event EventHandler<ContentRequestedEventArgs>? ContentRequested;
        protected virtual void OnContentRequested(string? path) =>
            ContentRequested?.Invoke(this, new ContentRequestedEventArgs(path));
        protected virtual void OnContentRequested() =>
            OnContentRequested(CurrentPath);

        public CustomSelectFileDialog() => InitializeComponent();

        protected override void OnLoad(System.EventArgs e)
        {
            OnContentRequested();
            DgvContent.Sort(DgvContent.Columns[1], ListSortDirection.Ascending);
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

        public void SetContent(IEnumerable<Entry> entries)
        {
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

        private void UpdateHistoryButtons()
        {
            BtnBack.Enabled = pathHistoryIndex > 0;
            BtnForward.Enabled = pathHistoryIndex < pathHistory.Count - 1;
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

            var selectedEntry = DgvContent.SelectedRows[0].Tag as Entry;
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
                if (DgvContent.SelectedRows.Count == 0)
                {
                    return;
                }

                var entry = DgvContent.SelectedRows[0].Tag as Entry;
                Debug.Assert(entry is not null);

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
                var selectedEntry = DgvContent.SelectedRows[0].Tag as Entry;
                Debug.Assert(selectedEntry is not null);

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
                if (EntryType == EntryType.File
                    && selectedEntry.Type == EntryType.Folder)
                {
                    CurrentPath =
                        Path.Combine(CurrentPath ?? "", selectedEntry.Name);
                    return;
                }

                if (EntryType == EntryType.Folder
                    && selectedEntry.Type != EntryType.Folder)
                {
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void HandleBtnUpClick(object sender, System.EventArgs e) =>
            CurrentPath = Directory.GetParent(CurrentPath ?? "")?.FullName;

        private void HandleBtnRefreshClick(object sender, System.EventArgs e) =>
            OnContentRequested();

        private void HandleTxtCurrentPathKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Return or Keys.Enter)
            {
                CurrentPath = TxtCurrentPath.Text;
            }
        }

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

                var rowToSelect = DgvContent.Rows
                    .OfType<DataGridViewRow>()
                    .FirstOrDefault(row =>
                    {
                        var entry = row.Tag as Entry;
                        Debug.Assert(entry is not null);
                        return entry.Name == TxtSelectedFileName.Text;
                    });
                if (rowToSelect is not null)
                {
                    rowToSelect.Selected = true;
                }

                SelectedPath = TxtSelectedFileName.Text;
            }
            finally
            {
                updatingSelectedPath = false;
            }
        }

        private void HandleBtnBackClick(object sender, System.EventArgs e)
        {
            if (pathHistoryIndex <= 0)
            {
                return;
            }

            applyingHistory = true;
            try
            {
                pathHistoryIndex--;
                CurrentPath = pathHistory[pathHistoryIndex];
                UpdateHistoryButtons();
            }
            finally
            {
                applyingHistory = false;
            }
        }

        private void BtnForward_Click(object sender, System.EventArgs e)
        {
            if (pathHistoryIndex >= pathHistory.Count - 1)
            {
                return;
            }

            applyingHistory = true;
            try
            {
                pathHistoryIndex++;
                CurrentPath = pathHistory[pathHistoryIndex];
                UpdateHistoryButtons();
            }
            finally
            {
                applyingHistory = false;
            }
        }
    }
}
