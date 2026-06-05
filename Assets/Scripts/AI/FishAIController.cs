using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FishEvolution.Combat;
using FishEvolution.Gameplay;
using UnityEngine;

namespace FishEvolution.AI
{
    [RequireComponent(typeof(FishController))]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class FishAIController : MonoBehaviour
    {
        [SerializeField] private FishController _fishController;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private AttackComponent _attackComponent;
        [SerializeField] private FishAISettings _settings = new FishAISettings();

        private FishAIContext _context;
        private FishStateMachine _stateMachine;
        private FishMovement _movement;
        private FishPerception _perception;
        private FishAIDecision _decision;
        private bool _isInitialized;
        private bool _isRunning;
        private int _runId;

        public float PatrolSpeed => GetBaseSpeed() * _settings.PatrolSpeedMultiplier;
        public float ChaseSpeed => GetBaseSpeed() * _settings.ChaseSpeedMultiplier;
        public float EscapeSpeed => GetBaseSpeed() * _settings.EscapeSpeedMultiplier;
        public float PatrolRetargetInterval => _settings.PatrolRetargetInterval;

        private void Awake()
        {
            CacheComponents();
            ConfigureRigidbody();
            CreateRuntime();
            CreateStateMachine();
        }

        private void OnEnable()
        {
            CreateRuntime();
            _stateMachine.ChangeState(FishState.Patrol, _context);
            StartAIWhenReady();
        }

        private void OnDisable()
        {
            _runId++;
            _isInitialized = false;
            _isRunning = false;
            Stop();
        }

        public void Initialize(
            PlayerController player,
            Vector2 patrolCenter,
            Vector2 patrolSize)
        {
            CreateRuntime();
            _settings.SetPatrolArea(patrolCenter, patrolSize);
            _context.SetPlayer(player != null ? player.transform : null);
            _context.SetPatrolArea(_settings.PatrolCenter, _settings.PatrolSize);
            _context.ResetPatrolTarget();
            _isInitialized = true;
            StartAIWhenReady();
        }

        public void MoveToward(Vector2 target, float speed)
        {
            _movement.MoveToward(target, speed);
        }

        public void MoveInDirection(Vector2 direction, float speed)
        {
            _movement.MoveInDirection(direction, speed);
        }

        public void Stop()
        {
            _movement?.Stop();
        }

        public void FaceDirection(Vector2 direction)
        {
            _movement.FaceDirection(direction);
        }

        public bool IsNear(Vector2 target)
        {
            return _movement.IsNear(target, _settings.PatrolTargetTolerance);
        }

        public bool TryGetDirectionToPlayer(out Vector2 direction)
        {
            return _perception.TryGetDirectionToPlayer(out direction);
        }

        public bool TryGetDirectionFromPlayer(out Vector2 direction)
        {
            return _perception.TryGetDirectionFromPlayer(out direction);
        }

        public bool TryAttackPlayer()
        {
            if (_attackComponent == null ||
                _context.Player == null ||
                !_context.Player.TryGetComponent<HealthComponent>(out var health))
            {
                return false;
            }

            return _attackComponent.TryAttack(health);
        }

        private async UniTaskVoid RunAIAsync(
            int runId,
            CancellationToken cancellationToken)
        {
            _isRunning = true;

            try
            {
                while (CanRun(runId, cancellationToken))
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
                    if (CanRun(runId, cancellationToken))
                    {
                        TickAI(Time.fixedDeltaTime);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _isRunning = false;
            }
        }

        private void StartAIWhenReady()
        {
            if (!_isInitialized || _isRunning || !isActiveAndEnabled)
            {
                return;
            }

            _runId++;
            RunAIAsync(_runId, destroyCancellationToken).Forget();
        }

        private bool CanRun(int runId, CancellationToken cancellationToken)
        {
            return _isRunning &&
                _runId == runId &&
                !cancellationToken.IsCancellationRequested;
        }

        private void TickAI(float deltaTime)
        {
            _stateMachine.ChangeState(_decision.GetDesiredState(), _context);
            _stateMachine.Tick(_context, deltaTime);
        }

        private float GetBaseSpeed()
        {
            var fishData = _fishController != null ? _fishController.FishData : null;
            return fishData != null ? Mathf.Max(0f, fishData.Speed) : 0f;
        }

        private void CreateRuntime()
        {
            if (_context != null)
            {
                return;
            }

            _movement = new FishMovement(transform, _rigidbody2D);
            _context = new FishAIContext(this, transform);
            _context.SetPatrolArea(_settings.PatrolCenter, _settings.PatrolSize);
            _perception = new FishPerception(_context);
            _decision = new FishAIDecision(_perception, _settings);
        }

        private void CreateStateMachine()
        {
            _stateMachine = new FishStateMachine();
            _stateMachine.Register(new PatrolState());
            _stateMachine.Register(new EscapeState());
            _stateMachine.Register(new ChaseState());
            _stateMachine.Register(new AttackState());
        }

        private void ConfigureRigidbody()
        {
            _rigidbody2D.gravityScale = 0f;
            _rigidbody2D.freezeRotation = true;
            _rigidbody2D.linearVelocity = Vector2.zero;
        }

        private void CacheComponents()
        {
            if (_fishController == null)
            {
                _fishController = GetComponent<FishController>();
            }

            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }

            if (_attackComponent == null)
            {
                _attackComponent = GetComponent<AttackComponent>();
            }
        }
    }
}
