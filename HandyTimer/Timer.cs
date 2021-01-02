namespace SmartTimer
{
	[System.Runtime.InteropServices.ComVisible(true)]
	[System.Security.Permissions.HostProtection(System.Security.Permissions.SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public class Timer : System.MarshalByRefObject, System.IDisposable
	{
		private System.Threading.Timer _timer;
		public bool IsRunning { get; protected set; }
		public bool IsPeriodic { get; protected set; }

		protected Timer()
		{
			IsRunning = false;
			IsPeriodic = false;
		}

		public Timer(System.Threading.TimerCallback callback, object state, long dueTime, long period)
			: this()
		{
			_timer = new System.Threading.Timer((param) =>
			{
				IsRunning = IsPeriodic;
				callback(param);
			}, state, dueTime, period);
			lock (_timer)
			{
				if (dueTime != System.Threading.Timeout.Infinite)
				{
					IsRunning = true;
				}
				if (period != System.Threading.Timeout.Infinite)
				{
					IsPeriodic = true;
				}
			}
		}

		public Timer(System.Threading.TimerCallback callback, object state, int dueTime, int period)
			: this(callback, state, dueTime, (long)period)
		{
		}

		public Timer(System.Threading.TimerCallback callback, object state, System.TimeSpan dueTime, System.TimeSpan period)
			: this(callback, state, (long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds)
		{
		}

		public Timer(System.Threading.TimerCallback callback, object state, uint dueTime, uint period)
			: this(callback, state, dueTime, (long)period)
		{
		}

		public Timer(System.Threading.TimerCallback callback)
			: this(callback, new object(), System.Threading.Timeout.Infinite, (long)System.Threading.Timeout.Infinite)
		{
		}

		public Timer(System.Threading.TimerCallback callback, object state)
			: this(callback, state, System.Threading.Timeout.Infinite, (long)System.Threading.Timeout.Infinite)
		{
		}

		public Timer(System.Threading.TimerCallback callback, object state, long period)
			: this(callback, state, period, period)
		{
		}

		public Timer(System.Threading.TimerCallback callback, object state, int period)
			: this(callback, state, period, period)
		{
		}

		public Timer(System.Threading.TimerCallback callback, object state, System.TimeSpan period)
			: this(callback, state, period, period)
		{
		}

		public Timer(System.Threading.TimerCallback callback, object state, uint period)
			: this(callback, state, period, period)
		{
		}

		public bool Change(long dueTime, long period)
		{
			bool result;
			try
			{
				lock (_timer)
				{
					result = _timer.Change(dueTime, period);
					IsRunning = (result && dueTime != System.Threading.Timeout.Infinite);
					IsPeriodic = (result && period != System.Threading.Timeout.Infinite);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public bool Change(int dueTime, int period)
		{
			return this.Change(dueTime, (long)period);
		}

		public bool Change(System.TimeSpan dueTime, System.TimeSpan period)
		{
			return this.Change((long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds);
		}

		public bool Change(uint dueTime, uint period)
		{
			return this.Change(dueTime, (long)period);
		}

		public bool StartSingle(long dueTime)
		{
			return this.Change(dueTime, System.Threading.Timeout.Infinite);
		}

		public bool StartSingle(int dueTime)
		{
			return this.StartSingle((long)dueTime);
		}

		public bool StartSingle(uint dueTime)
		{
			return this.StartSingle((long)dueTime);
		}

		public bool StartSingle(System.TimeSpan dueTime)
		{
			return this.StartSingle((long)dueTime.TotalMilliseconds);
		}

		public bool StartRecurring(long interval)
		{
			return this.Change(interval, interval);
		}

		public bool StartRecurring(int interval)
		{
			return this.StartRecurring((long)interval);
		}

		public bool StartRecurring(uint interval)
		{
			return this.StartRecurring((long)interval);
		}

		public bool StartRecurring(System.TimeSpan interval)
		{
			return this.StartRecurring((long)interval.TotalMilliseconds);
		}

		public bool Stop()
		{
			return this.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
		}

		#region IDisposable Members

		public void Dispose()
		{
			_timer.Dispose();
		}

		public void Dispose(System.Threading.WaitHandle notifyObject)
		{
			_timer.Dispose(notifyObject);
		}

		#endregion
	}
}