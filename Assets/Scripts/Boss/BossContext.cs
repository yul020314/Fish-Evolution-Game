using FishEvolution.Combat;
using FishEvolution.Gameplay;
using UnityEngine;

namespace FishEvolution.Boss
{
    public sealed class BossContext
    {
        public BossContext(
            BossController boss,
            Transform transform,
            Rigidbody2D rigidbody2D,
            HealthComponent health,
            AttackComponent attack,
            BossAISettings settings,
            BossSkillSystem skills)
        {
            Boss = boss;
            Transform = transform;
            Rigidbody2D = rigidbody2D;
            Health = health;
            Attack = attack;
            Settings = settings;
            Skills = skills;
        }

        public BossController Boss { get; }
        public Transform Transform { get; }
        public Rigidbody2D Rigidbody2D { get; }
        public HealthComponent Health { get; }
        public AttackComponent Attack { get; }
        public BossAISettings Settings { get; }
        public BossSkillSystem Skills { get; }
        public PlayerController Player { get; private set; }

        public bool HasPlayer => Player != null;
        public bool IsDead => Health != null && Health.IsDead;
        public bool IsEnraged => Health != null &&
            Health.CurrentHealth <= Health.MaxHealth * Settings.EnrageHealthPercent;

        public void SetPlayer(PlayerController player)
        {
            Player = player;
        }

        public bool IsPlayerInRange(float range)
        {
            if (!HasPlayer)
            {
                return false;
            }

            var sqrDistance = GetSqrDistanceToPlayer();
            return sqrDistance <= range * range;
        }

        public Vector2 GetDirectionToPlayer()
        {
            if (!HasPlayer)
            {
                return Vector2.zero;
            }

            return ((Vector2)Player.transform.position -
                (Vector2)Transform.position).normalized;
        }

        public float GetSqrDistanceToPlayer()
        {
            if (!HasPlayer)
            {
                return float.MaxValue;
            }

            return (Player.transform.position - Transform.position).sqrMagnitude;
        }

        public void MoveTowardPlayer(float speed)
        {
            var direction = GetDirectionToPlayer();
            Rigidbody2D.linearVelocity = direction * speed;
            FaceDirection(direction);
        }

        public void Stop()
        {
            Rigidbody2D.linearVelocity = Vector2.zero;
        }

        public void FaceDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
