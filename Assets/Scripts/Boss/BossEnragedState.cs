namespace FishEvolution.Boss
{
    public sealed class BossEnragedState : IBossState
    {
        public BossState State => BossState.Enraged;

        public void Enter(BossContext context)
        {
            context.Attack.SetDamageMultiplier(1.5f);
        }

        public void Tick(BossContext context, float deltaTime)
        {
            if (context.IsDead)
            {
                context.Boss.ChangeState(BossState.Dead);
                return;
            }

            if (!context.IsPlayerInRange(context.Settings.DetectionRadius))
            {
                context.Boss.ChangeState(BossState.Idle);
                return;
            }

            if (context.IsPlayerInRange(context.Settings.AttackRange))
            {
                context.Stop();
                context.Boss.TryAttackPlayer();
                context.Skills.TryUseSkill(context, true);
                return;
            }

            var speed = context.Settings.MoveSpeed *
                context.Settings.EnragedMoveMultiplier;
            context.MoveTowardPlayer(speed);
        }

        public void Exit(BossContext context)
        {
        }
    }
}
