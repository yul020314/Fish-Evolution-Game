namespace FishEvolution.AI
{
    public interface IFishAIState
    {
        FishState State { get; }

        void Enter(FishAIContext context);

        void Tick(FishAIContext context, float deltaTime);

        void Exit(FishAIContext context);
    }
}
