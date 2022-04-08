namespace VideoDedupSharedLib.ExtensionMethods.DataGridViewExtensions
{
    using System.Windows.Forms;

    public static class DataGridViewExtensions
    {
        public static int GetLastDisplayedScrollingRowIndex(
            this DataGridView dataGridView,
            bool includePartialRow) =>
            dataGridView.FirstDisplayedScrollingRowIndex +
                dataGridView.DisplayedRowCount(includePartialRow) - 1;
    }
}
