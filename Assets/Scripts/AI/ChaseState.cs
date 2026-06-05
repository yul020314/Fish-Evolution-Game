namespace FishEvolution.AI
{
    public sealed class ChaseState : IFishAIState
    {
        public FishState State => FishState.Chase;

        public void Enter(FishAIContext context)
        {
        }

        public void Tick(FishAIContext context, float deltaTime)
        {
            if (!context.Controller.TryGetDirectionToPlayer(out var direction))
            {
                context.Controller.Stop();
                return;
            }

            context.Controller.MoveInDirection(direction, context.Controller.ChaseSpeed);
        }

        public void Exit(FishAIContext context)
        {
        }
    }
}
