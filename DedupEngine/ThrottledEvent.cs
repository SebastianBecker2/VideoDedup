namespace DedupEngine
{
    internal abstract class ThrottledEvent
    {
        private long previousTicks = DateTime.UtcNow.Ticks;

        public TimeSpan Time { get; set; } = TimeSpan.FromMilliseconds(100);

        protected bool Execute()
        {
            var prevCounter = Interlocked.Read(ref previousTicks);
            var nowTicks = DateTime.UtcNow.Ticks;

            return ((nowTicks - prevCounter) >= Time.Ticks)
                && Interlocked.CompareExchange(
                    ref previousTicks, nowTicks, prevCounter)
                    == prevCounter;
        }
    }
}
