using MessagePipe;
using UnityEngine;
using VContainer;

namespace FishEvolution.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class CollisionSystem : MonoBehaviour
    {
        [SerializeField] private bool _canEat = true;
        [SerializeField] private bool _canBeEaten = true;

        private IPublisher<EatRequest> _eatPublisher;

        public bool CanEat => _canEat;
        public bool CanBeEaten => _canBeEaten;

        [Inject]
        public void Construct(IPublisher<EatRequest> eatPublisher)
        {
            _eatPublisher = eatPublisher;
        }

        public void Initialize(IPublisher<EatRequest> eatPublisher)
        {
            _eatPublisher = eatPublisher;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryPublishEatRequest(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryPublishEatRequest(collision.collider);
        }

        private void TryPublishEatRequest(Collider2D other)
        {
            if (!CanPublish(other))
            {
                return;
            }

            _eatPublisher.Publish(new EatRequest(gameObject, other.gameObject));
        }

        private bool CanPublish(Collider2D other)
        {
            if (!_canEat || _eatPublisher == null || other == null)
            {
                return false;
            }

            return other.TryGetComponent<CollisionSystem>(out var target) &&
                target.CanBeEaten;
        }
    }
}
