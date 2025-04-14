namespace FfmpegLib
{
    using System;
    using FFmpeg.AutoGen;
    using FfmpegLib.Exceptions;

    internal unsafe class CodecContext : IDisposable
    {
        private readonly AVCodecContext* codecContextPtr;
        private bool disposedValue;

        public int Width
        {
            get => codecContextPtr->width;
            set => codecContextPtr->width = value;
        }
        public int Height
        {
            get => codecContextPtr->height;
            set => codecContextPtr->height = value;
        }
        public AVPixelFormat PixFmt
        {
            get => codecContextPtr->pix_fmt;
            set => codecContextPtr->pix_fmt = value;
        }
        public int GlobalQuality
        {
            get => codecContextPtr->global_quality;
            set => codecContextPtr->global_quality = value;
        }
        public AVRational TimeBase
        {
            get => codecContextPtr->time_base;
            set => codecContextPtr->time_base = value;
        }
        public long BitRate
        {
            get => codecContextPtr->bit_rate;
            set => codecContextPtr->bit_rate = value;
        }

        public List<AVPixelFormat> SupportedPixelFormats { get; private set; } = [];

        public CodecContext(AVCodec* codec)
        {
            ArgumentNullException.ThrowIfNull(codec, nameof(codec));

            codecContextPtr = ffmpeg.avcodec_alloc_context3(codec);
            if (codecContextPtr == null)
            {
                throw new FfmpegOperationException(
                    "Unable to allocate codec context.");
            }

            var ptr = codecContextPtr->codec->pix_fmts;
            if (ptr is null)
            {
                return;
            }
            while (*ptr != AVPixelFormat.AV_PIX_FMT_NONE)
            {
                SupportedPixelFormats.Add(*ptr);
                ptr++;
            }
        }

        public int Open(AVCodec* codec)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(CodecContext),
                    "The CodecContext has already been disposed.");
            }

            return ffmpeg.avcodec_open2(codecContextPtr, codec, null);
        }

        public int ParametersToContext(AVCodecParameters* codecParameters)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(CodecContext),
                    "The CodecContext has already been disposed.");
            }

            return ffmpeg.avcodec_parameters_to_context(
                codecContextPtr,
                codecParameters);
        }

        public void FlushBuffers()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(CodecContext),
                    "The CodecContext has already been disposed.");
            }

            ffmpeg.avcodec_flush_buffers(codecContextPtr);
        }

        public int SendPacket(Packet packet) => SendPacket(packet.GetPointer());
        public int SendPacket(AVPacket* packet)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(CodecContext),
                    "The CodecContext has already been disposed.");
            }

            return ffmpeg.avcodec_send_packet(codecContextPtr, packet);
        }

        public int SendFrame(IFrame frame) => SendFrame(frame.GetPointer());
        public int SendFrame(AVFrame* frame)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(CodecContext),
                    "The CodecContext has already been disposed.");
            }

            return ffmpeg.avcodec_send_frame(codecContextPtr, frame);
        }

        public AVCodecContext* GetPointer()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(CodecContext),
                    "The CodecContext has already been disposed.");
            }

            return codecContextPtr;
        }

        private void DeleteCodecContext()
        {
            fixed (AVCodecContext** codecContextPtrRef = &codecContextPtr)
            {
                ffmpeg.avcodec_free_context(codecContextPtrRef);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                DeleteCodecContext();
                disposedValue = true;
            }
        }

        ~CodecContext()
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
