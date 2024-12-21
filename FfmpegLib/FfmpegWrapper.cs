namespace FfmpegLib
{
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

            var context = ffmpeg.avformat_alloc_context();
            if (context is null)
            {
                throw new FfmpegOperationException(
                    "Unable to read duration. Unable to allocate resources.");
            }

            try
            {
                if (ffmpeg.avformat_open_input(
                    &context,
                    filePath,
                    null,
                    null) != 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to read codec information. Unable to open file.",
                        filePath);
                }

                if (ffmpeg.avformat_find_stream_info(context, null) < 0)
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

                var codecId = ffmpeg.avcodec_find_decoder(
                    stream->codecpar->codec_id);
                if (codecId is null)
                {
                    throw new FfmpegOperationException(
                        "Unable to read codec information. Codec not found.",
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
            finally
            {
                ffmpeg.avformat_close_input(&context);
                ffmpeg.avformat_free_context(context);
            }
        }

        public static unsafe TimeSpan GetDuration(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FfmpegFileNotFoundException(
                    "Unable to read duration. The specified file was not found.",
                    filePath);
            }

            var context = ffmpeg.avformat_alloc_context();
            if (context is null)
            {
                throw new FfmpegOperationException(
                    "Unable to read duration. Unable to allocate resources.");
            }

            try
            {
                if (ffmpeg.avformat_open_input(
                    &context,
                    filePath,
                    null,
                    null) != 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to read duration. Unable to open file.");
                }

                if (ffmpeg.avformat_find_stream_info(context, null) < 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to read duration. Stream not found.");
                }

                if (context->duration < 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to read duration. Duration not available.");
                }

                var durationInSeconds = context->duration / (double)ffmpeg.AV_TIME_BASE;
                return TimeSpan.FromSeconds(durationInSeconds);
            }
            finally
            {
                ffmpeg.avformat_close_input(&context);
                ffmpeg.avformat_free_context(context);
            }
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

        public FfmpegWrapper(string filePath, TimeSpan? duration = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be" +
                   " null or whitespace", nameof(filePath));
            }

            FilePath = filePath;
            this.duration = duration;
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

        public IEnumerable<byte[]?> GetImages(
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

            var enumerator = new FfmpegImageEnumerator(
                FilePath, cancelToken, index, count, divisionCount);

            foreach (var image in enumerator)
            {
                yield return image;
            }
        }

        //public IEnumerable<byte[]?> GetImages(
        //    IEnumerable<ImageIndex> indices)
        //{

        //}

        //public IEnumerable<byte[]?> GetImages(
        //    IEnumerable<ImageIndex> indices,
        //    CancellationToken cancelToken)
        //{

        //}

        internal static unsafe AVStream* GetVideoStream(AVFormatContext* context)
        {
            for (uint i = 0; i < context->nb_streams; i++)
            {
                var stream = context->streams[i];
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
