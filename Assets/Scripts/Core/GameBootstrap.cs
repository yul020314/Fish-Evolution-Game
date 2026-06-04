using MessagePipe;
using UnityEngine;
using FishEvolution.Managers;
using VContainer;
using VContainer.Unity;

namespace FishEvolution.Core
{
    public sealed class GameBootstrap : LifetimeScope
    {
        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<GameStateChangedEvent>(options);

            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);
            builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);
            builder.Register<IConfigManager, ConfigManager>(Lifetime.Singleton);

            builder.RegisterEntryPoint<GameManager>(Lifetime.Singleton);
        }
    }
}
