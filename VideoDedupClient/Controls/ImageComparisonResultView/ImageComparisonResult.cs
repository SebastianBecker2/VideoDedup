namespace VideoDedupClient.Controls.ImageComparisonResultView
{
    using Size = Size;

    public class ImageComparisonResult : IDisposable
    {
        private bool disposedValue;

        public ImageComparisonResult(
            VideoDedupGrpc.ImageComparisonResult imageComparisonResult,
            Size containerSize)
        {
            InnerResult = imageComparisonResult;

            LeftImages = new ImageSet(
                InnerResult.LeftImages,
                containerSize);

            RightImages = new ImageSet(
                InnerResult.RightImages,
                containerSize);
        }

        private VideoDedupGrpc.ImageComparisonResult InnerResult { get; }
        public int Index => InnerResult.Index;
        public ImageSet LeftImages { get; set; }
        public ImageSet RightImages { get; set; }
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
                    LeftImages.Dispose();
                    RightImages.Dispose();
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
