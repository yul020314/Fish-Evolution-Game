using UnityEngine;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "MapData", menuName = "Fish Evolution/Config/Map Data")]
    public sealed class MapDataSO : ScriptableObject
    {
        [SerializeField] private string _mapId = string.Empty;
        [SerializeField] private string _mapName = string.Empty;
        [SerializeField] private MapAreaType _areaType = MapAreaType.ShallowSea;
        [SerializeField] private int _unlockLevel = 1;
        [SerializeField] private int _minFoodLevel = 1;
        [SerializeField] private int _maxFoodLevel = 5;
        [SerializeField] private string _bossId = string.Empty;

        public string MapId => _mapId;
        public string MapName => _mapName;
        public MapAreaType AreaType => _areaType;
        public int UnlockLevel => _unlockLevel;
        public int MinFoodLevel => _minFoodLevel;
        public int MaxFoodLevel => _maxFoodLevel;
        public string BossId => _bossId;
    }
}
