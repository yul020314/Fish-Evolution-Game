namespace FishEvolution.Core
{
    public readonly struct GameStateChangedEvent
    {
        public GameStateChangedEvent(GameState previousState, GameState currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }

        public GameState PreviousState { get; }
        public GameState CurrentState { get; }
    }
}
