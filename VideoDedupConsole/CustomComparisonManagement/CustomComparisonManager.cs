namespace VideoDedupConsole.CustomComparisonManagement
{
    using System;
    using System.Collections.Generic;
    using Wcf.Contracts.Data;

    internal class CustomComparisonManager
    {
        private IDictionary<Guid, CustomComparison> CustomComparisons { get; set; }
            = new Dictionary<Guid, CustomComparison>();

        public CustomVideoComparisonStartData StartCustomComparison(
            CustomVideoComparisonData customVideoComparisonData)
        {
            try
            {
                var customComparison = new CustomComparison(
                    customVideoComparisonData);

                CustomComparisons.Add(customComparison.Token, customComparison);

                return new CustomVideoComparisonStartData
                {
                    ComparisonToken = customComparison.Token,
                };
            }
            catch (Exception exc)
            {
                return new CustomVideoComparisonStartData
                {
                    ErrorMessage = exc.Message,
                };
            }
        }

        public CustomVideoComparisonStatusData GetStatus(
            Guid token,
            int imageComparisonIndex = 0)
        {
            if (!CustomComparisons.TryGetValue(token, out var customComparison))
            {
                return null;
            }

            return customComparison.GetStatus(imageComparisonIndex);
        }

        public bool CancelCustomComparison(Guid token)
        {
            if (!CustomComparisons.TryGetValue(token, out var customComparison))
            {
                return false;
            }

            customComparison.CancelComparison();

            return CustomComparisons.Remove(token);
        }
    }
}
