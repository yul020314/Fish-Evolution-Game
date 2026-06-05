using UnityEngine;

namespace FishEvolution.AI
{
    public sealed class PatrolState : IFishAIState
    {
        private float _elapsedTime;

        public FishState State => FishState.Patrol;

        public void Enter(FishAIContext context)
        {
            _elapsedTime = 0f;
            context.ResetPatrolTarget();
        }

        public void Tick(FishAIContext context, float deltaTime)
        {
            _elapsedTime += deltaTime;

            if (ShouldPickTarget(context))
            {
                PickTarget(context);
            }

            context.Controller.MoveToward(
                context.PatrolTarget,
                context.Controller.PatrolSpeed);
        }

        public void Exit(FishAIContext context)
        {
        }

        private bool ShouldPickTarget(FishAIContext context)
        {
            return !context.HasPatrolTarget ||
                context.Controller.IsNear(context.PatrolTarget) ||
                _elapsedTime >= context.Controller.PatrolRetargetInterval;
        }

        private void PickTarget(FishAIContext context)
        {
            var halfSize = context.PatrolSize * 0.5f;
            var min = context.PatrolCenter - halfSize;
            var max = context.PatrolCenter + halfSize;
            var x = Random.Range(min.x, max.x);
            var y = Random.Range(min.y, max.y);

            _elapsedTime = 0f;
            context.SetPatrolTarget(new Vector2(x, y));
        }
    }
}
