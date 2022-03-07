namespace VideoDedupShared
{
    using System;

    public class OperationInfo
    {
        public OperationType OperationType { get; set; }
        public int CurrentProgress { get; set; } = 0;
        public int MaximumProgress { get; set; } = 0;
        public ProgressStyle ProgressStyle { get; set; } =
            ProgressStyle.NoProgress;
        public DateTime StartTime { get; set; } = DateTime.MinValue;
    }
}
