using FishEvolution.Combat;
using FishEvolution.Config;
using FishEvolution.UI;
using System;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FishEvolution.Gameplay
{
    public sealed class GameplayLifetimeScope : LifetimeScope
    {
        [UnityEngine.SerializeField] private HudDisplayDataSO _hudDisplayData;
        [UnityEngine.SerializeField] private SkillDataSO[] _skillData = new SkillDataSO[0];

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterComponentInHierarchy<PlayerController>();
            builder.RegisterComponentInHierarchy<PlayerGrowthController>();
            builder.RegisterComponentInHierarchy<FishSpawner>();
            builder.RegisterMessageBroker<FoodConsumedEvent>(options);
            builder.RegisterMessageBroker<PlayerLevelUpEvent>(options);
            builder.RegisterMessageBroker<DamageRequest>(options);
            builder.RegisterMessageBroker<DamageAppliedEvent>(options);
            builder.RegisterMessageBroker<EntityDeathEvent>(options);
            builder.RegisterMessageBroker<EatRequest>(options);
            builder.RegisterMessageBroker<EatCompletedEvent>(options);
            builder.RegisterMessageBroker<SkillUseRequest>(options);
            builder.RegisterMessageBroker<SkillUsedEvent>(options);

            builder.Register<SizeCheck>(Lifetime.Singleton)
                .WithParameter(1.1f);
            builder.RegisterInstance(new SkillCatalog(_skillData));
            builder.Register<PlayerSkillState>(Lifetime.Singleton);
            if (_hudDisplayData != null)
            {
                builder.RegisterInstance(HudDisplaySettings.FromData(_hudDisplayData));
                builder.RegisterEntryPoint<GameplayHudController>(Lifetime.Singleton);
            }

            builder.RegisterEntryPoint<DamageSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<DeathSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<EatSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SkillSystem>(Lifetime.Singleton);
        }
    }

    public sealed class SkillCatalog
    {
        private readonly SkillDataSO[] _skills;

        public SkillCatalog(SkillDataSO[] skills)
        {
            _skills = skills ?? new SkillDataSO[0];
        }

        public bool TryGet(
            SkillType skillType,
            out SkillDataSO skillData)
        {
            for (var index = 0; index < _skills.Length; index++)
            {
                skillData = _skills[index];
                if (skillData != null && skillData.SkillType == skillType)
                {
                    return true;
                }
            }

            skillData = null;
            return false;
        }
    }

    public sealed class PlayerSkillState
    {
        private readonly PlayerController _player;
        private HealthComponent _playerHealth;
        private AttackComponent _playerAttack;
        private float _shieldExpiresAt;
        private float _shieldReduction;
        private float _frenzyExpiresAt;
        private float _frenzyMultiplier = 1f;

        public PlayerSkillState(PlayerController player)
        {
            _player = player;
            CachePlayerComponents();
        }

        public void Tick(float time)
        {
            if (!IsFrenzyActive(time))
            {
                ResetFrenzy();
            }
        }

        public void ActivateShield(
            float reduction,
            float duration,
            float time)
        {
            _shieldReduction = Mathf.Clamp01(reduction);
            _shieldExpiresAt = time + Mathf.Max(0f, duration);
        }

        public void ActivateFrenzy(
            float multiplier,
            float duration,
            float time)
        {
            _frenzyMultiplier = Mathf.Max(1f, multiplier);
            _frenzyExpiresAt = time + Mathf.Max(0f, duration);
            _playerAttack?.SetDamageMultiplier(_frenzyMultiplier);
        }

        public int GetDamageAfterShield(
            HealthComponent target,
            int damage)
        {
            if (!CanShield(target, damage))
            {
                return damage;
            }

            var value = damage * (1f - _shieldReduction);
            return Mathf.Max(1, Mathf.CeilToInt(value));
        }

        private bool CanShield(
            HealthComponent target,
            int damage)
        {
            return damage > 0 &&
                target == _playerHealth &&
                IsShieldActive(Time.time);
        }

        private bool IsShieldActive(float time)
        {
            return _shieldReduction > 0f && time < _shieldExpiresAt;
        }

        private bool IsFrenzyActive(float time)
        {
            return _frenzyMultiplier > 1f && time < _frenzyExpiresAt;
        }

        private void ResetFrenzy()
        {
            if (_frenzyMultiplier <= 1f)
            {
                return;
            }

            _frenzyMultiplier = 1f;
            _playerAttack?.SetDamageMultiplier(1f);
        }

        private void CachePlayerComponents()
        {
            if (_player == null)
            {
                return;
            }

            _player.TryGetComponent(out _playerHealth);
            _player.TryGetComponent(out _playerAttack);
        }
    }

    public sealed class SkillSystem : IStartable, ITickable, IDisposable
    {
        private readonly PlayerController _player;
        private readonly SkillCatalog _skillCatalog;
        private readonly PlayerSkillState _skillState;
        private readonly ISubscriber<SkillUseRequest> _skillSubscriber;
        private readonly IPublisher<SkillUsedEvent> _skillUsedPublisher;
        private readonly ISkillEffect[] _effects;
        private readonly SkillCooldownSlot[] _cooldowns;
        private IDisposable _skillSubscription;

        public SkillSystem(
            PlayerController player,
            SkillCatalog skillCatalog,
            PlayerSkillState skillState,
            ISubscriber<SkillUseRequest> skillSubscriber,
            IPublisher<SkillUsedEvent> skillUsedPublisher,
            IPublisher<DamageRequest> damagePublisher)
        {
            _player = player;
            _skillCatalog = skillCatalog;
            _skillState = skillState;
            _skillSubscriber = skillSubscriber;
            _skillUsedPublisher = skillUsedPublisher;
            _effects = CreateEffects(skillState, damagePublisher);
            _cooldowns = CreateCooldownSlots();
        }

        public void Start()
        {
            _skillSubscription = _skillSubscriber.Subscribe(HandleSkillUse);
        }

        public void Tick()
        {
            _skillState.Tick(Time.time);
        }

        public void Dispose()
        {
            _skillSubscription?.Dispose();
            _skillSubscription = null;
        }

        private void HandleSkillUse(SkillUseRequest request)
        {
            if (!CanHandle(request, out var skillType, out var skillData))
            {
                return;
            }

            var effect = GetEffect(skillType);
            if (effect == null || !effect.TryUse(_player, skillData, 1))
            {
                return;
            }

            var cooldown = skillData.GetCooldown(1);
            SetCooldown(skillType, Time.time + cooldown);
            _skillUsedPublisher.Publish(
                new SkillUsedEvent(_player, skillType, cooldown));
        }

        private bool CanHandle(
            SkillUseRequest request,
            out SkillType skillType,
            out SkillDataSO skillData)
        {
            skillType = GetPlayerSkillType(request);
            skillData = null;
            return request.Player == _player &&
                skillType != SkillType.None &&
                _skillCatalog.TryGet(skillType, out skillData) &&
                IsCooldownReady(skillType, Time.time);
        }

        private SkillType GetPlayerSkillType(SkillUseRequest request)
        {
            if (!request.IsValid || request.Player.FishData == null)
            {
                return SkillType.None;
            }

            return request.Player.FishData.Skill;
        }

        private ISkillEffect GetEffect(SkillType skillType)
        {
            for (var index = 0; index < _effects.Length; index++)
            {
                if (_effects[index].SkillType == skillType)
                {
                    return _effects[index];
                }
            }

            return null;
        }

        private bool IsCooldownReady(
            SkillType skillType,
            float time)
        {
            var slot = GetCooldownSlot(skillType);
            return slot == null || slot.ReadyAt <= time;
        }

        private void SetCooldown(
            SkillType skillType,
            float readyAt)
        {
            var slot = GetCooldownSlot(skillType);
            if (slot != null)
            {
                slot.ReadyAt = readyAt;
            }
        }

        private SkillCooldownSlot GetCooldownSlot(SkillType skillType)
        {
            for (var index = 0; index < _cooldowns.Length; index++)
            {
                if (_cooldowns[index].SkillType == skillType)
                {
                    return _cooldowns[index];
                }
            }

            return null;
        }

        private static ISkillEffect[] CreateEffects(
            PlayerSkillState skillState,
            IPublisher<DamageRequest> damagePublisher)
        {
            return new ISkillEffect[]
            {
                new DashSkillEffect(),
                new SonarSkillEffect(damagePublisher),
                new ShieldSkillEffect(skillState),
                new FrenzySkillEffect(skillState)
            };
        }

        private static SkillCooldownSlot[] CreateCooldownSlots()
        {
            return new SkillCooldownSlot[]
            {
                new SkillCooldownSlot(SkillType.Dash),
                new SkillCooldownSlot(SkillType.Sonar),
                new SkillCooldownSlot(SkillType.Shield),
                new SkillCooldownSlot(SkillType.Frenzy)
            };
        }
    }

    public interface ISkillEffect
    {
        SkillType SkillType { get; }

        bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level);
    }

    public sealed class SkillCooldownSlot
    {
        public SkillCooldownSlot(SkillType skillType)
        {
            SkillType = skillType;
        }

        public SkillType SkillType { get; }
        public float ReadyAt { get; set; }
    }

    public sealed class DashSkillEffect : ISkillEffect
    {
        public SkillType SkillType => SkillType.Dash;

        public bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            var distance = skillData.GetEffectValue(level);
            if (player == null || distance <= 0f)
            {
                return false;
            }

            player.Dash(distance);
            return true;
        }
    }

    public sealed class SonarSkillEffect : ISkillEffect
    {
        private const int MaxHits = 32;

        private readonly IPublisher<DamageRequest> _damagePublisher;
        private readonly Collider2D[] _hits = new Collider2D[MaxHits];

        public SonarSkillEffect(IPublisher<DamageRequest> damagePublisher)
        {
            _damagePublisher = damagePublisher;
        }

        public SkillType SkillType => SkillType.Sonar;

        public bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            if (!CanUse(player, skillData))
            {
                return false;
            }

            var damage = GetDamage(player, skillData, level);
            var filter = ContactFilter2D.noFilter;
            var count = Physics2D.OverlapCircle(
                player.transform.position,
                skillData.Range,
                filter,
                _hits);
            PublishHits(player, damage, count);
            return true;
        }

        private bool CanUse(
            PlayerController player,
            SkillDataSO skillData)
        {
            return player != null &&
                skillData.Range > 0f &&
                _damagePublisher != null;
        }

        private int GetDamage(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            var baseDamage = GetBaseDamage(player);
            var value = baseDamage * skillData.GetEffectValue(level);
            return Mathf.Max(1, Mathf.RoundToInt(value));
        }

        private int GetBaseDamage(PlayerController player)
        {
            return player.TryGetComponent<AttackComponent>(out var attack)
                ? attack.AttackDamage
                : 1;
        }

        private void PublishHits(
            PlayerController player,
            int damage,
            int count)
        {
            for (var index = 0; index < count; index++)
            {
                TryPublishHit(player, damage, _hits[index]);
                _hits[index] = null;
            }
        }

        private void TryPublishHit(
            PlayerController player,
            int damage,
            Collider2D hit)
        {
            if (hit == null ||
                hit.gameObject == player.gameObject ||
                !hit.TryGetComponent<HealthComponent>(out var health))
            {
                return;
            }

            _damagePublisher.Publish(
                new DamageRequest(player.gameObject, health, damage));
        }
    }

    public sealed class ShieldSkillEffect : ISkillEffect
    {
        private readonly PlayerSkillState _skillState;

        public ShieldSkillEffect(PlayerSkillState skillState)
        {
            _skillState = skillState;
        }

        public SkillType SkillType => SkillType.Shield;

        public bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            if (player == null)
            {
                return false;
            }

            _skillState.ActivateShield(
                skillData.GetEffectValue(level),
                skillData.Duration,
                Time.time);
            return true;
        }
    }

    public sealed class FrenzySkillEffect : ISkillEffect
    {
        private readonly PlayerSkillState _skillState;

        public FrenzySkillEffect(PlayerSkillState skillState)
        {
            _skillState = skillState;
        }

        public SkillType SkillType => SkillType.Frenzy;

        public bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            if (player == null)
            {
                return false;
            }

            _skillState.ActivateFrenzy(
                skillData.GetEffectValue(level),
                skillData.Duration,
                Time.time);
            return true;
        }
    }
}
