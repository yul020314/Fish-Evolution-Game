using System.Threading;
using Cysharp.Threading.Tasks;

namespace FishEvolution.Core
{
    public interface IGameStateMachine
    {
        GameState CurrentState { get; }

        UniTask ChangeStateAsync(GameState nextState, CancellationToken cancellationToken);
    }

    public sealed class GameStateMachine : IGameStateMachine
    {
        private readonly IEventBus _eventBus;

        public GameStateMachine(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public GameState CurrentState { get; private set; } = GameState.None;

        public UniTask ChangeStateAsync(GameState nextState, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (CurrentState == nextState)
            {
                return UniTask.CompletedTask;
            }

            var previousState = CurrentState;
            CurrentState = nextState;

            _eventBus.Publish(new GameStateChangedEvent(previousState, CurrentState));
            return UniTask.CompletedTask;
        }
    }
}
