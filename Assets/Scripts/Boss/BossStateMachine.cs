using System.Collections.Generic;

namespace FishEvolution.Boss
{
    public sealed class BossStateMachine
    {
        private readonly Dictionary<BossState, IBossState> _states =
            new Dictionary<BossState, IBossState>();

        private IBossState _currentState;

        public BossState CurrentState =>
            _currentState != null ? _currentState.State : BossState.Idle;

        public void Register(IBossState state)
        {
            if (state == null)
            {
                return;
            }

            _states[state.State] = state;
        }

        public void ChangeState(BossState state, BossContext context)
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

        public void Tick(BossContext context, float deltaTime)
        {
            _currentState?.Tick(context, deltaTime);
        }
    }
}
