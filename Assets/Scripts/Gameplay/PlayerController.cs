using FishEvolution.Config;
using FishEvolution.Combat;
using MessagePipe;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

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
        private IPublisher<SkillUseRequest> _skillUsePublisher;
        private BuffManager _buffManager;

        public FishDataSO FishData => _fishData;
        public Vector2 MoveInput => _moveInput;

        [Inject]
        public void Construct(
            IPublisher<SkillUseRequest> skillUsePublisher,
            BuffManager buffManager)
        {
            _skillUsePublisher = skillUsePublisher;
            _buffManager = buffManager;
            _buffManager.RegisterPlayer(this);
        }

        private void Awake()
        {
            CacheComponents();
            _rigidbody2D.gravityScale = 0f;
            _rigidbody2D.freezeRotation = true;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _collider2D.isTrigger = false;
        }

        private void Start()
        {
            InitializeCombat();
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

        public void OnSkill(InputValue inputValue)
        {
            if (!inputValue.isPressed || _skillUsePublisher == null)
            {
                return;
            }

            _skillUsePublisher.Publish(new SkillUseRequest(this));
        }

        public void Dash(float distance)
        {
            if (_rigidbody2D == null || distance <= 0f)
            {
                return;
            }

            var direction = GetSkillDirection();
            _rigidbody2D.position += direction * distance;
            RotateTo(direction);
        }

        public Vector2 GetSkillDirection()
        {
            if (_moveInput.sqrMagnitude > MinMoveMagnitudeSqr)
            {
                return _moveInput.normalized;
            }

            return transform.right;
        }

        public void RefreshMovement()
        {
            Move(_moveInput);
        }

        private void Move(Vector2 direction)
        {
            var speed = _fishData != null ? _fishData.Speed : 0f;
            _rigidbody2D.linearVelocity = direction * GetMoveSpeed(speed);
        }

        private float GetMoveSpeed(float baseSpeed)
        {
            if (_buffManager == null)
            {
                return baseSpeed;
            }

            return baseSpeed * _buffManager.GetSpeedMultiplier(this);
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

        private void InitializeCombat()
        {
            if (_fishData == null)
            {
                return;
            }

            if (TryGetComponent<HealthComponent>(out var health))
            {
                health.Initialize(_fishData.HP);
            }

            if (TryGetComponent<AttackComponent>(out var attack))
            {
                attack.Initialize(_fishData.Attack);
            }
        }
    }

    public readonly struct SkillUseRequest
    {
        public SkillUseRequest(PlayerController player)
        {
            Player = player;
        }

        public PlayerController Player { get; }
        public bool IsValid => Player != null;
    }

    public readonly struct SkillUsedEvent
    {
        public SkillUsedEvent(
            PlayerController player,
            SkillType skillType,
            float cooldown)
        {
            Player = player;
            SkillType = skillType;
            Cooldown = cooldown;
        }

        public PlayerController Player { get; }
        public SkillType SkillType { get; }
        public float Cooldown { get; }
    }

}
