namespace VideoDedupShared.ISynchronizeInvokeExtensions
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    public static class ISynchronizeInvokeExtensions
    {
        public static void InvokeIfRequired(this ISynchronizeInvoke @object, MethodInvoker action)
        {
            if (@object.InvokeRequired)
            {
                try
                {
                    _ = @object.BeginInvoke(action, new object[0]);
                }
                catch (ObjectDisposedException) { }
            }
            else
            {
                action();
            }
        }
    }
}
