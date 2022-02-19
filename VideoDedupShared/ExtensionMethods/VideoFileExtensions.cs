namespace VideoDedupShared.ExtensionMethods
{
    using System;

    public static class VideoFileExtensions
    {
        public static string GetInfoText(this VideoFile videoFile)
        {
            var fileSize = videoFile.FileSize;
            var duration = videoFile.Duration;
            var codecInfo = videoFile.CodecInfo;
            var durationFormat = duration.Hours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";

            var infoText = videoFile.FilePath + Environment.NewLine +
                (fileSize / (1024 * 1024)).ToString() + " MB" +
                Environment.NewLine + duration.ToString(durationFormat);

            if (codecInfo != null)
            {
                infoText += Environment.NewLine +
                    codecInfo.Size.Width.ToString() + " x " +
                    codecInfo.Size.Height.ToString() + " @ " +
                    codecInfo.FrameRate + " Frames" + Environment.NewLine +
                    codecInfo.Name;
            }

            return infoText;
        }
    }
}
