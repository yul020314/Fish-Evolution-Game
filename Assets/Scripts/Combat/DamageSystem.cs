using System;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Combat
{
    public sealed class DamageSystem : IStartable, IDisposable
    {
        private readonly ISubscriber<DamageRequest> _damageSubscriber;
        private readonly IPublisher<DamageAppliedEvent> _damagePublisher;
        private readonly IPublisher<EntityDeathEvent> _deathPublisher;

        private IDisposable _damageSubscription;

        public DamageSystem(
            ISubscriber<DamageRequest> damageSubscriber,
            IPublisher<DamageAppliedEvent> damagePublisher,
            IPublisher<EntityDeathEvent> deathPublisher)
        {
            _damageSubscriber = damageSubscriber;
            _damagePublisher = damagePublisher;
            _deathPublisher = deathPublisher;
        }

        public void Start()
        {
            _damageSubscription = _damageSubscriber.Subscribe(HandleDamageRequest);
        }

        public void Dispose()
        {
            _damageSubscription?.Dispose();
            _damageSubscription = null;
        }

        private void HandleDamageRequest(DamageRequest request)
        {
            if (!request.IsValid || request.Target.IsDead)
            {
                return;
            }

            var appliedDamage = request.Target.ApplyDamage(request.Damage);
            if (appliedDamage <= 0)
            {
                return;
            }

            PublishDamageApplied(request, appliedDamage);
            PublishDeathIfNeeded(request, appliedDamage);
        }

        private void PublishDamageApplied(
            DamageRequest request,
            int appliedDamage)
        {
            _damagePublisher.Publish(
                new DamageAppliedEvent(
                    request.Attacker,
                    request.Target,
                    appliedDamage,
                    request.Target.CurrentHealth,
                    request.Target.MaxHealth,
                    request.Target.IsDead));
        }

        private void PublishDeathIfNeeded(
            DamageRequest request,
            int appliedDamage)
        {
            if (!request.Target.IsDead)
            {
                return;
            }

            _deathPublisher.Publish(
                new EntityDeathEvent(
                    request.Attacker,
                    request.Target,
                    appliedDamage));
        }
    }
}
