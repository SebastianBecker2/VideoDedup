namespace CustomSelectFileDialog
{
    using System;
    using Properties;

    public class Item
    {
        public string Name { get; set; }
        public ItemType Type { get; set; } = ItemType.File;
        public long? Size { get; set; }
        public DateTime? DateModified { get; set; }
        public string? MimeType { get; set; }

        public Item(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    $"'{nameof(name)}' cannot be null or whitespace.",
                    nameof(name));
            }

            Name = name;
        }

        public Image GetIcon()
        {
            if (Type == ItemType.Folder)
            {
                return Resources.folder;
            }

            var ext = Path.GetExtension(Name);
            if (string.IsNullOrWhiteSpace(ext))
            {
                return Resources.file_generic;
            }

            var resourceObject = Resources.ResourceManager.GetObject(
                $"file_extension_{ext[1..]}",
                Resources.Culture);

            if (resourceObject is Image icon)
            {
                return icon;
            }

            return Resources.file_generic;
        }
    }
}
