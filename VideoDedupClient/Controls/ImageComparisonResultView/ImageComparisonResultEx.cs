namespace VideoDedup.ImageComparisonResultView
{
    using System.Drawing;
    using VideoDedupShared;

    public class ImageComparisonResultEx
    {
        public ImageComparisonResultEx(
            ImageComparisonResult imageComparisonResult,
            Size containerSize)
        {
            ImageComparisonResult = imageComparisonResult;

            LeftImages = new ImageSetEx(
                ImageComparisonResult.LeftImages,
                containerSize);

            RightImages = new ImageSetEx(
                ImageComparisonResult.RightImages,
                containerSize);
        }

        public ImageComparisonResult ImageComparisonResult { get; set; }
        public int Index => ImageComparisonResult.Index;
        public ImageSetEx LeftImages { get; set; }
        public ImageSetEx RightImages { get; set; }
        public double Difference => ImageComparisonResult.Difference;
        public int LoadLevel => ImageComparisonResult.LoadLevel;
        public ComparisonResult ComparisonResult =>
            ImageComparisonResult.ComparisonResult;

    }
}
