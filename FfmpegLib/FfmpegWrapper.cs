namespace FfmpegLib
{
    using System;
    using FFmpeg.AutoGen;
    using FfmpegLib.Exceptions;
    using VideoDedupGrpc;

    public class FfmpegWrapper
    {
        public static unsafe CodecInfo GetCodecInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FfmpegFileNotFoundException(
                    "Unable to read duration. The specified file was not found.",
                    filePath);
            }

            ffmpeg.avdevice_register_all();

            using var context = new FormatContext();
            if (context.GetPointer() is null)
            {
                throw new FfmpegOperationException(
                    "Unable to read duration. Unable to allocate resources.");
            }

            if (context.Open(filePath) < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to read codec information. Unable to open file.",
                    filePath);
            }

            if (context.FindStreamInfo() < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to read codec information. Stream not found.",
                    filePath);
            }

            var stream = GetVideoStream(context);
            if (stream is null)
            {
                throw new FfmpegOperationException(
                    "Unable to read codec information. Stream not found.",
                    filePath);
            }

            var frameRate = stream->avg_frame_rate;

            return new CodecInfo
            {
                Size = new Size
                {
                    Width = stream->codecpar->width,
                    Height = stream->codecpar->height,
                },
                Name = ffmpeg.avcodec_get_name(stream->codecpar->codec_id),
                FrameRate = frameRate.den != 0
                    ? (float)frameRate.num / frameRate.den
                    : (float)0.0,
            };
        }

        public static unsafe TimeSpan GetDuration(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FfmpegFileNotFoundException(
                    "Unable to read duration. The specified file was not found.",
                    filePath);
            }

            using var context = new FormatContext();
            if (context.GetPointer() is null)
            {
                throw new FfmpegOperationException(
                    "Unable to read duration. Unable to allocate resources.");
            }

            if (context.Open(filePath) < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to read codec information. Unable to open file.",
                    filePath);
            }

            if (context.FindStreamInfo() < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to read codec information. Stream not found.",
                    filePath);
            }

            if (context.Duration < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to read duration. Duration not available.");
            }

            var durationInSeconds = context.Duration / (double)ffmpeg.AV_TIME_BASE;
            return TimeSpan.FromSeconds(durationInSeconds);
        }

        public string FilePath { get; private set; }
        public TimeSpan Duration
        {
            get
            {
                duration ??= GetDuration(FilePath);
                return duration.Value;
            }
            private set => duration = value;
        }
        private TimeSpan? duration;

        public FfmpegWrapper(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be" +
                   " null or whitespace", nameof(filePath));
            }

            FilePath = filePath;
        }

        public IEnumerable<byte[]?> GetImages(
            int index,
            int count,
            int divisionCount) =>
            GetImages(null, index, count, divisionCount);

        public IEnumerable<byte[]?> GetImages(
            int index,
            int count,
            int divisionCount,
            CancellationToken cancelToken) =>
            GetImages(cancelToken, index, count, divisionCount);

        private IEnumerable<byte[]?> GetImages(
            CancellationToken? cancelToken,
            int index,
            int count,
            int divisionCount)
        {
            if (!File.Exists(FilePath))
            {
                throw new FfmpegFileNotFoundException(
                    "Unable to extract images. File not found.",
                    FilePath);
            }

            if (count <= 0)
            {
                yield break;
            }

            if (divisionCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(divisionCount),
                    $"{nameof(divisionCount)} cannot be less than zero.");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    $"{nameof(index)} cannot be less than zero.");
            }

            if (index + count > divisionCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    $"{nameof(index)} and {nameof(count)} must refer to a " +
                    $"location within {nameof(divisionCount)}.");
            }

            var indices = Enumerable.Range(index, count)
                .Select(i => new ImageIndex(i, divisionCount - 1));

            using var enumerator = new FfmpegImageEnumerator(
                FilePath, cancelToken, indices);

            foreach (var image in enumerator)
            {
                yield return image;
            }
        }

        public IEnumerable<byte[]?> GetImages(
            IEnumerable<ImageIndex> indices) =>
            GetImages(null, indices);

        public IEnumerable<byte[]?> GetImages(
            IEnumerable<ImageIndex> indices,
            CancellationToken cancelToken) =>
            GetImages(cancelToken, indices);

        private IEnumerable<byte[]?> GetImages(
            CancellationToken? cancelToken,
            IEnumerable<ImageIndex> indices)
        {
            if (!File.Exists(FilePath))
            {
                throw new FfmpegFileNotFoundException(
                    "Unable to extract images. File not found.",
                    FilePath);
            }

            ArgumentNullException.ThrowIfNull(indices);

            using var enumerator = new FfmpegImageEnumerator(
                FilePath, cancelToken, indices);

            foreach (var image in enumerator)
            {
                yield return image;
            }
        }

        internal static unsafe AVStream* GetVideoStream(FormatContext context)
        {
            for (uint i = 0; i < context.NbStreams; i++)
            {
                var stream = context.Streams[i];
                if (stream->codecpar->codec_type
                    != AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    continue;
                }
                return stream;
            }
            return null;
        }
    }
}
