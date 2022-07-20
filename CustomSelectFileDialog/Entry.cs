namespace CustomSelectFileDialog
{
    using System;
    using Properties;

    public class Entry
    {
        public Image? Icon { get; set; }
        public string Name { get; set; }
        public EntryType Type { get; set; } = EntryType.File;
        public long? Size { get; set; }
        public DateTime? DateModified { get; set; }
        public string? MimeType { get; set; }

        public Entry(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    $"'{nameof(name)}' cannot be null or whitespace.",
                    nameof(name));
            }

            Name = name.TrimEnd('\\').Trim('/');
        }

        public Image? GetIcon(IconStyle iconStyle)
        {
            if (iconStyle == IconStyle.NoIcon)
            {
                return null;
            }

            if (Icon is not null
                || iconStyle == IconStyle.NoFallbackOnNull)
            {
                return Icon;
            }

            if (Type == EntryType.Folder)
            {
                return Resources.folder;
            }

            if (iconStyle == IconStyle.FallbackToExtensionSpecificIcons)
            {
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
            }

            return Resources.file_generic;
        }
    }
}
