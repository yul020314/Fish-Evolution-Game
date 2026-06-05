using FishEvolution.Config;

namespace FishEvolution.Map
{
    public readonly struct MapUnlockedEvent
    {
        public MapUnlockedEvent(MapDataSO mapData)
        {
            MapData = mapData;
        }

        public MapDataSO MapData { get; }
        public MapAreaType AreaType => MapData.AreaType;
    }
}
