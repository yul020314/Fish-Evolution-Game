using UnityEngine;

namespace FishEvolution.Combat
{
    public readonly struct DamageRequest
    {
        public DamageRequest(
            GameObject attacker,
            HealthComponent target,
            int damage)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
        }

        public GameObject Attacker { get; }
        public HealthComponent Target { get; }
        public int Damage { get; }
        public bool IsValid => Target != null && Damage > 0;
    }
}
