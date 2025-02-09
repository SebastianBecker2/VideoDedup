namespace FfmpegLib
{
    using System;
    using FFmpeg.AutoGen;

    internal unsafe class DoubleBufferedFrame : IDisposable, IFrame
    {
        private AVFrame* framePtr = ffmpeg.av_frame_alloc();
        private AVFrame* bufferedFramePtr = ffmpeg.av_frame_alloc();
        private bool bufferHasData;
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
                    nameof(DoubleBufferedFrame),
                    "The DoubleBufferedFrame has already been disposed.");
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
                    nameof(DoubleBufferedFrame),
                    "The DoubleBufferedFrame has already been disposed.");
            }

            return framePtr;
        }

        public void UseBufferedFrame()
        {
            if (!bufferHasData)
            {
                return;
            }

            var temp = framePtr;
            framePtr = bufferedFramePtr;
            bufferedFramePtr = temp;
            (HasData, bufferHasData) = (bufferHasData, HasData);
        }

        private void Unreference()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(
                    nameof(DoubleBufferedFrame),
                    "The AVFrameWrapper has already been disposed.");
            }

            if (!HasData)
            {
                return;
            }

            if (bufferHasData)
            {
                ffmpeg.av_frame_unref(bufferedFramePtr);
            }

            var temp = framePtr;
            framePtr = bufferedFramePtr;
            bufferedFramePtr = temp;
            HasData = false;
            bufferHasData = true;
        }

        private void DeleteFrames()
        {
            if (HasData)
            {
                ffmpeg.av_frame_unref(framePtr);
            }

            if (bufferHasData)
            {
                ffmpeg.av_frame_unref(bufferedFramePtr);
            }

            fixed (AVFrame** framePtrRef = &framePtr)
            {
                ffmpeg.av_frame_free(framePtrRef);
            }

            fixed (AVFrame** bufferedFramePtrRef = &bufferedFramePtr)
            {
                ffmpeg.av_frame_free(bufferedFramePtrRef);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                DeleteFrames();
                disposedValue = true;
            }
        }

        ~DoubleBufferedFrame()
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
