namespace FishEvolution.AI
{
    public sealed class AttackState : IFishAIState
    {
        public FishState State => FishState.Attack;

        public void Enter(FishAIContext context)
        {
            context.Controller.Stop();
        }

        public void Tick(FishAIContext context, float deltaTime)
        {
            if (context.Controller.TryGetDirectionToPlayer(out var direction))
            {
                context.Controller.FaceDirection(direction);
            }

            context.Controller.Stop();
        }

        public void Exit(FishAIContext context)
        {
        }
    }
}
