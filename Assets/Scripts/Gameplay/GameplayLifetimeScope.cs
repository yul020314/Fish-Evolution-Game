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
            builder.RegisterMessageBroker<FoodConsumedEvent>(options);
            builder.RegisterMessageBroker<PlayerLevelUpEvent>(options);
        }
    }
}
