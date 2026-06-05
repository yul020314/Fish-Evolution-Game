namespace FishEvolution.Boss
{
    public sealed class BossChaseState : IBossState
    {
        public BossState State => BossState.Chase;

        public void Enter(BossContext context)
        {
        }

        public void Tick(BossContext context, float deltaTime)
        {
            if (context.IsDead)
            {
                context.Boss.ChangeState(BossState.Dead);
                return;
            }

            if (context.IsEnraged)
            {
                context.Boss.ChangeState(BossState.Enraged);
                return;
            }

            if (!context.IsPlayerInRange(context.Settings.DetectionRadius))
            {
                context.Boss.ChangeState(BossState.Idle);
                return;
            }

            if (context.IsPlayerInRange(context.Settings.AttackRange))
            {
                context.Boss.ChangeState(BossState.Attack);
                return;
            }

            context.MoveTowardPlayer(context.Settings.MoveSpeed);
        }

        public void Exit(BossContext context)
        {
            context.Stop();
        }
    }
}
