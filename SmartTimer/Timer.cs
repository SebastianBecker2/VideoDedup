namespace SmartTimer
{
    public class Timer : MarshalByRefObject, IDisposable
    {
        private readonly System.Threading.Timer timer;
        private bool disposedValue;

        public bool IsRunning { get; protected set; }
        public bool IsPeriodic { get; protected set; }

        public Timer(
            TimerCallback callback,
            object? state,
            long dueTime,
            long period)
        {
            timer = new System.Threading.Timer(
                param =>
                {
                    IsRunning = IsPeriodic;
                    callback(param);
                },
                state,
                dueTime,
                period);

            lock (timer)
            {
                if (dueTime != Timeout.Infinite)
                {
                    IsRunning = true;
                }
                if (period != Timeout.Infinite)
                {
                    IsPeriodic = true;
                }
            }
        }

        public Timer(
            TimerCallback callback,
            object? state,
            int dueTime,
            int period)
            : this(callback, state, dueTime, (long)period)
        {
        }

        public Timer(
            TimerCallback callback,
            object? state,
            TimeSpan dueTime,
            TimeSpan period)
            : this(
                callback,
                state,
                (long)dueTime.TotalMilliseconds,
                (long)period.TotalMilliseconds)
        { }

        public Timer(
            TimerCallback callback,
            object? state,
            uint dueTime,
            uint period)
            : this(callback, state, dueTime, (long)period) { }

        public Timer(TimerCallback callback)
            : this(
                callback,
                new object(),
                Timeout.Infinite,
                (long)Timeout.Infinite)
        { }

        public Timer(TimerCallback callback, object? state)
            : this(callback, state, Timeout.Infinite, (long)Timeout.Infinite) { }

        public Timer(TimerCallback callback, object? state, long period)
            : this(callback, state, period, period) { }

        public Timer(TimerCallback callback, object? state, int period)
            : this(callback, state, period, period) { }

        public Timer(TimerCallback callback, object? state, TimeSpan period)
            : this(callback, state, period, period) { }

        public Timer(TimerCallback callback, object? state, uint period)
            : this(callback, state, period, period) { }

        public bool Change(long dueTime, long period)
        {
            bool result;
            try
            {
                lock (timer)
                {
                    result = timer.Change(dueTime, period);
                    IsRunning = result && dueTime != Timeout.Infinite;
                    IsPeriodic = result && period != Timeout.Infinite;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool Change(int dueTime, int period) =>
            Change(dueTime, (long)period);

        public bool Change(TimeSpan dueTime, TimeSpan period) =>
            Change((long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds);

        public bool Change(uint dueTime, uint period) =>
            Change(dueTime, (long)period);

        public bool StartSingle(long dueTime) =>
            Change(dueTime, Timeout.Infinite);

        public bool StartSingle(int dueTime) =>
            StartSingle((long)dueTime);

        public bool StartSingle(uint dueTime) =>
            StartSingle((long)dueTime);

        public bool StartSingle(TimeSpan dueTime) =>
            StartSingle((long)dueTime.TotalMilliseconds);

        public bool StartRecurring(long interval) =>
            Change(interval, interval);

        public bool StartRecurring(int interval) =>
            StartRecurring((long)interval);

        public bool StartRecurring(uint interval) =>
            StartRecurring((long)interval);

        public bool StartRecurring(TimeSpan interval) =>
            StartRecurring((long)interval.TotalMilliseconds);

        public bool Stop() => Change(Timeout.Infinite, Timeout.Infinite);

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }

            if (disposing)
            {
                timer.Dispose();
            }

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
