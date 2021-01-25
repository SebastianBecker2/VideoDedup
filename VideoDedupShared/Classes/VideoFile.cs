using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("System.Runtime.Serialization")]
namespace VideoDedupShared
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;

    [DataContract]
    public class VideoFile : IVideoFile, IDisposable
    {
        internal VideoFile() { }

        public VideoFile(IVideoFile other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            filePath = other.FilePath;
            fileSize = other.FileSize;
            duration = other.Duration;
            codecInfo = other.CodecInfo;

            imageStreams = other.ImageStreams.Select(ms =>
            {
                ms.Position = 0;
                var @new = new MemoryStream();
                ms.CopyTo(@new);
                return @new;
            }).ToList();
        }

        public string FilePath => filePath;
        [DataMember]
        protected internal string filePath;

        public string FileName => Path.GetFileName(FilePath);

        public long FileSize => fileSize;
        [DataMember]
        protected internal long fileSize;

        public TimeSpan Duration => duration;
        [DataMember]
        protected internal TimeSpan duration;

        public CodecInfo CodecInfo => codecInfo;
        [DataMember]
        protected internal CodecInfo codecInfo;

        public IEnumerable<Image> Images
        {
            get
            {
                if (imageStreams == null)
                {
                    return Enumerable.Empty<Image>();
                }
                return imageStreams.Select(image => Image.FromStream(image));
            }
        }
        public IEnumerable<MemoryStream> ImageStreams => imageStreams;
        [DataMember]
        protected internal IList<MemoryStream> imageStreams =
            new List<MemoryStream>();

        public override bool Equals(object obj) => Equals(obj as IVideoFile);

        public bool Equals(IVideoFile other) => other != null &&
                   FilePath == other.FilePath;

        public override int GetHashCode() =>
            1230029444 + EqualityComparer<string>.Default.GetHashCode(FilePath);

        public static bool operator ==(VideoFile left, IVideoFile right) =>
            EqualityComparer<IVideoFile>.Default.Equals(left, right);

        public static bool operator !=(VideoFile left, IVideoFile right) =>
            !(left == right);

        // For the IDisposable pattern
        private bool disposedValue;

        public void DisposeImages()
        {
            foreach (var image in imageStreams)
            {
                image.Dispose();
            }
            imageStreams.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeImages();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
