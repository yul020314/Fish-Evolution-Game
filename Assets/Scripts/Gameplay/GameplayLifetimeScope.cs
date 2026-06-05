using FishEvolution.Combat;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace FishEvolution.Gameplay
{
    public sealed class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterComponentInHierarchy<PlayerController>();
            builder.RegisterComponentInHierarchy<FishSpawner>();
            builder.RegisterMessageBroker<FoodConsumedEvent>(options);
            builder.RegisterMessageBroker<PlayerLevelUpEvent>(options);
            builder.RegisterMessageBroker<DamageRequest>(options);
            builder.RegisterMessageBroker<DamageAppliedEvent>(options);
            builder.RegisterMessageBroker<EntityDeathEvent>(options);
            builder.RegisterMessageBroker<EatRequest>(options);
            builder.RegisterMessageBroker<EatCompletedEvent>(options);

            builder.Register<SizeCheck>(Lifetime.Singleton)
                .WithParameter(1.1f);
            builder.RegisterEntryPoint<DamageSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<DeathSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<EatSystem>(Lifetime.Singleton);
        }
    }
}
