using UnityEngine;

namespace FishEvolution.AI
{
    public sealed class FishPerception
    {
        private const float MinDirectionSqr = 0.0001f;

        private readonly FishAIContext _context;

        public FishPerception(FishAIContext context)
        {
            _context = context;
        }

        public bool TryGetDirectionToPlayer(out Vector2 direction)
        {
            direction = Vector2.zero;
            if (_context.Player == null)
            {
                return false;
            }

            direction = _context.Player.position - _context.Transform.position;
            return direction.sqrMagnitude > MinDirectionSqr;
        }

        public bool TryGetDirectionFromPlayer(out Vector2 direction)
        {
            var hasDirection = TryGetDirectionToPlayer(out direction);
            direction = -direction;
            return hasDirection;
        }

        public bool IsPlayerDetected(float detectionRadius)
        {
            return GetPlayerDistance() <= Mathf.Max(0.1f, detectionRadius);
        }

        public bool ShouldEscape(float escapeDistance, float sizeMultiplier)
        {
            return GetPlayerDistance() <= Mathf.Max(0.1f, escapeDistance) &&
                GetFishSize() * sizeMultiplier < GetPlayerSize();
        }

        public bool ShouldChase(float sizeMultiplier)
        {
            return GetFishSize() > GetPlayerSize() * sizeMultiplier;
        }

        public float GetPlayerDistance()
        {
            return _context.Player != null
                ? Vector2.Distance(_context.Transform.position, _context.Player.position)
                : float.MaxValue;
        }

        private float GetFishSize()
        {
            return Mathf.Max(
                _context.Transform.lossyScale.x,
                _context.Transform.lossyScale.y);
        }

        private float GetPlayerSize()
        {
            return _context.Player != null
                ? Mathf.Max(_context.Player.lossyScale.x, _context.Player.lossyScale.y)
                : 0f;
        }
    }
}
