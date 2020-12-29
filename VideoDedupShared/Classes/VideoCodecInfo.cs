using NReco.VideoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDedupShared
{
    public class VideoCodecInfo
    {
        public int Width { get => videoStream.Width; }
        public int Height { get => videoStream.Height; }
        public float FrameRate { get => videoStream.FrameRate; }
        public string CodecType { get => videoStream.CodecType; }
        public string CodecLongName { get => videoStream.CodecLongName; }
        public string CodecName { get => videoStream.CodecName; }

        private readonly MediaInfo.StreamInfo videoStream;

        internal VideoCodecInfo(MediaInfo.StreamInfo videoStream)
        {
            this.videoStream = videoStream ?? throw new ArgumentNullException(nameof(videoStream));
        }
    }
}
