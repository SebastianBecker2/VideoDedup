namespace VideoDedupShared
{
    using NReco.VideoInfo;
    using System;

    public class VideoCodecInfo
    {
        public VideoCodecInfo() { }
        protected internal VideoCodecInfo(MediaInfo.StreamInfo videoStream) =>
            this.videoStream = videoStream ?? throw new ArgumentNullException(nameof(videoStream));

        public int Width
        {
            get
            {
                if (width != 0)
                {
                    return width;
                }
                if (videoStream != null)
                {
                    return videoStream.Width;
                }
                return 0;
            }
            set => width = value;
        }
        private int width = 0;

        public int Height
        {
            get
            {
                if (height != 0)
                {
                    return height;
                }
                if (videoStream != null)
                {
                    return videoStream.Height;
                }
                return 0;
            }
            set => height = value;
        }
        private int height = 0;

        public float FrameRate
        {
            get
            {
                if (frameRate != 0)
                {
                    return frameRate;
                }
                if (videoStream != null)
                {
                    return videoStream.FrameRate;
                }
                return 0;
            }
            set => frameRate = value;
        }
        private float frameRate = 0;

        public string CodecType
        {
            get
            {
                if (codecType != null)
                {
                    return codecType;
                }
                if (videoStream != null)
                {
                    return videoStream.CodecType;
                }
                return null;
            }
            set => codecType = value;
        }
        private string codecType = null;

        public string CodecLongName
        {
            get
            {
                if (codecLongName != null)
                {
                    return codecLongName;
                }
                if (videoStream != null)
                {
                    return videoStream.CodecLongName;
                }
                return null;
            }
            set => codecLongName = value;
        }
        private string codecLongName = null;

        public string CodecName
        {
            get
            {
                if (codecName != null)
                {
                    return codecName;
                }
                if (videoStream != null)
                {
                    return videoStream.CodecName;
                }
                return null;
            }
            set => codecName = value;
        }
        private string codecName = null;

        private readonly MediaInfo.StreamInfo videoStream;
    }
}
