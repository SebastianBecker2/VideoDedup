namespace VideoDedupSharedLib.ExtensionMethods.ISynchronizeInvokeExtensions
{
    using System.ComponentModel;
    using System.Windows.Forms;

    public static class ISynchronizeInvokeExtensions
    {
        public static void InvokeIfRequired(
            this ISynchronizeInvoke invokableControl,
            MethodInvoker action)
        {
            if (invokableControl.InvokeRequired)
            {
                try
                {
                    _ = invokableControl.BeginInvoke(
                        action,
                        Array.Empty<object>());
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
