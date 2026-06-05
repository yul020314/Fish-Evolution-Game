using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Combat
{
    public sealed class DeathSystem : IStartable, IDisposable
    {
        private readonly ISubscriber<EntityDeathEvent> _deathSubscriber;
        private readonly Dictionary<HealthComponent, DeathSnapshot> _deadEntities;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private IDisposable _deathSubscription;

        public DeathSystem(ISubscriber<EntityDeathEvent> deathSubscriber)
        {
            _deathSubscriber = deathSubscriber;
            _deadEntities = new Dictionary<HealthComponent, DeathSnapshot>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _deathSubscription = _deathSubscriber.Subscribe(HandleEntityDeath);
        }

        public void Dispose()
        {
            _deathSubscription?.Dispose();
            _deathSubscription = null;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _deadEntities.Clear();
        }

        private void HandleEntityDeath(EntityDeathEvent deathEvent)
        {
            var health = deathEvent.Health;
            if (health == null || _deadEntities.ContainsKey(health))
            {
                return;
            }

            var snapshot = DeathSnapshot.Capture(health.gameObject);
            _deadEntities.Add(health, snapshot);
            snapshot.SetAlive(false);
            ReviveAsync(
                health,
                snapshot,
                _cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid ReviveAsync(
            HealthComponent health,
            DeathSnapshot snapshot,
            CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(health.ReviveDelay),
                    cancellationToken: cancellationToken);
                Revive(health, snapshot);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void Revive(
            HealthComponent health,
            DeathSnapshot snapshot)
        {
            if (health == null)
            {
                return;
            }

            health.Revive();
            snapshot.SetAlive(true);
            _deadEntities.Remove(health);
        }
    }
}
