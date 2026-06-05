using UnityEngine;

namespace FishEvolution.Combat
{
    public readonly struct EntityDeathEvent
    {
        public EntityDeathEvent(
            GameObject killer,
            HealthComponent health,
            int finalDamage)
        {
            Killer = killer;
            Health = health;
            FinalDamage = finalDamage;
        }

        public GameObject Killer { get; }
        public HealthComponent Health { get; }
        public GameObject Entity => Health != null ? Health.gameObject : null;
        public int FinalDamage { get; }
    }
}
