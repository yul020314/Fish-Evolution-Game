namespace FishEvolution.Boss
{
    public sealed class BossIdleState : IBossState
    {
        public BossState State => BossState.Idle;

        public void Enter(BossContext context)
        {
            context.Stop();
        }

        public void Tick(BossContext context, float deltaTime)
        {
            if (context.IsDead)
            {
                context.Boss.ChangeState(BossState.Dead);
                return;
            }

            if (context.IsPlayerInRange(context.Settings.DetectionRadius))
            {
                context.Boss.ChangeState(BossState.Chase);
            }
        }

        public void Exit(BossContext context)
        {
        }
    }
}
