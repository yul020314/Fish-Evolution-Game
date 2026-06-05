namespace FishEvolution.Performance
{
    public readonly struct PerformanceSnapshot
    {
        public PerformanceSnapshot(
            float fps,
            float frameTimeMs,
            long totalAllocatedMemoryBytes,
            long gcUsedMemoryBytes,
            long gcAllocatedInFrameBytes,
            int drawCalls,
            int setPassCalls)
        {
            Fps = fps;
            FrameTimeMs = frameTimeMs;
            TotalAllocatedMemoryBytes = totalAllocatedMemoryBytes;
            GcUsedMemoryBytes = gcUsedMemoryBytes;
            GcAllocatedInFrameBytes = gcAllocatedInFrameBytes;
            DrawCalls = drawCalls;
            SetPassCalls = setPassCalls;
        }

        public float Fps { get; }
        public float FrameTimeMs { get; }
        public long TotalAllocatedMemoryBytes { get; }
        public long GcUsedMemoryBytes { get; }
        public long GcAllocatedInFrameBytes { get; }
        public int DrawCalls { get; }
        public int SetPassCalls { get; }
    }
}
