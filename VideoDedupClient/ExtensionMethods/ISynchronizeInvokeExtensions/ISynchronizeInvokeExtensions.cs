namespace VideoDedupClient.ExtensionMethods.ISynchronizeInvokeExtensions
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
                    _ = invokableControl.BeginInvoke(action, []);
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
