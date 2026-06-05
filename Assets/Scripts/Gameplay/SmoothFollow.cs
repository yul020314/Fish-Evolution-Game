using DG.Tweening;
using UnityEngine;

namespace FishEvolution.Gameplay
{
    [System.Serializable]
    public sealed class SmoothFollow
    {
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);
        [SerializeField] private float _duration = 0.18f;
        [SerializeField] private float _positionThreshold = 0.02f;

        private Tweener _moveTween;

        public Vector3 Offset => _offset;

        public void Initialize(Transform cameraTransform, Transform target)
        {
            if (cameraTransform == null || target == null)
            {
                return;
            }

            cameraTransform.position = CalculateTargetPosition(target);
        }

        public void Follow(Transform cameraTransform, Transform target)
        {
            if (cameraTransform == null || target == null)
            {
                return;
            }

            var targetPosition = CalculateTargetPosition(target);
            if (!ShouldMove(cameraTransform.position, targetPosition))
            {
                return;
            }

            TweenToPosition(cameraTransform, targetPosition);
        }

        public void Stop()
        {
            if (_moveTween != null && _moveTween.IsActive())
            {
                _moveTween.Kill();
            }

            _moveTween = null;
        }

        private Vector3 CalculateTargetPosition(Transform target)
        {
            return target.position + _offset;
        }

        private void TweenToPosition(Transform cameraTransform, Vector3 targetPosition)
        {
            if (_moveTween != null && _moveTween.IsActive())
            {
                _moveTween.ChangeEndValue(targetPosition, _duration, true);
                return;
            }

            _moveTween = DOTween.To(
                    () => cameraTransform.position,
                    value => cameraTransform.position = value,
                    targetPosition,
                    _duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(UpdateType.Late)
                .SetTarget(cameraTransform)
                .OnKill(() => _moveTween = null);
        }

        private bool ShouldMove(Vector3 currentPosition, Vector3 targetPosition)
        {
            var distanceSqr = (currentPosition - targetPosition).sqrMagnitude;
            return distanceSqr > _positionThreshold * _positionThreshold;
        }
    }
}
