namespace VideoDedupShared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImageIndex : IComparable<ImageIndex>, IEquatable<ImageIndex>
    {
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        public double Quotient => Numerator / (double)Denominator;

        public override string ToString() =>
            Numerator.ToString() + "/" + Denominator.ToString();

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

        public int CompareTo(ImageIndex other)
        {
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

        public override bool Equals(object obj) =>
            Equals(obj as ImageIndex);

        public bool Equals(ImageIndex other) =>
            other != null
            && Numerator == other.Numerator
            && Denominator == other.Denominator;

        public override int GetHashCode()
        {
            var hashCode = -1534900553;
            hashCode = (hashCode * -1521134295) + Numerator.GetHashCode();
            hashCode = (hashCode * -1521134295) + Denominator.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ImageIndex left, ImageIndex right) =>
            EqualityComparer<ImageIndex>.Default.Equals(left, right);

        public static bool operator !=(ImageIndex left, ImageIndex right) =>
            !(left == right);
    }
}
