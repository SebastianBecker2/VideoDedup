namespace CustomSelectFileDlg
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal class History
    {
        private IList<(string path, Entry? selectedEntry)> history =
            new List<(string path, Entry? selectedEntry)>();
        private int index = -1;

        private void PrintHistory()
        {
            Debug.Print($"History has {history.Count} items. Current Index is {index}");
            foreach (var index in Enumerable.Range(0, history.Count))
            {
                Debug.Print($"{index}: {history[index].path}" +
                    $" -> {history[index].selectedEntry?.Name ?? "none"}");
            }
            Debug.Print("");
        }

        public void Add(string path)
        {
            Debug.Print($"*** Adding at index {index + 1} the path {path}");
            // Avoid consecutive duplicates
            if (index >= 0 && history[index].path == path)
            {
                return;
            }
            // Overwrite history when we at a new item in the middle
            if (index < history.Count)
            {
                history = history
                    .Take(index + 1)
                    .ToList();
            }
            history.Add((path, null));
            index++;
            PrintHistory();
        }

        public void SetSelection(Entry? selectedEntry)
        {
            if (index == -1)
            {
                return;
            }
            Debug.Print($"*** Updating index {index} with selection" +
                $" {selectedEntry?.Name ?? "none"}");
            history[index] = (history[index].path, selectedEntry);
            PrintHistory();
        }

        public (string path, Entry? selectedEntry) MoveForward()
        {
            var item = history[++index];
            Debug.Print($"*** Moving forward to index {index} with item" +
                $" {item.path}");
            return item;
        }

        public (string path, Entry? selectedEntry) MoveBackward()
        {
            var item = history[--index];
            Debug.Print($"*** Moving backward to index {index} with item" +
                $" {item.path}");
            return item;
        }

        public bool CanMoveBackward() => index > 0;

        public bool CanMoveForward() => index < history.Count - 1;
    }
}
