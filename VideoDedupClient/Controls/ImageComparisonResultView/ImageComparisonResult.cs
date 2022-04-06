namespace VideoDedupClient.Controls.ImageComparisonResultView
{
    using Size = Size;

    public class ImageComparisonResult
    {
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

    }
}
