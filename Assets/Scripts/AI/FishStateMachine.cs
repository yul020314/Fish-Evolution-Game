using System.Collections.Generic;

namespace FishEvolution.AI
{
    public sealed class FishStateMachine
    {
        private readonly Dictionary<FishState, IFishAIState> _states =
            new Dictionary<FishState, IFishAIState>();

        private IFishAIState _currentState;

        public FishState CurrentState =>
            _currentState != null ? _currentState.State : FishState.Patrol;

        public void Register(IFishAIState state)
        {
            if (state == null)
            {
                return;
            }

            _states[state.State] = state;
        }

        public void ChangeState(FishState state, FishAIContext context)
        {
            if (_currentState != null && _currentState.State == state)
            {
                return;
            }

            if (!_states.TryGetValue(state, out var nextState))
            {
                return;
            }

            _currentState?.Exit(context);
            _currentState = nextState;
            _currentState.Enter(context);
        }

        public void Tick(FishAIContext context, float deltaTime)
        {
            _currentState?.Tick(context, deltaTime);
        }
    }
}
