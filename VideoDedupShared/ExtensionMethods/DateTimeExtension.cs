namespace VideoDedupShared.DateTimeExtension
{
    using System;

    public static class DateTimeExtension
    {
        public static long ToUnixDate(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)Math.Floor((date.ToUniversalTime() - epoch).TotalSeconds);
        }
    }
}
