using FishEvolution.Config;

namespace FishEvolution.Map
{
    public sealed class MapRuntimeState
    {
        public MapRuntimeState(MapDataSO mapData)
        {
            MapData = mapData;
        }

        public MapDataSO MapData { get; }
        public MapAreaType AreaType => MapData.AreaType;
        public bool IsUnlocked { get; private set; }

        public void SetUnlocked(bool isUnlocked)
        {
            IsUnlocked = isUnlocked;
        }
    }
}
