using UnityEngine;

namespace FishEvolution.Combat
{
    internal sealed class DeathSnapshot
    {
        private readonly Collider2D[] _colliders;
        private readonly bool[] _colliderStates;
        private readonly Renderer[] _renderers;
        private readonly bool[] _rendererStates;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly bool _rigidbodySimulated;

        private DeathSnapshot(
            Collider2D[] colliders,
            Renderer[] renderers,
            Rigidbody2D rigidbody2D)
        {
            _colliders = colliders;
            _renderers = renderers;
            _rigidbody2D = rigidbody2D;
            _colliderStates = CaptureColliderStates(colliders);
            _rendererStates = CaptureRendererStates(renderers);
            _rigidbodySimulated = rigidbody2D == null || rigidbody2D.simulated;
        }

        public static DeathSnapshot Capture(GameObject entity)
        {
            return new DeathSnapshot(
                entity.GetComponentsInChildren<Collider2D>(true),
                entity.GetComponentsInChildren<Renderer>(true),
                entity.GetComponent<Rigidbody2D>());
        }

        public void SetAlive(bool isAlive)
        {
            SetColliderStates(_colliders, _colliderStates, isAlive);
            SetRendererStates(_renderers, _rendererStates, isAlive);
            SetRigidbodyAlive(isAlive);
        }

        private void SetRigidbodyAlive(bool isAlive)
        {
            if (_rigidbody2D == null)
            {
                return;
            }

            if (!isAlive)
            {
                _rigidbody2D.linearVelocity = Vector2.zero;
            }

            _rigidbody2D.simulated = isAlive && _rigidbodySimulated;
        }

        private static bool[] CaptureColliderStates(Collider2D[] colliders)
        {
            var states = new bool[colliders.Length];
            for (var index = 0; index < colliders.Length; index++)
            {
                states[index] = colliders[index].enabled;
            }

            return states;
        }

        private static bool[] CaptureRendererStates(Renderer[] renderers)
        {
            var states = new bool[renderers.Length];
            for (var index = 0; index < renderers.Length; index++)
            {
                states[index] = renderers[index].enabled;
            }

            return states;
        }

        private static void SetColliderStates(Collider2D[] colliders, bool[] states, bool isAlive)
        {
            for (var index = 0; index < colliders.Length; index++)
            {
                colliders[index].enabled = isAlive && states[index];
            }
        }

        private static void SetRendererStates(Renderer[] renderers, bool[] states, bool isAlive)
        {
            for (var index = 0; index < renderers.Length; index++)
            {
                renderers[index].enabled = isAlive && states[index];
            }
        }
    }
}
