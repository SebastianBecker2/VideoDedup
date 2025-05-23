namespace VideoDedupClient.Controls.FrameComparisonResultView
{
    using Size = Size;

    public class FrameComparisonResult : IDisposable
    {
        private bool disposedValue;

        public FrameComparisonResult(
            VideoDedupGrpc.FrameComparisonResult frameComparisonResult,
            Size containerSize)
        {
            InnerResult = frameComparisonResult;

            LeftFrames = new FrameSet(
                InnerResult.LeftFrames,
                containerSize);

            RightFrames = new FrameSet(
                InnerResult.RightFrames,
                containerSize);
        }

        private VideoDedupGrpc.FrameComparisonResult InnerResult { get; }
        public int Index => InnerResult.Index;
        public FrameSet LeftFrames { get; set; }
        public FrameSet RightFrames { get; set; }
        public double Difference => InnerResult.Difference;
        public int LoadLevel => InnerResult.LoadLevel;
        public VideoDedupGrpc.ComparisonResult ComparisonResult =>
            InnerResult.ComparisonResult;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    LeftFrames.Dispose();
                    RightFrames.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
