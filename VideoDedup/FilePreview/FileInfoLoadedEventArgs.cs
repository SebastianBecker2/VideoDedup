using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDedup.FilePreview
{
    public class FileInfoLoadedEventArgs : EventArgs
    {
        public NReco.VideoInfo.MediaInfo FileInfo { get; set; }
        public long FileSize { get; set; }
    }
}
