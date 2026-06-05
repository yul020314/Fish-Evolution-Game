using UnityEngine;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "FishData", menuName = "Fish Evolution/Config/Fish Data")]
    public sealed class FishDataSO : ScriptableObject
    {
        [SerializeField] private string _fishId = string.Empty;
        [SerializeField] private string _fishName = string.Empty;
        [SerializeField] private FishRarity _rarity = FishRarity.Common;
        [SerializeField] private int _hp = 100;
        [SerializeField] private int _attack = 10;
        [SerializeField] private float _speed = 6f;
        [SerializeField] private float _scale = 1f;
        [SerializeField] private int _unlockCost = 0;
        [SerializeField] private SkillType _skill = SkillType.None;

        public string FishId => _fishId;
        public string FishName => _fishName;
        public FishRarity Rarity => _rarity;
        public int HP => _hp;
        public int Attack => _attack;
        public float Speed => _speed;
        public float Scale => _scale;
        public int UnlockCost => _unlockCost;
        public SkillType Skill => _skill;
    }
}
