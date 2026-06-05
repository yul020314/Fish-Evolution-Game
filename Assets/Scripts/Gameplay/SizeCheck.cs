using UnityEngine;

namespace FishEvolution.Gameplay
{
    public sealed class SizeCheck
    {
        private readonly float _requiredMultiplier;

        public SizeCheck(float requiredMultiplier)
        {
            _requiredMultiplier = Mathf.Max(1f, requiredMultiplier);
        }

        public bool CanEat(
            Transform eater,
            Transform target)
        {
            if (eater == null || target == null)
            {
                return false;
            }

            return GetSize(eater) > GetSize(target) * _requiredMultiplier;
        }

        public float GetSize(Transform target)
        {
            if (target == null)
            {
                return 0f;
            }

            var scale = target.lossyScale;
            return Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
        }
    }
}
