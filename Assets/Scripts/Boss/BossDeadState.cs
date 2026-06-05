namespace FishEvolution.Boss
{
    public sealed class BossDeadState : IBossState
    {
        public BossState State => BossState.Dead;

        public void Enter(BossContext context)
        {
            context.Stop();
        }

        public void Tick(BossContext context, float deltaTime)
        {
        }

        public void Exit(BossContext context)
        {
        }
    }
}
