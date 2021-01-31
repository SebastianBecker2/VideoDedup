using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("System.Runtime.Serialization")]
namespace VideoDedupShared
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class VideoFile : IVideoFile, IDisposable, IEquatable<IVideoFile>
    {
        internal VideoFile() { }

        public VideoFile(IVideoFile other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            FilePath = other.FilePath;
            FileSize = other.FileSize;
            Duration = other.Duration;
            CodecInfo = other.CodecInfo;

            ImageStreams = other.ImageStreams.Select(ms =>
            {
                ms.Position = 0;
                var @new = new MemoryStream();
                ms.CopyTo(@new);
                return @new;
            }).ToList();
        }

        [DataMember]
        public string FilePath { get; set; }

        public string FileName => Path.GetFileName(FilePath);

        [DataMember]
        public long FileSize { get; set; }

        [DataMember]
        public TimeSpan Duration { get; set; }

        [DataMember]
        public CodecInfo CodecInfo { get; set; }

        public IEnumerable<Image> Images
        {
            get
            {
                if (ImageStreams == null)
                {
                    return Enumerable.Empty<Image>();
                }
                return ImageStreams.Select(image => Image.FromStream(image));
            }
        }
        [DataMember]
        public IList<MemoryStream> ImageStreams { get; set; }

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
            foreach (var image in ImageStreams)
            {
                image.Dispose();
            }
            ImageStreams.Clear();
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
