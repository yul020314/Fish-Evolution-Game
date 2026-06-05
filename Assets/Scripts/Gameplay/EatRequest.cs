using UnityEngine;

namespace FishEvolution.Gameplay
{
    public readonly struct EatRequest
    {
        public EatRequest(GameObject eater, GameObject target)
        {
            Eater = eater;
            Target = target;
        }

        public GameObject Eater { get; }
        public GameObject Target { get; }
        public bool IsValid => Eater != null && Target != null && Eater != Target;
    }
}
