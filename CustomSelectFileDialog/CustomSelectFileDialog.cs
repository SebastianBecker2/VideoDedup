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
        private IEnumerable<Item>? content;
        private List<string> pathHistory = new();
        private int pathHistoryIndex = -1;

        public ItemType ItemType { get; set; } = ItemType.File;
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

        private DataGridViewRow ItemToRow(Item item)
        {
            var row = new DataGridViewRow { Tag = item, };

#pragma warning disable CA1305 // Specify IFormatProvider
            row.Cells.AddRange(
                new DataGridViewImageCell
                {
                    Value = item.GetIcon(),
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                },
                new DataGridViewTextBoxCell { Value = item.Name, },
                new DataGridViewTextBoxCell
                {
                    Value = item.DateModified?.ToString(),
                },
                new DataGridViewTextBoxCell { Value = item.MimeType, },
                new DataGridViewTextBoxCell { Value = item.Size, });
#pragma warning restore CA1305 // Specify IFormatProvider

            return row;
        }

        public void SetContent(IEnumerable<Item> items)
        {
            updatingSelectedPath = true;

            try
            {
                content = items.ToList();

                SelectedPath = string.Empty;
                TxtSelectedFileName.Text = string.Empty;
                DgvContent.Rows.Clear();

                DgvContent.Rows.AddRange(content
                    .OrderBy(i => (int)i.Type)
                    .ThenBy(i => i.Name)
                    .Select(ItemToRow)
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

                var item = DgvContent.SelectedRows[0].Tag as Item;
                Debug.Assert(item is not null);

                TxtSelectedFileName.Text = item.Name;
                SelectedPath = Path.Combine(CurrentPath ?? "", item.Name);
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
            var item1 = DgvContent.Rows[e.RowIndex1].Tag as Item;
            Debug.Assert(item1 is not null);
            var item2 = DgvContent.Rows[e.RowIndex2].Tag as Item;
            Debug.Assert(item2 is not null);

            e.Handled = item1.Type != item2.Type;
            e.SortResult = item1.Type == ItemType.Folder ? -1 : 1;
        }

        private void HandleBtnOkClick(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtSelectedFileName.Text))
            {
                return;
            }

            var selectedItem = content?
                .FirstOrDefault(c => c.Name == TxtSelectedFileName.Text);

            if (selectedItem is not null
                && selectedItem.Type == ItemType.Folder)
            {
                CurrentPath = Path.Combine(CurrentPath ?? "", selectedItem.Name);
                return;
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
                        var item = row.Tag as Item;
                        Debug.Assert(item is not null);
                        return item.Name == TxtSelectedFileName.Text;
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

        private void HandleDgvContentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Return or Keys.Enter)
            {
                BtnOk.PerformClick();
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
