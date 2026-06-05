using UnityEngine;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "Fish Evolution/Config/Skill Data")]
    public sealed class SkillDataSO : ScriptableObject
    {
        [SerializeField] private string _skillId = string.Empty;
        [SerializeField] private string _skillName = string.Empty;
        [SerializeField] private SkillType _skillType = SkillType.None;
        [SerializeField] private int _maxLevel = 1;
        [SerializeField] private float _cooldown = 1f;
        [SerializeField] private float _cooldownGrowth = -0.1f;
        [SerializeField] private float _effectValue = 1f;
        [SerializeField] private float _effectGrowth = 0.1f;
        [SerializeField] private float _duration = 0f;
        [SerializeField] private float _range = 0f;

        public string SkillId => _skillId;
        public string SkillName => _skillName;
        public SkillType SkillType => _skillType;
        public int MaxLevel => _maxLevel;
        public float Cooldown => _cooldown;
        public float CooldownGrowth => _cooldownGrowth;
        public float EffectValue => _effectValue;
        public float EffectGrowth => _effectGrowth;
        public float Duration => _duration;
        public float Range => _range;

        public float GetCooldown(int level)
        {
            var value = _cooldown + _cooldownGrowth * GetGrowthStep(level);
            return Mathf.Max(0.05f, value);
        }

        public float GetEffectValue(int level)
        {
            var value = _effectValue + _effectGrowth * GetGrowthStep(level);
            return Mathf.Max(0f, value);
        }

        private int GetGrowthStep(int level)
        {
            return Mathf.Clamp(level, 1, Mathf.Max(1, _maxLevel)) - 1;
        }
    }
}
