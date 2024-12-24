namespace FfmpegLib
{
    using FFmpeg.AutoGen;

    internal interface IFrame
    {
        long BestEffortTimestamp { get; }
        bool HasData { get; }
        AVPictureType PictType { get; }

        void Dispose();
        unsafe AVFrame* GetPointer();
        int ReceiveFrame(CodecContext codecContext);
        unsafe int ReceiveFrame(AVCodecContext* codecContext);
    }
}
