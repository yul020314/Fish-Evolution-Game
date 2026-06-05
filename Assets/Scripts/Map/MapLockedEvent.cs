using FishEvolution.Config;

namespace FishEvolution.Map
{
    public readonly struct MapLockedEvent
    {
        public MapLockedEvent(MapDataSO mapData)
        {
            MapData = mapData;
        }

        public MapDataSO MapData { get; }
        public MapAreaType AreaType => MapData.AreaType;
    }
}
