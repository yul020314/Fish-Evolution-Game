namespace FishEvolution.Performance
{
    public readonly struct PerformanceWarningEvent
    {
        public PerformanceWarningEvent(
            PerformanceWarningType warningType,
            float value,
            float threshold,
            PerformanceSnapshot snapshot)
        {
            WarningType = warningType;
            Value = value;
            Threshold = threshold;
            Snapshot = snapshot;
        }

        public PerformanceWarningType WarningType { get; }
        public float Value { get; }
        public float Threshold { get; }
        public PerformanceSnapshot Snapshot { get; }
    }
}
