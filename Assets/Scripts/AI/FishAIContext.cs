using UnityEngine;

namespace FishEvolution.AI
{
    public sealed class FishAIContext
    {
        public FishAIContext(
            FishAIController controller,
            Transform transform)
        {
            Controller = controller;
            Transform = transform;
        }

        public FishAIController Controller { get; }
        public Transform Transform { get; }
        public Transform Player { get; private set; }
        public Vector2 PatrolCenter { get; private set; }
        public Vector2 PatrolSize { get; private set; }
        public Vector2 PatrolTarget { get; private set; }
        public bool HasPatrolTarget { get; private set; }

        public void SetPlayer(Transform player)
        {
            Player = player;
        }

        public void SetPatrolArea(Vector2 center, Vector2 size)
        {
            PatrolCenter = center;
            PatrolSize = size;
        }

        public void SetPatrolTarget(Vector2 target)
        {
            PatrolTarget = target;
            HasPatrolTarget = true;
        }

        public void ResetPatrolTarget()
        {
            HasPatrolTarget = false;
        }
    }
}
