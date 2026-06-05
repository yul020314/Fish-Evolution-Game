using System;
using FishEvolution.Combat;
using MessagePipe;
using VContainer.Unity;
using UnityEngine;

namespace FishEvolution.Gameplay
{
    public sealed class SizeCheck
    {
        private readonly float _requiredMultiplier;

        public SizeCheck(float requiredMultiplier)
        {
            _requiredMultiplier = Mathf.Max(1f, requiredMultiplier);
        }

        public bool CanEat(
            Transform eater,
            Transform target)
        {
            if (eater == null || target == null)
            {
                return false;
            }

            return GetSize(eater) > GetSize(target) * _requiredMultiplier;
        }

        public float GetSize(Transform target)
        {
            if (target == null)
            {
                return 0f;
            }

            var scale = target.lossyScale;
            return Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
        }
    }

    public enum BuffType
    {
        Speed = 1,
        Shield = 2,
        Exp = 3,
        Frenzy = 4
    }

    public readonly struct BuffRequest
    {
        public BuffRequest(
            PlayerController target,
            BuffType buffType,
            float value,
            float duration)
        {
            Target = target;
            BuffType = buffType;
            Value = value;
            Duration = duration;
        }

        public PlayerController Target { get; }
        public BuffType BuffType { get; }
        public float Value { get; }
        public float Duration { get; }
        public bool IsValid => Target != null && Duration > 0f;
    }

    public readonly struct BuffAppliedEvent
    {
        public BuffAppliedEvent(
            PlayerController target,
            BuffType buffType,
            int stackCount,
            float duration)
        {
            Target = target;
            BuffType = buffType;
            StackCount = stackCount;
            Duration = duration;
        }

        public PlayerController Target { get; }
        public BuffType BuffType { get; }
        public int StackCount { get; }
        public float Duration { get; }
    }

    public sealed class BuffManager
    {
        private const int MaxBuffs = 8;
        private const int MaxStacks = 3;

        private readonly SpeedBuff _speedBuff;
        private readonly ShieldBuff _shieldBuff;
        private readonly ExpBuff _expBuff;
        private readonly ActiveBuff[] _buffs = new ActiveBuff[MaxBuffs];

        private PlayerController _player;
        private HealthComponent _playerHealth;
        private AttackComponent _playerAttack;

        public BuffManager(
            SpeedBuff speedBuff,
            ShieldBuff shieldBuff,
            ExpBuff expBuff)
        {
            _speedBuff = speedBuff;
            _shieldBuff = shieldBuff;
            _expBuff = expBuff;
        }

        public void RegisterPlayer(PlayerController player)
        {
            if (player == null)
            {
                return;
            }

            _player = player;
            CachePlayerComponents();
            ApplyFrenzyMultiplier();
        }

        public int AddBuff(BuffRequest request)
        {
            if (!request.IsValid || _player == null || request.Target != _player)
            {
                return 0;
            }

            var buff = GetOrCreateBuff(request.BuffType);
            if (buff == null)
            {
                return 0;
            }

            buff.Apply(request.Value, request.Duration, Time.time, MaxStacks);
            ApplyImmediateEffect(buff);
            RefreshMovementIfNeeded(buff.BuffType);
            return buff.StackCount;
        }

        public void Tick(float time)
        {
            for (var index = 0; index < _buffs.Length; index++)
            {
                TickBuff(_buffs[index], time);
            }
        }

        public float GetSpeedMultiplier(PlayerController target)
        {
            if (target != _player)
            {
                return 1f;
            }

            return _speedBuff.GetMultiplier(GetBuff(BuffType.Speed));
        }

        public float GetExperienceMultiplier(PlayerController target)
        {
            if (target != _player)
            {
                return 1f;
            }

            return _expBuff.GetMultiplier(GetBuff(BuffType.Exp));
        }

        public int GetDamageAfterShield(
            HealthComponent target,
            int damage)
        {
            if (damage <= 0 || target != _playerHealth)
            {
                return damage;
            }

            return _shieldBuff.GetDamageAfterShield(GetBuff(BuffType.Shield), damage);
        }

        private void TickBuff(ActiveBuff buff, float time)
        {
            if (buff == null || !buff.IsActive)
            {
                return;
            }

            if (buff.IsExpired(time))
            {
                var buffType = buff.BuffType;
                buff.Clear();
                ApplyExpiredEffect(buffType);
            }
        }

        private float GetStackedValue(BuffType buffType)
        {
            var buff = GetBuff(buffType);
            return buff != null && buff.IsActive ? buff.StackedValue : 0f;
        }

        private void ApplyImmediateEffect(ActiveBuff buff)
        {
            if (buff.BuffType == BuffType.Frenzy)
            {
                ApplyFrenzyMultiplier();
            }
        }

        private void ApplyExpiredEffect(BuffType buffType)
        {
            if (buffType == BuffType.Frenzy)
            {
                ApplyFrenzyMultiplier();
                return;
            }

            RefreshMovementIfNeeded(buffType);
        }

        private void RefreshMovementIfNeeded(BuffType buffType)
        {
            if (buffType == BuffType.Speed)
            {
                _player?.RefreshMovement();
            }
        }

        private void ApplyFrenzyMultiplier()
        {
            var multiplier = 1f + GetStackedValue(BuffType.Frenzy);
            _playerAttack?.SetDamageMultiplier(multiplier);
        }

        private ActiveBuff GetOrCreateBuff(BuffType buffType)
        {
            var buff = GetBuff(buffType);
            if (buff != null)
            {
                return buff;
            }

            return CreateBuff(buffType);
        }

        private ActiveBuff GetBuff(BuffType buffType)
        {
            for (var index = 0; index < _buffs.Length; index++)
            {
                if (_buffs[index] != null && _buffs[index].BuffType == buffType)
                {
                    return _buffs[index];
                }
            }

            return null;
        }

        private ActiveBuff CreateBuff(BuffType buffType)
        {
            for (var index = 0; index < _buffs.Length; index++)
            {
                if (_buffs[index] == null)
                {
                    _buffs[index] = new ActiveBuff(buffType);
                    return _buffs[index];
                }
            }

            return null;
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

    public sealed class SpeedBuff
    {
        public float GetMultiplier(ActiveBuff buff)
        {
            return 1f + GetStackedValue(buff);
        }

        private float GetStackedValue(ActiveBuff buff)
        {
            return buff != null && buff.IsActive ? buff.StackedValue : 0f;
        }
    }

    public sealed class ShieldBuff
    {
        public int GetDamageAfterShield(
            ActiveBuff buff,
            int damage)
        {
            if (damage <= 0)
            {
                return damage;
            }

            var reduction = Mathf.Clamp01(GetStackedValue(buff));
            if (reduction <= 0f)
            {
                return damage;
            }

            return Mathf.Max(1, Mathf.CeilToInt(damage * (1f - reduction)));
        }

        private float GetStackedValue(ActiveBuff buff)
        {
            return buff != null && buff.IsActive ? buff.StackedValue : 0f;
        }
    }

    public sealed class ExpBuff
    {
        public float GetMultiplier(ActiveBuff buff)
        {
            return 1f + GetStackedValue(buff);
        }

        private float GetStackedValue(ActiveBuff buff)
        {
            return buff != null && buff.IsActive ? buff.StackedValue : 0f;
        }
    }

    public sealed class ActiveBuff
    {
        private float _value;
        private float _expiresAt;

        public ActiveBuff(BuffType buffType)
        {
            BuffType = buffType;
        }

        public BuffType BuffType { get; }
        public int StackCount { get; private set; }
        public bool IsActive => StackCount > 0;
        public float StackedValue => _value * StackCount;

        public void Apply(
            float value,
            float duration,
            float time,
            int maxStacks)
        {
            _value = Mathf.Max(0f, value);
            StackCount = Mathf.Clamp(StackCount + 1, 1, Mathf.Max(1, maxStacks));
            _expiresAt = time + Mathf.Max(0f, duration);
        }

        public bool IsExpired(float time)
        {
            return time >= _expiresAt;
        }

        public void Clear()
        {
            StackCount = 0;
            _value = 0f;
            _expiresAt = 0f;
        }
    }

    public sealed class BuffSystem : IStartable, ITickable, IDisposable
    {
        private readonly BuffManager _buffManager;
        private readonly ISubscriber<BuffRequest> _buffSubscriber;
        private readonly IPublisher<BuffAppliedEvent> _buffAppliedPublisher;

        private IDisposable _buffSubscription;

        public BuffSystem(
            BuffManager buffManager,
            ISubscriber<BuffRequest> buffSubscriber,
            IPublisher<BuffAppliedEvent> buffAppliedPublisher)
        {
            _buffManager = buffManager;
            _buffSubscriber = buffSubscriber;
            _buffAppliedPublisher = buffAppliedPublisher;
        }

        public void Start()
        {
            _buffSubscription = _buffSubscriber.Subscribe(HandleBuffRequest);
        }

        public void Tick()
        {
            _buffManager.Tick(Time.time);
        }

        public void Dispose()
        {
            _buffSubscription?.Dispose();
            _buffSubscription = null;
        }

        private void HandleBuffRequest(BuffRequest request)
        {
            if (!request.IsValid)
            {
                return;
            }

            var stackCount = _buffManager.AddBuff(request);
            if (stackCount <= 0)
            {
                return;
            }

            _buffAppliedPublisher.Publish(
                new BuffAppliedEvent(
                    request.Target,
                    request.BuffType,
                    stackCount,
                    request.Duration));
        }
    }
}
