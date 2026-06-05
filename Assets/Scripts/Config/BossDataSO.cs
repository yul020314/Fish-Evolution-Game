using UnityEngine;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "BossData", menuName = "Fish Evolution/Config/Boss Data")]
    public sealed class BossDataSO : ScriptableObject
    {
        [SerializeField] private string _bossId = string.Empty;
        [SerializeField] private string _bossName = string.Empty;
        [SerializeField] private MapAreaType _areaType = MapAreaType.ShallowSea;
        [SerializeField] private int _hp = 5000;
        [SerializeField] private int _attack = 100;
        [SerializeField] private int _rewardGold = 1000;
        [SerializeField] private int _rewardDiamond = 20;

        public string BossId => _bossId;
        public string BossName => _bossName;
        public MapAreaType AreaType => _areaType;
        public int HP => _hp;
        public int Attack => _attack;
        public int RewardGold => _rewardGold;
        public int RewardDiamond => _rewardDiamond;
    }
}
