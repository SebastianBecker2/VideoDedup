namespace VideoDedupGrpc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed partial class ImageIndex
    {
        public double Quotient => Numerator / (double)Denominator;

        public string ToPrettyString() =>
            $"{Numerator}/{Denominator}";

        public static ImageIndex CreateImageIndex(int index, int count)
        {
            var gcd = CalculateGcd(index, count + 1);
            return new ImageIndex
            {
                Numerator = index / gcd,
                Denominator = (count + 1) / gcd,
            };
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
    }
}
