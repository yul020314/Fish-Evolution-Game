using FishEvolution.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishEvolution.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerController : MonoBehaviour
    {
        private const float MinMoveMagnitudeSqr = 0.0001f;

        [SerializeField] private FishDataSO _fishData = null;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Collider2D _collider2D;

        private Vector2 _moveInput;

        public Vector2 MoveInput => _moveInput;

        private void Awake()
        {
            CacheComponents();
            _rigidbody2D.gravityScale = 0f;
            _rigidbody2D.freezeRotation = true;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _collider2D.isTrigger = false;
        }

        private void Reset()
        {
            CacheComponents();
        }

        private void OnDisable()
        {
            if (_rigidbody2D != null)
            {
                _rigidbody2D.linearVelocity = Vector2.zero;
            }
        }

        public void OnMove(InputValue inputValue)
        {
            _moveInput = Vector2.ClampMagnitude(inputValue.Get<Vector2>(), 1f);
            Move(_moveInput);
            RotateTo(_moveInput);
        }

        private void Move(Vector2 direction)
        {
            var speed = _fishData != null ? _fishData.Speed : 0f;
            _rigidbody2D.linearVelocity = direction * speed;
        }

        private void RotateTo(Vector2 direction)
        {
            if (direction.sqrMagnitude <= MinMoveMagnitudeSqr)
            {
                return;
            }

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void CacheComponents()
        {
            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }

            if (_collider2D == null)
            {
                _collider2D = GetComponent<Collider2D>();
            }

            if (_collider2D == null)
            {
                _collider2D = gameObject.AddComponent<CircleCollider2D>();
            }
        }
    }
}
