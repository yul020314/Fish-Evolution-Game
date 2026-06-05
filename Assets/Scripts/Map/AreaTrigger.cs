using FishEvolution.Config;
using FishEvolution.Gameplay;
using UnityEngine;

namespace FishEvolution.Map
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class AreaTrigger : MonoBehaviour
    {
        [SerializeField] private MapAreaType _targetArea = MapAreaType.ShallowSea;
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private MapManager _mapManager;

        private void Awake()
        {
            CacheComponents();
            _collider2D.isTrigger = true;
        }

        private void Reset()
        {
            CacheComponents();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_mapManager == null ||
                !other.TryGetComponent<PlayerController>(out var player))
            {
                return;
            }

            _mapManager.TryRequestAreaChange(_targetArea, player);
        }

        private void CacheComponents()
        {
            if (_collider2D == null)
            {
                _collider2D = GetComponent<Collider2D>();
            }
        }
    }
}
