using UnityEngine;

namespace FishEvolution.Gameplay
{
    public sealed class GrowthSystem
    {
        private readonly int _baseHp;
        private readonly int _hpPerLevel;
        private readonly int _baseAttack;
        private readonly int _attackPerLevel;
        private readonly float _baseSpeed;
        private readonly float _speedPerLevel;
        private readonly float _minSpeed;
        private readonly float _scaleBase;
        private readonly float _scalePerLevel;

        public GrowthSystem(
            int baseHp,
            int hpPerLevel,
            int baseAttack,
            int attackPerLevel,
            float baseSpeed,
            float speedPerLevel,
            float minSpeed,
            float scaleBase,
            float scalePerLevel)
        {
            _baseHp = baseHp;
            _hpPerLevel = hpPerLevel;
            _baseAttack = baseAttack;
            _attackPerLevel = attackPerLevel;
            _baseSpeed = baseSpeed;
            _speedPerLevel = speedPerLevel;
            _minSpeed = minSpeed;
            _scaleBase = scaleBase;
            _scalePerLevel = scalePerLevel;
        }

        public int GetHp(int level)
        {
            return _baseHp + Mathf.Max(1, level) * _hpPerLevel;
        }

        public int GetAttack(int level)
        {
            return _baseAttack + Mathf.Max(1, level) * _attackPerLevel;
        }

        public float GetSpeed(int level)
        {
            var speed = _baseSpeed - Mathf.Max(1, level) * _speedPerLevel;
            return Mathf.Max(_minSpeed, speed);
        }

        public float GetScaleMultiplier(int level)
        {
            return _scaleBase + Mathf.Max(1, level) * _scalePerLevel;
        }

        public void ApplyScale(Transform target, Vector3 baseScale, int level)
        {
            if (target == null)
            {
                return;
            }

            target.localScale = baseScale * GetScaleMultiplier(level);
        }
    }
}
