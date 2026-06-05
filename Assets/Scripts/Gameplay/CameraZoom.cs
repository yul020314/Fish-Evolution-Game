using DG.Tweening;
using UnityEngine;

namespace FishEvolution.Gameplay
{
    [System.Serializable]
    public sealed class CameraZoom
    {
        [SerializeField] private float _baseOrthographicSize = 5f;
        [SerializeField] private float _zoomPerScale = 1.2f;
        [SerializeField] private float _minOrthographicSize = 4f;
        [SerializeField] private float _maxOrthographicSize = 12f;
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private float _sizeThreshold = 0.03f;

        private Tweener _zoomTween;

        public float BaseOrthographicSize => _baseOrthographicSize;

        public void Initialize(Camera camera, Transform target)
        {
            if (camera == null)
            {
                return;
            }

            camera.orthographic = true;
            camera.orthographicSize = CalculateTargetSize(target);
        }

        public void Zoom(Camera camera, Transform target)
        {
            if (camera == null || target == null)
            {
                return;
            }

            var targetSize = CalculateTargetSize(target);
            if (Mathf.Abs(camera.orthographicSize - targetSize) <= _sizeThreshold)
            {
                return;
            }

            TweenToSize(camera, targetSize);
        }

        public void Stop()
        {
            if (_zoomTween != null && _zoomTween.IsActive())
            {
                _zoomTween.Kill();
            }

            _zoomTween = null;
        }

        private float CalculateTargetSize(Transform target)
        {
            var targetScale = GetTargetScale(target);
            var growthScale = Mathf.Max(0f, targetScale - 1f);
            var targetSize = _baseOrthographicSize + growthScale * _zoomPerScale;
            return Mathf.Clamp(targetSize, _minOrthographicSize, _maxOrthographicSize);
        }

        private void TweenToSize(Camera camera, float targetSize)
        {
            if (_zoomTween != null && _zoomTween.IsActive())
            {
                _zoomTween.ChangeEndValue(targetSize, _duration, true);
                return;
            }

            _zoomTween = DOTween.To(
                    () => camera.orthographicSize,
                    value => camera.orthographicSize = value,
                    targetSize,
                    _duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(UpdateType.Late)
                .SetTarget(camera)
                .OnKill(() => _zoomTween = null);
        }

        private static float GetTargetScale(Transform target)
        {
            if (target == null)
            {
                return 1f;
            }

            var scale = target.lossyScale;
            return Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
        }
    }
}
