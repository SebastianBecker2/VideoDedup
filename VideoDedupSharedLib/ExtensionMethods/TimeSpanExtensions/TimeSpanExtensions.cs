namespace VideoDedupSharedLib.ExtensionMethods.TimeSpanExtensions
{
    using System;
    using System.Globalization;

    public static class TimeSpanExtensions
    {
        private static readonly string TimeSpanLongFormat = @"dd\.hh\:mm\:ss";
        private static readonly string TimeSpanShortFormat = @"hh\:mm\:ss";

        public static string ToPrettyString(this TimeSpan ts)
        {
            var format = ts.Days >= 1 ? TimeSpanLongFormat : TimeSpanShortFormat;
            return ts.ToString(format, CultureInfo.InvariantCulture);
        }

        public static TimeSpan Multiply(this TimeSpan ts, double multiplier) =>
            TimeSpan.FromTicks((long)(ts.Ticks * multiplier));
    }
}
