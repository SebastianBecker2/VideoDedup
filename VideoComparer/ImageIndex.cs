namespace VideoComparer
{
    using System.Diagnostics;

    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class ImageIndex : IEquatable<ImageIndex>
    {
        public int Numerator { get; }
        public int Denominator { get; }

        public double Quotient => Numerator / (double)Denominator;

        public string ToPrettyString() =>
            $"{Numerator}/{Denominator}";

        private string GetDebuggerDisplay() => ToPrettyString();

        public ImageIndex(int numerator, int denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public static implicit operator VideoDedupGrpc.ImageIndex(
            ImageIndex index) =>
            new()
            {
                Denominator = index.Denominator,
                Numerator = index.Numerator
            };

        public static implicit operator ImageIndex(
            VideoDedupGrpc.ImageIndex index) =>
            new(index.Numerator, index.Denominator);

        public static ImageIndex CreateImageIndex(int index, int count)
        {
            var gcd = CalculateGcd(index, count + 1);
            return new ImageIndex(index / gcd, (count + 1) / gcd);
        }

        public static IEnumerable<ImageIndex> CreateImageIndices(
            int imageCount) =>
            Enumerable
                .Range(1, imageCount)
                .Select(i => CreateImageIndex(i, imageCount));

        // Calculate greatest common divisor (GCD)
        private static int CalculateGcd(int a, int b)
        {
            if (a < b)
            {
                return CalculateGcd(b, a);
            }

            if (b == 0)
            {
                return a;
            }

            return CalculateGcd(b, a % b);
        }

        public bool Equals(ImageIndex? other) =>
            other is not null
            && other.Numerator == Numerator
            && other.Denominator == Denominator;

        public override bool Equals(object? obj) => Equals(obj as ImageIndex);

        public override int GetHashCode() =>
            HashCode.Combine(Numerator, Denominator);

        public static bool operator ==(ImageIndex left, ImageIndex right) =>
            EqualityComparer<ImageIndex>.Default.Equals(left, right);

        public static bool operator !=(ImageIndex left, ImageIndex right) =>
            !(left == right);
    }
}
