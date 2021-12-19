namespace VideoDedupShared.TimeSpanExtension
{
    using System;

    public static class TimeSpanExtension
    {
        private static readonly string TimeSpanLongFormat = @"dd\.hh\:mm\:ss";
        private static readonly string TimeSpanShortFormat = @"hh\:mm\:ss";

        public static string ToPrettyString(this TimeSpan ts)
        {
            var format = ts.Days >= 1 ? TimeSpanLongFormat : TimeSpanShortFormat;
            return ts.ToString(format);
        }

        public static TimeSpan Multiply(this TimeSpan ts, double multiplier) =>
            TimeSpan.FromTicks((long)(ts.Ticks * multiplier));
    }
}
