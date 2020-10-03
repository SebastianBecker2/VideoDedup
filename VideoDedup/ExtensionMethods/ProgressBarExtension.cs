using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoDedup.ProgressBarExtension
{
    static class ProgressBarExtension
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern uint SendMessage(
            IntPtr hWnd,
            uint Msg,
            uint wParam,
            uint lParam);

        public static void Stop(this ProgressBar progressBar)
        {
            SendMessage(
                progressBar.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0003, //PBST_PAUSED
                0);
        }

        public static void ShowError(this ProgressBar progressBar)
        {
            SendMessage(
                progressBar.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0002, //PBST_ERROR
                0);
        }
    }
}
