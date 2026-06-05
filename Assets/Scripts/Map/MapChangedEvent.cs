using FishEvolution.Config;

namespace FishEvolution.Map
{
    public readonly struct MapChangedEvent
    {
        public MapChangedEvent(
            MapAreaType previousArea,
            MapDataSO currentMap)
        {
            PreviousArea = previousArea;
            CurrentMap = currentMap;
        }

        public MapAreaType PreviousArea { get; }
        public MapDataSO CurrentMap { get; }
        public MapAreaType CurrentArea => CurrentMap.AreaType;
    }
}
