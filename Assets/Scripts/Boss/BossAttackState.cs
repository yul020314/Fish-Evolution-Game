namespace FishEvolution.Boss
{
    public sealed class BossAttackState : IBossState
    {
        public BossState State => BossState.Attack;

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

            if (context.IsEnraged)
            {
                context.Boss.ChangeState(BossState.Enraged);
                return;
            }

            if (!context.IsPlayerInRange(context.Settings.AttackRange))
            {
                context.Boss.ChangeState(BossState.Chase);
                return;
            }

            context.FaceDirection(context.GetDirectionToPlayer());
            context.Boss.TryAttackPlayer();
            context.Skills.TryUseSkill(context, false);
        }

        public void Exit(BossContext context)
        {
        }
    }
}
