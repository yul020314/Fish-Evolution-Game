namespace FishEvolution.AI
{
    public sealed class EscapeState : IFishAIState
    {
        public FishState State => FishState.Escape;

        public void Enter(FishAIContext context)
        {
        }

        public void Tick(FishAIContext context, float deltaTime)
        {
            if (!context.Controller.TryGetDirectionFromPlayer(out var direction))
            {
                context.Controller.Stop();
                return;
            }

            context.Controller.MoveInDirection(direction, context.Controller.EscapeSpeed);
        }

        public void Exit(FishAIContext context)
        {
        }
    }
}
