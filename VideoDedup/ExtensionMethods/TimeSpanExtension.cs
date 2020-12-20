namespace VideoDedup.TimeSpanExtension
{
    using System;

    internal static class TimeSpanExtension
    {
        private static readonly string TimeSpanLongFormat = @"dd\.hh\:mm\:ss";
        private static readonly string TimeSpanShortFormat = @"hh\:mm\:ss";

        public static string ToPrettyString(this TimeSpan ts)
        {
            var format = ts.Days >= 1 ? TimeSpanLongFormat : TimeSpanShortFormat;
            return ts.ToString(format);

        }
    }
}
