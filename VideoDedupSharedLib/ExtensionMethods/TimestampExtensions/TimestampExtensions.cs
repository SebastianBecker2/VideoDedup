namespace VideoDedupSharedLib.ExtensionMethods.TimestampExtensions
{
    using System;
    using Google.Protobuf.WellKnownTypes;

    public static class TimestampExtensions
    {
        public static DateTime ToLocalDateTime(this Timestamp timestamp) =>
            timestamp.ToDateTime().ToLocalTime();
    }
}
