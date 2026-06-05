using System;
using MessagePipe;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using VContainer.Unity;

namespace FishEvolution.Performance
{
    public sealed class PerformanceMonitor : IStartable, ITickable, IDisposable
    {
        private readonly PerformanceSettings _settings;
        private readonly IPublisher<PerformanceSnapshotEvent> _snapshotPublisher;
        private readonly IPublisher<PerformanceWarningEvent> _warningPublisher;

        private ProfilerRecorder _gcAllocRecorder;
        private ProfilerRecorder _drawCallsRecorder;
        private ProfilerRecorder _setPassRecorder;
        private float _elapsedTime;
        private int _frameCount;

        public PerformanceMonitor(
            PerformanceSettings settings,
            IPublisher<PerformanceSnapshotEvent> snapshotPublisher,
            IPublisher<PerformanceWarningEvent> warningPublisher)
        {
            _settings = settings;
            _snapshotPublisher = snapshotPublisher;
            _warningPublisher = warningPublisher;
        }

        public void Start()
        {
            ApplyFrameSettings();
            StartRecorders();
        }

        public void Tick()
        {
            var deltaTime = Time.unscaledDeltaTime;
            _elapsedTime += deltaTime;
            _frameCount++;

            if (_elapsedTime < _settings.SampleInterval)
            {
                return;
            }

            PublishSnapshot();
            ResetSample();
        }

        public void Dispose()
        {
            DisposeRecorders();
        }

        private void ApplyFrameSettings()
        {
            if (_settings.DisableVSync)
            {
                QualitySettings.vSyncCount = 0;
            }

            Application.targetFrameRate = _settings.TargetFrameRate;
        }

        private void StartRecorders()
        {
            _gcAllocRecorder = StartRecorder(
                ProfilerCategory.Memory,
                "GC Allocated In Frame");
            _drawCallsRecorder = StartRecorder(
                ProfilerCategory.Render,
                "Draw Calls Count");
            _setPassRecorder = StartRecorder(
                ProfilerCategory.Render,
                "SetPass Calls Count");
        }

        private static ProfilerRecorder StartRecorder(
            ProfilerCategory category,
            string markerName)
        {
            return ProfilerRecorder.StartNew(category, markerName);
        }

        private void PublishSnapshot()
        {
            var snapshot = CreateSnapshot();
            _snapshotPublisher.Publish(new PerformanceSnapshotEvent(snapshot));
            PublishWarnings(snapshot);
        }

        private PerformanceSnapshot CreateSnapshot()
        {
            var frameTimeMs = GetAverageFrameTimeMs();
            var fps = frameTimeMs > 0f ? 1000f / frameTimeMs : 0f;
            return new PerformanceSnapshot(
                fps,
                frameTimeMs,
                Profiler.GetTotalAllocatedMemoryLong(),
                Profiler.GetMonoUsedSizeLong(),
                GetRecorderValue(_gcAllocRecorder),
                GetRecorderValueAsInt(_drawCallsRecorder),
                GetRecorderValueAsInt(_setPassRecorder));
        }

        private float GetAverageFrameTimeMs()
        {
            if (_frameCount <= 0)
            {
                return 0f;
            }

            return _elapsedTime * 1000f / _frameCount;
        }

        private static long GetRecorderValue(ProfilerRecorder recorder)
        {
            return recorder.Valid ? recorder.LastValue : 0;
        }

        private static int GetRecorderValueAsInt(ProfilerRecorder recorder)
        {
            var value = GetRecorderValue(recorder);
            return value > int.MaxValue ? int.MaxValue : (int)value;
        }

        private void PublishWarnings(PerformanceSnapshot snapshot)
        {
            TryPublishWarning(
                PerformanceWarningType.LowFps,
                snapshot.Fps,
                _settings.LowFpsThreshold,
                snapshot.Fps < _settings.LowFpsThreshold,
                snapshot);
            TryPublishWarning(
                PerformanceWarningType.SlowFrame,
                snapshot.FrameTimeMs,
                _settings.SlowFrameThresholdMs,
                snapshot.FrameTimeMs > _settings.SlowFrameThresholdMs,
                snapshot);
            TryPublishWarning(
                PerformanceWarningType.HighMemory,
                snapshot.TotalAllocatedMemoryBytes,
                _settings.MemoryWarningBytes,
                snapshot.TotalAllocatedMemoryBytes > _settings.MemoryWarningBytes,
                snapshot);
            TryPublishWarning(
                PerformanceWarningType.GcAllocation,
                snapshot.GcAllocatedInFrameBytes,
                _settings.GcAllocWarningBytes,
                snapshot.GcAllocatedInFrameBytes > _settings.GcAllocWarningBytes,
                snapshot);
            TryPublishWarning(
                PerformanceWarningType.HighDrawCalls,
                snapshot.DrawCalls,
                _settings.DrawCallWarningCount,
                IsDrawCallWarning(snapshot),
                snapshot);
        }

        private bool IsDrawCallWarning(PerformanceSnapshot snapshot)
        {
            return _settings.DrawCallWarningCount > 0 &&
                snapshot.DrawCalls > _settings.DrawCallWarningCount;
        }

        private void TryPublishWarning(
            PerformanceWarningType warningType,
            float value,
            float threshold,
            bool shouldPublish,
            PerformanceSnapshot snapshot)
        {
            if (!shouldPublish)
            {
                return;
            }

            _warningPublisher.Publish(
                new PerformanceWarningEvent(
                    warningType,
                    value,
                    threshold,
                    snapshot));
        }

        private void ResetSample()
        {
            _elapsedTime = 0f;
            _frameCount = 0;
        }

        private void DisposeRecorders()
        {
            _gcAllocRecorder.Dispose();
            _drawCallsRecorder.Dispose();
            _setPassRecorder.Dispose();
        }
    }
}
