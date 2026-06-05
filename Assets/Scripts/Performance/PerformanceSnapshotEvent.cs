namespace FishEvolution.Performance
{
    public readonly struct PerformanceSnapshotEvent
    {
        public PerformanceSnapshotEvent(PerformanceSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public PerformanceSnapshot Snapshot { get; }
    }
}
