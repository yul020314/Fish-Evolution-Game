using FishEvolution.Combat;
using FishEvolution.Gameplay;
using MessagePipe;
using UnityEngine;

namespace FishEvolution.Boss
{
    public sealed class BossSkillSystem
    {
        private readonly BossSkillSet _skillSet;
        private readonly IPublisher<DamageRequest> _damagePublisher;
        private float _nextSkillTime;

        public BossSkillSystem(
            BossSkillSet skillSet,
            IPublisher<DamageRequest> damagePublisher)
        {
            _skillSet = skillSet;
            _damagePublisher = damagePublisher;
        }

        public bool TryUseSkill(
            BossContext context,
            bool isEnraged)
        {
            if (!CanUseSkill(context))
            {
                return false;
            }

            var target = GetPlayerHealth(context.Player);
            if (target == null)
            {
                return false;
            }

            _nextSkillTime = Time.time + GetCooldown(isEnraged);
            _damagePublisher.Publish(
                new DamageRequest(
                    context.Boss.gameObject,
                    target,
                    GetDamage(isEnraged)));
            return true;
        }

        private bool CanUseSkill(BossContext context)
        {
            return _damagePublisher != null &&
                context.HasPlayer &&
                Time.time >= _nextSkillTime &&
                context.IsPlayerInRange(_skillSet.SkillRange);
        }

        private int GetDamage(bool isEnraged)
        {
            var multiplier = isEnraged ? 1.5f : 1f;
            return Mathf.RoundToInt(_skillSet.SkillDamage * multiplier);
        }

        private float GetCooldown(bool isEnraged)
        {
            if (!isEnraged)
            {
                return _skillSet.SkillCooldown;
            }

            return _skillSet.SkillCooldown *
                _skillSet.EnragedCooldownMultiplier;
        }

        private static HealthComponent GetPlayerHealth(PlayerController player)
        {
            if (player == null ||
                !player.TryGetComponent<HealthComponent>(out var health))
            {
                return null;
            }

            return health;
        }
    }
}
