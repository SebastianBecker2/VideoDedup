namespace VideoDedupShared.DataGridViewExtension
{
    using System.Windows.Forms;

    public static class DataGridViewExtension
    {
        public static int GetLastDisplayedScrollingRowIndex(
            this DataGridView dataGridView,
            bool includePartialRow) =>
            dataGridView.FirstDisplayedScrollingRowIndex +
                dataGridView.DisplayedRowCount(includePartialRow) - 1;
    }
}
