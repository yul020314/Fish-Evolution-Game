using System;
using FishEvolution.Config;
using UnityEngine;

namespace FishEvolution.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class FoodController : MonoBehaviour
    {
        [SerializeField] private FoodDataSO _foodData;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider2D;

        private Action<FoodController, PlayerController> _consumed;

        public FoodDataSO FoodData => _foodData;

        private void Awake()
        {
            CacheComponents();
            _collider2D.isTrigger = true;
        }

        private void Reset()
        {
            CacheComponents();
        }

        private void OnDisable()
        {
            _consumed = null;
        }

        public void Initialize(
            FoodDataSO foodData,
            Action<FoodController, PlayerController> consumed)
        {
            _foodData = foodData;
            _consumed = consumed;
            ApplyFoodData();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerController>(out var player))
            {
                return;
            }

            _consumed?.Invoke(this, player);
        }

        private void ApplyFoodData()
        {
            if (_foodData == null)
            {
                return;
            }

            gameObject.name = _foodData.FoodName;
            transform.localScale = Vector3.one * _foodData.Scale;
            _spriteRenderer.color = _foodData.Color;
        }

        private void CacheComponents()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_collider2D == null)
            {
                _collider2D = GetComponent<Collider2D>();
            }

            if (_collider2D == null)
            {
                _collider2D = gameObject.AddComponent<CircleCollider2D>();
            }
        }
    }
}
