namespace VideoDedup.ISynchronizeInvokeExtensions
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    internal static class ISynchronizeInvokeExtensions
    {
        public static void InvokeIfRequired(this ISynchronizeInvoke @object, MethodInvoker action)
        {
            if (@object.InvokeRequired)
            {
                try
                {
                    _ = @object.Invoke(action, new object[0]);
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
