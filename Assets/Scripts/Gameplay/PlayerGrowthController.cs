using System;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace FishEvolution.Gameplay
{
    [RequireComponent(typeof(PlayerController))]
    public sealed class PlayerGrowthController : MonoBehaviour
    {
        [SerializeField] private int _initialLevel = 1;
        [SerializeField] private int _maxLevel = 100;
        [SerializeField] private int _experienceMultiplier = 100;
        [SerializeField] private float _experiencePower = 1.5f;
        [SerializeField] private bool _captureBaseScaleOnAwake = true;
        [SerializeField] private Vector3 _baseScale = Vector3.one;
        [SerializeField] private int _baseHp = 100;
        [SerializeField] private int _hpPerLevel = 25;
        [SerializeField] private int _baseAttack = 10;
        [SerializeField] private int _attackPerLevel = 5;
        [SerializeField] private float _baseSpeed = 6f;
        [SerializeField] private float _speedPerLevel = 0.03f;
        [SerializeField] private float _minSpeed = 2f;
        [SerializeField] private float _scaleBase = 1f;
        [SerializeField] private float _scalePerLevel = 0.12f;

        private ExpSystem _expSystem;
        private LevelSystem _levelSystem;
        private GrowthSystem _growthSystem;
        private PlayerController _playerController;
        private IPublisher<PlayerLevelUpEvent> _levelUpPublisher;
        private IPublisher<PlayerProgressChangedEvent> _progressChangedPublisher;
        private BuffManager _buffManager;
        private IDisposable _foodConsumedSubscription;

        public int CurrentLevel => _levelSystem != null ? _levelSystem.CurrentLevel : _initialLevel;
        public int CurrentExperience => _expSystem != null ? _expSystem.CurrentExperience : 0;
        public int RequiredExperience => _levelSystem != null ? _levelSystem.GetRequiredExperience() : 0;
        public int CurrentHp => _growthSystem != null ? _growthSystem.GetHp(CurrentLevel) : 0;
        public int CurrentAttack => _growthSystem != null ? _growthSystem.GetAttack(CurrentLevel) : 0;
        public float CurrentSpeed => _growthSystem != null ? _growthSystem.GetSpeed(CurrentLevel) : 0f;

        [Inject]
        public void Construct(
            ISubscriber<FoodConsumedEvent> foodConsumedSubscriber,
            IPublisher<PlayerLevelUpEvent> levelUpPublisher,
            IPublisher<PlayerProgressChangedEvent> progressChangedPublisher,
            BuffManager buffManager)
        {
            _levelUpPublisher = levelUpPublisher;
            _progressChangedPublisher = progressChangedPublisher;
            _buffManager = buffManager;
            _foodConsumedSubscription = foodConsumedSubscriber.Subscribe(HandleFoodConsumed);
        }

        private void Awake()
        {
            CachePlayerController();
            InitializeSystems();
            ApplyGrowth();
        }

        private void Reset()
        {
            _baseScale = transform.localScale;
            CachePlayerController();
        }

        private void OnDestroy()
        {
            _foodConsumedSubscription?.Dispose();
            _foodConsumedSubscription = null;
        }

        public void AddExperience(int amount)
        {
            if (_expSystem == null || _levelSystem == null)
            {
                InitializeSystems();
            }

            _expSystem.AddExperience(amount);
            ProcessLevelUps();
        }

        public void RestoreProgress(
            int level,
            int experience)
        {
            if (_expSystem == null || _levelSystem == null)
            {
                InitializeSystems();
            }

            _levelSystem.SetLevel(level);
            _expSystem.SetExperience(experience);
            ApplyGrowth();
            PublishProgressChanged();
        }

        private void InitializeSystems()
        {
            CacheBaseScale();

            _expSystem = new ExpSystem();
            _levelSystem = new LevelSystem(
                _initialLevel,
                _maxLevel,
                _experienceMultiplier,
                _experiencePower);
            _growthSystem = CreateGrowthSystem();
        }

        private GrowthSystem CreateGrowthSystem()
        {
            return new GrowthSystem(
                _baseHp,
                _hpPerLevel,
                _baseAttack,
                _attackPerLevel,
                _baseSpeed,
                _speedPerLevel,
                _minSpeed,
                _scaleBase,
                _scalePerLevel);
        }

        private void ProcessLevelUps()
        {
            var previousLevel = CurrentLevel;
            while (_levelSystem.TryLevelUp(_expSystem))
            {
            }

            if (previousLevel != CurrentLevel)
            {
                ApplyGrowth();
                PublishLevelUp(previousLevel);
            }
        }

        private void ApplyGrowth()
        {
            _growthSystem.ApplyScale(transform, _baseScale, CurrentLevel);
        }

        private void CacheBaseScale()
        {
            if (_captureBaseScaleOnAwake || _baseScale == Vector3.zero)
            {
                _baseScale = transform.localScale;
            }
        }

        private void CachePlayerController()
        {
            if (_playerController == null)
            {
                _playerController = GetComponent<PlayerController>();
            }
        }

        private void HandleFoodConsumed(FoodConsumedEvent message)
        {
            if (message.Player != _playerController)
            {
                return;
            }

            AddExperience(GetBuffedExperience(message.Experience));
        }

        private int GetBuffedExperience(int experience)
        {
            var multiplier = _buffManager != null
                ? _buffManager.GetExperienceMultiplier(_playerController)
                : 1f;
            return Mathf.Max(0, Mathf.RoundToInt(experience * multiplier));
        }

        private void PublishLevelUp(int previousLevel)
        {
            if (_levelUpPublisher == null)
            {
                return;
            }

            _levelUpPublisher.Publish(
                new PlayerLevelUpEvent(_playerController, previousLevel, CurrentLevel));
        }

        private void PublishProgressChanged()
        {
            if (_progressChangedPublisher == null)
            {
                return;
            }

            _progressChangedPublisher.Publish(
                new PlayerProgressChangedEvent(_playerController));
        }
    }
}
