using FishEvolution.Config;
using FishEvolution.Gameplay;

namespace FishEvolution.Map
{
    public readonly struct MapChangeRequest
    {
        public MapChangeRequest(
            MapAreaType areaType,
            PlayerController player)
        {
            AreaType = areaType;
            Player = player;
        }

        public MapAreaType AreaType { get; }
        public PlayerController Player { get; }
        public bool IsValid => Player != null;
    }
}
