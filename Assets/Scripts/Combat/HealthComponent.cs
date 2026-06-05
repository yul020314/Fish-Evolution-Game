using UnityEngine;

namespace FishEvolution.Combat
{
    [DisallowMultipleComponent]
    public sealed class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _reviveDelay = 3f;
        [SerializeField] private bool _reviveFullHealth = true;

        private int _currentHealth;
        private bool _isDead;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;
        public float ReviveDelay => Mathf.Max(0f, _reviveDelay);
        public bool IsDead => _isDead;

        private void Awake()
        {
            Initialize(_maxHealth);
        }

        public void Initialize(int maxHealth)
        {
            _maxHealth = Mathf.Max(1, maxHealth);
            _currentHealth = _maxHealth;
            _isDead = false;
        }

        public int ApplyDamage(int damage)
        {
            if (_isDead || damage <= 0)
            {
                return 0;
            }

            var previousHealth = _currentHealth;
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            _isDead = _currentHealth <= 0;
            return previousHealth - _currentHealth;
        }

        public void Revive()
        {
            _isDead = false;
            _currentHealth = _reviveFullHealth ? _maxHealth : GetReviveHealth();
        }

        private int GetReviveHealth()
        {
            return Mathf.Clamp(_currentHealth, 1, _maxHealth);
        }
    }
}
