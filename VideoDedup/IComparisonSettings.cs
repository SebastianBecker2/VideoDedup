using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDedup
{
    public interface IComparisonSettings
    {
        int MaxThumbnailComparison { get; }
        int MaxDifferentThumbnails { get; }
        int MaxDifferencePercentage { get; }
        int MaxDurationDifferenceSeconds { get; }
        int MaxDurationDifferencePercent { get; }
        DurationDifferenceType DurationDifferenceType { get; }
    }
}
