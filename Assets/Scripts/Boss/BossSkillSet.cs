using System;
using UnityEngine;

namespace FishEvolution.Boss
{
    [Serializable]
    public sealed class BossSkillSet
    {
        [SerializeField] private int _skillDamage = 60;
        [SerializeField] private float _skillRange = 4f;
        [SerializeField] private float _skillCooldown = 4f;
        [SerializeField] private float _enragedCooldownMultiplier = 0.6f;

        public int SkillDamage => Mathf.Max(1, _skillDamage);
        public float SkillRange => Mathf.Max(0.1f, _skillRange);
        public float SkillCooldown => Mathf.Max(0.1f, _skillCooldown);
        public float EnragedCooldownMultiplier =>
            Mathf.Clamp(_enragedCooldownMultiplier, 0.1f, 1f);
    }
}
