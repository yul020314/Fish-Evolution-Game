using MessagePipe;
using UnityEngine;
using VContainer;

namespace FishEvolution.Combat
{
    [DisallowMultipleComponent]
    public sealed class AttackComponent : MonoBehaviour
    {
        [SerializeField] private int _attackDamage = 10;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _attackRange = 1.2f;

        private IPublisher<DamageRequest> _damagePublisher;
        private HealthComponent _selfHealth;
        private float _nextAttackTime;

        public int AttackDamage => Mathf.Max(0, _attackDamage);
        public float AttackCooldown => Mathf.Max(0.01f, _attackCooldown);
        public float AttackRange => Mathf.Max(0.01f, _attackRange);

        [Inject]
        public void Construct(IPublisher<DamageRequest> damagePublisher)
        {
            _damagePublisher = damagePublisher;
        }

        private void Awake()
        {
            CacheComponents();
        }

        public void Initialize(int attackDamage)
        {
            _attackDamage = Mathf.Max(0, attackDamage);
            _nextAttackTime = 0f;
        }

        public void Initialize(
            int attackDamage,
            IPublisher<DamageRequest> damagePublisher)
        {
            Initialize(attackDamage);
            _damagePublisher = damagePublisher;
        }

        public bool TryAttack(HealthComponent target)
        {
            if (!CanAttack(target))
            {
                return false;
            }

            _nextAttackTime = Time.time + AttackCooldown;
            _damagePublisher.Publish(
                new DamageRequest(gameObject, target, AttackDamage));
            return true;
        }

        public bool CanAttack(HealthComponent target)
        {
            return _damagePublisher != null &&
                target != null &&
                !target.IsDead &&
                !IsSelfDead() &&
                IsCooldownReady() &&
                IsTargetInRange(target.transform);
        }

        private bool IsSelfDead()
        {
            return _selfHealth != null && _selfHealth.IsDead;
        }

        private bool IsCooldownReady()
        {
            return Time.time >= _nextAttackTime;
        }

        private bool IsTargetInRange(Transform target)
        {
            var range = AttackRange;
            var sqrDistance = (target.position - transform.position).sqrMagnitude;
            return sqrDistance <= range * range;
        }

        private void CacheComponents()
        {
            if (_selfHealth == null)
            {
                _selfHealth = GetComponent<HealthComponent>();
            }
        }
    }
}
