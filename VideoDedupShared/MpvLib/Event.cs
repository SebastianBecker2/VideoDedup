namespace VideoDedupShared.MpvLib
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class Event
    {
        public int Id;
        public int Error;
        public ulong UserData;
        public IntPtr Data;
    }
}
