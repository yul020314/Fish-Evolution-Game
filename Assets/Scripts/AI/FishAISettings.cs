using System;
using UnityEngine;

namespace FishEvolution.AI
{
    [Serializable]
    public sealed class FishAISettings
    {
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _escapeDistance = 3f;
        [SerializeField] private float _attackDistance = 1.2f;
        [SerializeField] private float _sizeAdvantageMultiplier = 1.1f;
        [SerializeField] private float _patrolSpeedMultiplier = 0.45f;
        [SerializeField] private float _chaseSpeedMultiplier = 0.75f;
        [SerializeField] private float _escapeSpeedMultiplier = 0.9f;
        [SerializeField] private float _patrolTargetTolerance = 0.35f;
        [SerializeField] private float _patrolRetargetInterval = 2.5f;
        [SerializeField] private Vector2 _patrolCenter = Vector2.zero;
        [SerializeField] private Vector2 _patrolSize = new Vector2(6f, 4f);

        public float DetectionRadius => Mathf.Max(0.1f, _detectionRadius);
        public float EscapeDistance => Mathf.Max(0.1f, _escapeDistance);
        public float AttackDistance => Mathf.Max(0.1f, _attackDistance);
        public float SizeAdvantageMultiplier => Mathf.Max(0.1f, _sizeAdvantageMultiplier);
        public float PatrolSpeedMultiplier => Mathf.Max(0f, _patrolSpeedMultiplier);
        public float ChaseSpeedMultiplier => Mathf.Max(0f, _chaseSpeedMultiplier);
        public float EscapeSpeedMultiplier => Mathf.Max(0f, _escapeSpeedMultiplier);
        public float PatrolTargetTolerance => Mathf.Max(0.01f, _patrolTargetTolerance);
        public float PatrolRetargetInterval => Mathf.Max(0.2f, _patrolRetargetInterval);
        public Vector2 PatrolCenter => _patrolCenter;
        public Vector2 PatrolSize => _patrolSize;

        public void SetPatrolArea(Vector2 center, Vector2 size)
        {
            _patrolCenter = center;
            _patrolSize = size;
        }
    }
}
