using System;
using UnityEngine;

namespace FishEvolution.Boss
{
    [Serializable]
    public sealed class BossAISettings
    {
        [SerializeField] private float _detectionRadius = 10f;
        [SerializeField] private float _attackRange = 1.8f;
        [SerializeField] private float _moveSpeed = 2.6f;
        [SerializeField] private float _enragedMoveMultiplier = 1.35f;
        [SerializeField, Range(0.05f, 0.95f)] private float _enrageHealthPercent = 0.35f;

        public float DetectionRadius => Mathf.Max(0.1f, _detectionRadius);
        public float AttackRange => Mathf.Max(0.1f, _attackRange);
        public float MoveSpeed => Mathf.Max(0f, _moveSpeed);
        public float EnragedMoveMultiplier => Mathf.Max(1f, _enragedMoveMultiplier);
        public float EnrageHealthPercent => Mathf.Clamp01(_enrageHealthPercent);
    }
}
