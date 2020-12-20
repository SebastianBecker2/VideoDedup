namespace VideoDedup.ProgressBarExtension
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal static class ProgressBarExtension
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SendMessage(
            IntPtr hWnd,
            uint msg,
            uint wParam,
            uint lParam);

        public static void Stop(this ProgressBar progressBar) => _ = SendMessage(
                progressBar.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0003, //PBST_PAUSED
                0);

        public static void ShowError(this ProgressBar progressBar) => _ = SendMessage(
                progressBar.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0002, //PBST_ERROR
                0);
    }
}
