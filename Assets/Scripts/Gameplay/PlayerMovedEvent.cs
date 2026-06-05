using UnityEngine;

namespace FishEvolution.Gameplay
{
    public readonly struct PlayerMovedEvent
    {
        public PlayerMovedEvent(
            PlayerController player,
            Vector2 direction)
        {
            Player = player;
            Direction = direction;
        }

        public PlayerController Player { get; }
        public Vector2 Direction { get; }
    }
}
