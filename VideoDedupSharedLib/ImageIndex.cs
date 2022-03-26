namespace VideoDedupGrpc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed partial class ImageIndex :
        IComparable<ImageIndex>
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

        public int CompareTo(ImageIndex? other)
        {
            if (other is null)
            {
                return 1;
            }

            if (Denominator < other.Denominator)
            {
                return -1;
            }

            if (Denominator > other.Denominator)
            {
                return 1;
            }

            if (Numerator < other.Numerator)
            {
                return -1;
            }

            if (Numerator > other.Numerator)
            {
                return 1;
            }

            return 0;
        }

        public static bool operator ==(ImageIndex left, ImageIndex right) =>
            left.Equals(right);

        public static bool operator !=(ImageIndex left, ImageIndex right) =>
            !(left == right);

        public static bool operator <(ImageIndex left, ImageIndex right) =>
            left.CompareTo(right) < 0;

        public static bool operator <=(ImageIndex left, ImageIndex right) =>
             left.CompareTo(right) <= 0;

        public static bool operator >(ImageIndex left, ImageIndex right) =>
             left.CompareTo(right) > 0;

        public static bool operator >=(ImageIndex left, ImageIndex right) =>
             left.CompareTo(right) >= 0;
    }
}
