using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDedup.TimeSpanExtension
{
    static class TimeSpanExtension
    {
        private readonly static string TimeSpanLongFormat = @"dd\.hh\:mm\:ss";
        private readonly static string TimeSpanShortFormat = @"hh\:mm\:ss";

        public static string ToPrettyString(this TimeSpan ts)
        {
            var format = ts.Days >= 1 ? TimeSpanLongFormat : TimeSpanShortFormat;
            return ts.ToString(format);

        }
    }
}
