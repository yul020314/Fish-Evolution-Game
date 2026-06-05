namespace FishEvolution.Gameplay
{
    public readonly struct PlayerLevelUpEvent
    {
        public PlayerLevelUpEvent(
            PlayerController player,
            int previousLevel,
            int currentLevel)
        {
            Player = player;
            PreviousLevel = previousLevel;
            CurrentLevel = currentLevel;
        }

        public PlayerController Player { get; }
        public int PreviousLevel { get; }
        public int CurrentLevel { get; }
    }
}
