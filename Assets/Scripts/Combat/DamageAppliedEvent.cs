using UnityEngine;

namespace FishEvolution.Combat
{
    public readonly struct DamageAppliedEvent
    {
        public DamageAppliedEvent(
            GameObject attacker,
            HealthComponent target,
            int damage,
            int currentHealth,
            int maxHealth,
            bool isFatal)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            IsFatal = isFatal;
        }

        public GameObject Attacker { get; }
        public HealthComponent Target { get; }
        public int Damage { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public bool IsFatal { get; }
    }
}
