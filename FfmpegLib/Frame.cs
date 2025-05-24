namespace FfmpegLib
{
    using System;
    using FFmpeg.AutoGen;

    internal sealed unsafe class Frame : IDisposable, IFrame
    {
        private AVFrame* framePtr = ffmpeg.av_frame_alloc();
        private bool disposedValue;

        public long BestEffortTimestamp => framePtr->best_effort_timestamp;
        public AVPictureType PictType => framePtr->pict_type;

        public bool HasData { get; private set; }

        public int ReceiveFrame(CodecContext codecContext) =>
            ReceiveFrame(codecContext.GetPointer());
        public int ReceiveFrame(AVCodecContext* codecContext)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(Frame),
                    "The Frame has already been disposed.");
            }

            Unreference();
            var res = ffmpeg.avcodec_receive_frame(codecContext, framePtr);
            HasData = res >= 0;
            return res;
        }

        public AVFrame* GetPointer()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(Frame),
                    "The Frame has already been disposed.");
            }

            return framePtr;
        }

        private void Unreference()
        {
            if (!HasData)
            {
                return;
            }

            ffmpeg.av_frame_unref(framePtr);
            HasData = false;
        }

        private void DeleteFrame()
        {
            Unreference();

            fixed (AVFrame** framePtrRef = &framePtr)
            {
                ffmpeg.av_frame_free(framePtrRef);
            }
            framePtr = null;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                DeleteFrame();
                disposedValue = true;
            }
        }

        ~Frame()
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
