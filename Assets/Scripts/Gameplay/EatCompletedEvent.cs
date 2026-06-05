using UnityEngine;

namespace FishEvolution.Gameplay
{
    public readonly struct EatCompletedEvent
    {
        public EatCompletedEvent(
            GameObject eater,
            GameObject target,
            float eaterScale,
            float targetScale)
        {
            Eater = eater;
            Target = target;
            EaterScale = eaterScale;
            TargetScale = targetScale;
        }

        public GameObject Eater { get; }
        public GameObject Target { get; }
        public float EaterScale { get; }
        public float TargetScale { get; }
    }
}
