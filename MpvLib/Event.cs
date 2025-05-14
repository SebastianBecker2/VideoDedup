namespace MpvLib
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class Event
    {
        public int Id { get; set; }
        public int Error { get; set; }
        public ulong UserData { get; set; }
        public IntPtr Data { get; set; }
    }
}
