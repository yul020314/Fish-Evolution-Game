using System;
using UnityEngine;

namespace FishEvolution.Performance
{
    [Serializable]
    public sealed class PerformanceSettings
    {
        [SerializeField] private int _targetFrameRate = 60;
        [SerializeField] private float _sampleInterval = 1f;
        [SerializeField] private float _lowFpsThreshold = 55f;
        [SerializeField] private float _slowFrameThresholdMs = 18f;
        [SerializeField] private float _memoryWarningMb = 512f;
        [SerializeField] private long _gcAllocWarningBytes = 1024;
        [SerializeField] private int _drawCallWarningCount = 120;
        [SerializeField] private bool _disableVSync = true;

        public int TargetFrameRate => Mathf.Max(30, _targetFrameRate);
        public float SampleInterval => Mathf.Max(0.25f, _sampleInterval);
        public float LowFpsThreshold => Mathf.Max(1f, _lowFpsThreshold);
        public float SlowFrameThresholdMs => Mathf.Max(1f, _slowFrameThresholdMs);
        public long MemoryWarningBytes =>
            Mathf.Max(1f, _memoryWarningMb) * 1024f * 1024f > long.MaxValue
                ? long.MaxValue
                : (long)(Mathf.Max(1f, _memoryWarningMb) * 1024f * 1024f);
        public long GcAllocWarningBytes => Math.Max(0, _gcAllocWarningBytes);
        public int DrawCallWarningCount => Mathf.Max(0, _drawCallWarningCount);
        public bool DisableVSync => _disableVSync;
    }
}
