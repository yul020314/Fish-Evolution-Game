using UnityEngine;

namespace FishEvolution.AI
{
    public sealed class FishMovement
    {
        private const float MinDirectionSqr = 0.0001f;

        private readonly Transform _transform;
        private readonly Rigidbody2D _rigidbody2D;

        public FishMovement(Transform transform, Rigidbody2D rigidbody2D)
        {
            _transform = transform;
            _rigidbody2D = rigidbody2D;
        }

        public void MoveToward(Vector2 target, float speed)
        {
            var direction = target - (Vector2)_transform.position;
            MoveInDirection(direction.normalized, speed);
        }

        public void MoveInDirection(Vector2 direction, float speed)
        {
            if (direction.sqrMagnitude <= MinDirectionSqr)
            {
                Stop();
                return;
            }

            _rigidbody2D.linearVelocity = direction.normalized * speed;
            FaceDirection(direction);
        }

        public void Stop()
        {
            if (_rigidbody2D != null)
            {
                _rigidbody2D.linearVelocity = Vector2.zero;
            }
        }

        public void FaceDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude <= MinDirectionSqr)
            {
                return;
            }

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public bool IsNear(Vector2 target, float tolerance)
        {
            var distance = Vector2.Distance(_transform.position, target);
            return distance <= Mathf.Max(0.01f, tolerance);
        }
    }
}
