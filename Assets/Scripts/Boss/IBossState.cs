namespace FishEvolution.Boss
{
    public interface IBossState
    {
        BossState State { get; }

        void Enter(BossContext context);

        void Tick(BossContext context, float deltaTime);

        void Exit(BossContext context);
    }
}
