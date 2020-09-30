using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDedup.FilePreview
{
    public class ThumbnailLoadedEventArgs : EventArgs
    {
        public Image Thumbnail { get; set; }
        public int Index { get; set; }
    }
}
