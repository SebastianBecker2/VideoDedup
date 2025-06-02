namespace FfmpegLib
{
    using System.Diagnostics;

    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class FrameIndex(int numerator, int denominator)
        : IEquatable<FrameIndex>
    {
        public int Numerator { get; } = numerator;
        public int Denominator { get; } = denominator;

        public double Quotient => Numerator / (double)Denominator;

        public string ToPrettyString() =>
            $"{Numerator}/{Denominator}={Quotient}";

        private string GetDebuggerDisplay() => ToPrettyString();

        public static implicit operator VideoDedupGrpc.FrameIndex(
            FrameIndex index) =>
            new()
            {
                Denominator = index.Denominator,
                Numerator = index.Numerator
            };

        public static implicit operator FrameIndex(
            VideoDedupGrpc.FrameIndex index) =>
            new(index.Numerator, index.Denominator);

        public static FrameIndex CreateFrameIndex(int index, int count)
        {
            var gcd = CalculateGcd(index, count + 1);
            return new FrameIndex(index / gcd, (count + 1) / gcd);
        }

        public static IEnumerable<FrameIndex> CreateFrameIndices(
            int frameCount) =>
            Enumerable
                .Range(1, frameCount)
                .Select(i => CreateFrameIndex(i, frameCount));

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

        public override string ToString() => ToPrettyString();

        public bool Equals(FrameIndex? other) =>
            other is not null
            && other.Numerator == Numerator
            && other.Denominator == Denominator;

        public override bool Equals(object? obj) => Equals(obj as FrameIndex);

        public override int GetHashCode() =>
            HashCode.Combine(Numerator, Denominator);

        public static bool operator ==(FrameIndex left, FrameIndex right) =>
            EqualityComparer<FrameIndex>.Default.Equals(left, right);

        public static bool operator !=(FrameIndex left, FrameIndex right) =>
            !(left == right);
    }
}
