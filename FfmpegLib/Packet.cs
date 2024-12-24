namespace FfmpegLib
{
    using System;
    using FFmpeg.AutoGen;

    internal unsafe class Packet : IDisposable
    {
        private AVPacket* packetPtr = ffmpeg.av_packet_alloc();
        private bool disposedValue;

        public int Size => packetPtr->size;
        public byte* Data => packetPtr->data;

        public bool HasData { get; private set; }

        public int StreamIndex => packetPtr->stream_index;

        public int ReadFrame(FormatContext formatContext) =>
            ReadFrame(formatContext.GetPointer());
        public int ReadFrame(AVFormatContext* formatContext)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(Packet),
                    "The Packet has already been disposed.");
            }

            Unreference();
            var res = ffmpeg.av_read_frame(formatContext, packetPtr);
            HasData = res >= 0;
            return res;
        }

        public int ReceivePacket(CodecContext codecContext) =>
            ReceivePacket(codecContext.GetPointer());
        public int ReceivePacket(AVCodecContext* codecContext)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(Packet),
                    "The Packet has already been disposed.");
            }

            Unreference();
            var res = ffmpeg.avcodec_receive_packet(codecContext, packetPtr);
            HasData = res >= 0;
            return res;
        }

        public AVPacket* GetPointer()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(Packet),
                    "The Packet has already been disposed.");
            }

            return packetPtr;
        }

        private void Unreference()
        {
            if (!HasData)
            {
                return;
            }

            ffmpeg.av_packet_unref(packetPtr);
            HasData = false;
        }

        private void DeletePacket()
        {
            Unreference();

            fixed (AVPacket** packetPtrRef = &packetPtr)
            {
                ffmpeg.av_packet_free(packetPtrRef);
            }
            packetPtr = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                DeletePacket();
                disposedValue = true;
            }
        }

        ~Packet()
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
