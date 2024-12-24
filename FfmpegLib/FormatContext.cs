namespace FfmpegLib
{
    using System;
    using FFmpeg.AutoGen;

    internal unsafe class FormatContext : IDisposable
    {
        private AVFormatContext* formatContextPtr =
            ffmpeg.avformat_alloc_context();
        private bool disposedValue;

        public uint NbStreams
        {
            get => formatContextPtr->nb_streams;
            set => formatContextPtr->nb_streams = value;
        }
        public AVStream** Streams => formatContextPtr->streams;
        public long Duration => formatContextPtr->duration;

        public int Open(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(FormatContext),
                    "The FormatContext has already been disposed.");
            }

            fixed (AVFormatContext** formatContextPtrRef = &formatContextPtr)
            {
                return ffmpeg.avformat_open_input(
                    formatContextPtrRef,
                    filePath,
                    null,
                    null);
            }
        }

        public int FindStreamInfo()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(FormatContext),
                    "The FormatContext has already been disposed.");
            }

            return ffmpeg.avformat_find_stream_info(formatContextPtr, null);
        }

        public AVStream* GetVideoStream(bool deactivateOthers = false)
        {
            AVStream* stream = null;
            for (uint i = 0; i < formatContextPtr->nb_streams; i++)
            {
                if (stream is null
                    && formatContextPtr->streams[i]->codecpar->codec_type
                    == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    stream = formatContextPtr->streams[i];
                    if (!deactivateOthers)
                    {
                        return stream;
                    }
                    continue;
                }
                if (!deactivateOthers)
                {
                    continue;
                }
                formatContextPtr->streams[i]->discard = AVDiscard.AVDISCARD_ALL;
            }
            return stream;
        }

        public AVFormatContext* GetPointer()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(FormatContext),
                    "The FormatContext has already been disposed.");
            }

            return formatContextPtr;
        }

        private void DeleteFormatContext()
        {
            fixed (AVFormatContext** formatContextPtrRef = &formatContextPtr)
            {
                ffmpeg.avformat_close_input(formatContextPtrRef);
            }
            ffmpeg.avformat_free_context(formatContextPtr);
            formatContextPtr = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                DeleteFormatContext();
                disposedValue = true;
            }
        }

        ~FormatContext()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
