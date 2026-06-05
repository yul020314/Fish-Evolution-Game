using System;
using FishEvolution.Combat;
using FishEvolution.Config;
using FishEvolution.Gameplay;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace FishEvolution.Boss
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(AttackComponent))]
    public sealed class BossController : MonoBehaviour
    {
        [SerializeField] private BossDataSO _bossData;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private HealthComponent _health;
        [SerializeField] private AttackComponent _attack;
        [SerializeField] private BossAISettings _settings = new BossAISettings();
        [SerializeField] private BossSkillSet _skillSet = new BossSkillSet();

        private BossContext _context;
        private BossStateMachine _stateMachine;
        private BossSkillSystem _skillSystem;
        private PlayerController _player;
        private IPublisher<DamageRequest> _damagePublisher;
        private IPublisher<BossDeadEvent> _bossDeadPublisher;
        private ISubscriber<EntityDeathEvent> _deathSubscriber;
        private IDisposable _deathSubscription;
        private bool _deathPublished;

        public BossDataSO BossData => _bossData;
        public BossState CurrentState =>
            _stateMachine != null ? _stateMachine.CurrentState : BossState.Idle;

        [Inject]
        public void Construct(
            PlayerController player,
            IPublisher<DamageRequest> damagePublisher,
            IPublisher<BossDeadEvent> bossDeadPublisher,
            ISubscriber<EntityDeathEvent> deathSubscriber)
        {
            _player = player;
            _damagePublisher = damagePublisher;
            _bossDeadPublisher = bossDeadPublisher;
            _deathSubscriber = deathSubscriber;
        }

        private void Awake()
        {
            CacheComponents();
            ConfigureRigidbody();
            InitializeCombat();
            CreateRuntime();
        }

        private void OnEnable()
        {
            _deathPublished = false;
            _context?.SetPlayer(_player);
            _stateMachine?.ChangeState(BossState.Idle, _context);
            SubscribeDeath();
        }

        private void FixedUpdate()
        {
            if (_context == null || _stateMachine == null)
            {
                return;
            }

            _stateMachine.Tick(_context, Time.fixedDeltaTime);
        }

        private void OnDisable()
        {
            _deathSubscription?.Dispose();
            _deathSubscription = null;
            _rigidbody2D.linearVelocity = Vector2.zero;
        }

        private void Reset()
        {
            CacheComponents();
        }

        public void ChangeState(BossState state)
        {
            _stateMachine.ChangeState(state, _context);
        }

        public bool TryAttackPlayer()
        {
            if (_player == null ||
                !_player.TryGetComponent<HealthComponent>(out var target))
            {
                return false;
            }

            return _attack.TryAttack(target);
        }

        private void InitializeCombat()
        {
            if (_bossData == null)
            {
                return;
            }

            gameObject.name = _bossData.BossName;
            _health.Initialize(_bossData.HP);
            _attack.Initialize(_bossData.Attack, _damagePublisher);
        }

        private void CreateRuntime()
        {
            _skillSystem = new BossSkillSystem(_skillSet, _damagePublisher);
            _context = new BossContext(
                this,
                transform,
                _rigidbody2D,
                _health,
                _attack,
                _settings,
                _skillSystem);
            _context.SetPlayer(_player);
            CreateStateMachine();
        }

        private void CreateStateMachine()
        {
            _stateMachine = new BossStateMachine();
            _stateMachine.Register(new BossIdleState());
            _stateMachine.Register(new BossChaseState());
            _stateMachine.Register(new BossAttackState());
            _stateMachine.Register(new BossEnragedState());
            _stateMachine.Register(new BossDeadState());
        }

        private void SubscribeDeath()
        {
            _deathSubscription?.Dispose();
            _deathSubscription = _deathSubscriber?.Subscribe(HandleEntityDeath);
        }

        private void HandleEntityDeath(EntityDeathEvent deathEvent)
        {
            if (_deathPublished ||
                deathEvent.Entity != gameObject)
            {
                return;
            }

            _deathPublished = true;
            ChangeState(BossState.Dead);
            _bossDeadPublisher.Publish(
                new BossDeadEvent(
                    _bossData,
                    gameObject,
                    deathEvent.Killer));
        }

        private void ConfigureRigidbody()
        {
            _rigidbody2D.gravityScale = 0f;
            _rigidbody2D.freezeRotation = true;
            _rigidbody2D.linearVelocity = Vector2.zero;
        }

        private void CacheComponents()
        {
            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }

            if (_health == null)
            {
                _health = GetComponent<HealthComponent>();
            }

            if (_attack == null)
            {
                _attack = GetComponent<AttackComponent>();
            }
        }
    }
}
