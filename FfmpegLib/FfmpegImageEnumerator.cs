namespace FfmpegLib
{
    using System.Collections;
    using System.Runtime.InteropServices;
    using FFmpeg.AutoGen;
    using FfmpegLib.Exceptions;

    public sealed class FfmpegImageEnumerator(
        string filePath,
        CancellationToken? cancelToken,
        int index,
        int count,
        int divisionCount
    ) : IEnumerable<byte[]?>, IEnumerator<byte[]?>
    {
        private static readonly int DecodeRetryCount = 5;

        private double stepping;
        private nint formatContext;
        private nint streamContext;
        private nint jpegContext;
        private int streamIndex;

        public byte[]? Current { get; private set; }

        object IEnumerator.Current => Current!;

        public IEnumerator<byte[]?> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public unsafe bool MoveNext()
        {
            if (count <= 0 || cancelToken?.IsCancellationRequested == true)
            {
                return false;
            }

            InitializeFfmpegResources();

            try
            {
                // Extract image at the calculated timestamp.
                var timestamp = stepping * (index + 1);

                var fContext = (AVFormatContext*)formatContext;
                var stream = fContext->streams[streamIndex];
                timestamp = ffmpeg.av_rescale(
                    (long)timestamp,
                    stream->time_base.den,
                    stream->time_base.num);

                //Console.WriteLine($"Index {index}: {timestamp}");
                Current = ExtractImageAtTimestamp((long)timestamp);

                index++;
                count--;
                return true;
            }
            catch
            {
                Current = null;
                return false;
            }
        }

        public void Reset() =>
            throw new NotSupportedException("Reset is not supported.");

        public void Dispose() => ReleaseFfmpegResources();

        private static double CalculateStepping(
            double duration,
            int divisionCount) =>
            Math.Max(duration / (divisionCount + 1), 1);

        private unsafe byte[]? ExtractImageAtTimestamp(
            long timestamp)
        {
            var fContext = (AVFormatContext*)formatContext;
            if (ffmpeg.av_seek_frame(
                fContext,
                streamIndex,
                timestamp,
                ffmpeg.AVSEEK_FLAG_BACKWARD | ffmpeg.AVSEEK_FLAG_FRAME)
                < 0)
            {
                return null;
            }

            var sContext = (AVCodecContext*)streamContext;

            var frame = ffmpeg.av_frame_alloc();
            var packet = ffmpeg.av_packet_alloc();
            var jpegPacket = ffmpeg.av_packet_alloc();

            try
            {
                if (!GetFrame(packet, frame, timestamp))
                {
                    throw new FfmpegOperationException(
                        "Unable to extract images. Unable to decode video.",
                        filePath);
                }

                ReinitializeJpegContext();
                var jContext = (AVCodecContext*)jpegContext;

                var result = ffmpeg.avcodec_send_frame(jContext, frame);
                if (result != 0)
                {
                    var errorBuffer = new byte[128];
                    fixed (byte* errorPtr = errorBuffer)
                    {
                        ffmpeg.av_strerror(result, errorPtr, (ulong)errorBuffer.Length);
                        var errorMessage = System.Text.Encoding.UTF8.GetString(errorBuffer);
                        throw new FfmpegOperationException(
                            "Unable to extract images. Unable to decode video.",
                            filePath);
                    }
                }

                if (ffmpeg.avcodec_receive_packet(jContext, jpegPacket) != 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract images. Unable to decode video.",
                        filePath);
                }

                var jpegData = new byte[jpegPacket->size];
                Marshal.Copy((IntPtr)jpegPacket->data, jpegData, 0, jpegPacket->size);

                return jpegData;
            }
            finally
            {
                ffmpeg.av_frame_free(&frame);
                ffmpeg.av_packet_unref(packet);
                ffmpeg.av_packet_free(&packet);
                ffmpeg.av_packet_unref(jpegPacket);
                ffmpeg.av_packet_free(&jpegPacket);
            }

        }

        private unsafe bool GetFrame(
            AVPacket* packet,
            AVFrame* frame,
            long timestamp)
        {
            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (frame is null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            var fContext = (AVFormatContext*)formatContext;
            var sContext = (AVCodecContext*)streamContext;
            ffmpeg.avcodec_flush_buffers(sContext);

            //var hasKeyFrame = false;
            while (ffmpeg.av_read_frame(fContext, packet) >= 0)
            {
                if (packet->stream_index != streamIndex)
                {
                    ffmpeg.av_packet_unref(packet);
                    continue;
                }

                //hasKeyFrame |= (packet->flags & ffmpeg.AV_PKT_FLAG_KEY) != 0;
                //if (!hasKeyFrame)
                //{
                //    ffmpeg.av_packet_unref(packet);
                //    continue;
                //}

                foreach (var _ in Enumerable.Range(0, DecodeRetryCount))
                {
                    if (ffmpeg.avcodec_send_packet(sContext, packet) != 0)
                    {
                        return false;
                    }

                    var result = ffmpeg.avcodec_receive_frame(sContext, frame);
                    if (result == ffmpeg.AVERROR_EOF)
                    {
                        ffmpeg.av_packet_unref(packet);
                        return false;
                    }
                    if (result == ffmpeg.AVERROR(ffmpeg.EAGAIN))
                    {
                        break;
                    }
                    if (result < 0)
                    {
                        ffmpeg.av_packet_unref(packet);
                        return false;
                    }

                    if (frame->best_effort_timestamp < timestamp)
                    {
                        ffmpeg.av_packet_unref(packet);
                        ffmpeg.av_frame_unref(frame);
                        break;
                    }
                    return true;
                }
            }
            return false;
        }

        private unsafe AVFormatContext* AllocateFormatContext()
        {
            var fContext = ffmpeg.avformat_alloc_context();
            formatContext = (nint)fContext;

            if (ffmpeg.avformat_open_input(&fContext, filePath, null, null) != 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Unable to open file.",
                    filePath);
            }
            return fContext;
        }

        private unsafe AVStream* FindStream()
        {
            var fContext = (AVFormatContext*)formatContext;

            if (ffmpeg.avformat_find_stream_info(fContext, null) < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Video stream not found.",
                    filePath);
            }

            AVStream* stream = null;
            for (var index = 0;
                index < fContext->nb_streams;
                index++)
            {
                if (stream is null
                    && fContext->streams[index]->codecpar->codec_type
                    == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    stream = fContext->streams[index];
                    streamIndex = index;
                    continue;
                }
                fContext->streams[index]->discard =
                    AVDiscard.AVDISCARD_ALL;
            }

            if (stream is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Video stream not found.",
                    filePath);
            }

            return stream;
        }

        private unsafe AVCodec* GetDecoder(AVStream* stream)
        {
            var codec = ffmpeg.avcodec_find_decoder(stream->codecpar->codec_id);
            if (codec is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Video codec not found.",
                    filePath);
            }
            return codec;
        }

        private unsafe AVCodecContext* AllocateStreamContext(
            AVStream* stream,
            AVCodec* codec)
        {
            var sContext = ffmpeg.avcodec_alloc_context3(codec);
            streamContext = (nint)sContext;
            if (ffmpeg.avcodec_parameters_to_context(
                    sContext,
                    stream->codecpar)
                < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images." +
                    "Unable to allocate video codec context.",
                    filePath);
            }

            if (ffmpeg.avcodec_open2(sContext, codec, null) < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Could not open video codec.",
                    filePath);
            }

            return sContext;
        }

        private unsafe void InitializeFfmpegResources()
        {
            if (formatContext != nint.Zero)
            {
                return;
            }

            ffmpeg.avformat_network_init();

            var fContext = AllocateFormatContext();
            var stream = FindStream();
            var codec = GetDecoder(stream);
            var sContext = AllocateStreamContext(stream, codec);
            sContext->skip_frame = AVDiscard.AVDISCARD_NONREF;

            // Check thread_count???? Multithreading?
            sContext->thread_count = 32;
            sContext->thread_type = ffmpeg.FF_THREAD_FRAME; /*ffmpeg.FF_THREAD_SLICE;*/
            // Check gpu acceleration?
            // Cache decoded frames for small files?


            if (fContext->duration <= 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Duration not available.",
                    filePath);
            }
            var duration = fContext->duration / (double)ffmpeg.AV_TIME_BASE;
            stepping = CalculateStepping(duration, divisionCount);
            //Console.WriteLine($"Duration: {fContext->duration}");
            //Console.WriteLine($"Duration in seconds: {duration}");
            //Console.WriteLine($"Stepping: {stepping}");
            //Console.WriteLine($"steam context TimeBase num: {sContext->time_base.num} / den: {sContext->time_base.den}");
            //Console.WriteLine($"format TimeBase num: {fContext->time_base}")
            //Console.WriteLine($"stream frame rate num: {stream->avg_frame_rate.num} / den: {stream->avg_frame_rate.den}");
            //Console.WriteLine($"stream TimeBase num: {stream->time_base.num} / den: {stream->time_base.den}");
        }

        // We need to get non-iframes too.
        // Key-frames isn't enough for short videos...
        // Maybe depending on the length of the video?
        // Or on the iframe count?

        private unsafe void ReinitializeJpegContext()
        {
            // Free the current JPEG context if it exists
            if (jpegContext != nint.Zero)
            {
                var oldJpegContext = (AVCodecContext*)jpegContext;
                ffmpeg.avcodec_free_context(&oldJpegContext);
                jpegContext = nint.Zero;
            }

            // Reinitialize the JPEG codec context
            var jpegCodec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MJPEG);
            if (jpegCodec == null)
            {
                throw new FfmpegOperationException("Jpeg codec not found.", filePath);
            }

            var newJpegContext = ffmpeg.avcodec_alloc_context3(jpegCodec);
            if (newJpegContext == null)
            {
                throw new FfmpegOperationException("Unable to allocate JPEG codec.", filePath);
            }

            // Configure the new context
            newJpegContext->width = ((AVCodecContext*)streamContext)->width;
            newJpegContext->height = ((AVCodecContext*)streamContext)->height;
            newJpegContext->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUVJ420P;
            newJpegContext->global_quality = 12;
            newJpegContext->time_base = new AVRational { num = 1, den = 25 };
            newJpegContext->bit_rate = 400000;

            if (ffmpeg.avcodec_open2(newJpegContext, jpegCodec, null) < 0)
            {
                ffmpeg.avcodec_free_context(&newJpegContext);
                throw new FfmpegOperationException("Could not open JPEG codec.", filePath);
            }

            // Assign the new context
            jpegContext = (nint)newJpegContext;
        }

        private unsafe void ReleaseFfmpegResources()
        {
            var jContext = (AVCodecContext*)jpegContext;
            ffmpeg.avcodec_free_context(&jContext);

            var sContext = (AVCodecContext*)streamContext;
            ffmpeg.avcodec_free_context(&sContext);

            var fContext = (AVFormatContext*)formatContext;
            ffmpeg.avformat_close_input(&fContext);
            ffmpeg.avformat_free_context(fContext);
        }
    }
}
