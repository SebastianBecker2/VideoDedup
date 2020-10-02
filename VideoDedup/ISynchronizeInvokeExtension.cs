using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoDedup
{
	static class ISynchronizeInvokeExtensions
	{
		public static void InvokeIfRequired(this ISynchronizeInvoke @object, MethodInvoker action)
		{
			if (@object.InvokeRequired)
			{
				try
				{
					@object.Invoke(action, new object[0]);
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
