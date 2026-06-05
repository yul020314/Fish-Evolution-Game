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

        public string SkillId => _skillId;
        public string SkillName => _skillName;
        public SkillType SkillType => _skillType;
        public int MaxLevel => _maxLevel;
        public float Cooldown => _cooldown;
        public float CooldownGrowth => _cooldownGrowth;
        public float EffectValue => _effectValue;
        public float EffectGrowth => _effectGrowth;
    }
}
