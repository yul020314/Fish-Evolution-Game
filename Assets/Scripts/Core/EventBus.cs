using System;
using MessagePipe;

namespace FishEvolution.Core
{
    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message);

        IDisposable Subscribe<TMessage>(Action<TMessage> handler);
    }

    public sealed class EventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public EventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Publish<TMessage>(TMessage message)
        {
            var publisher = (IPublisher<TMessage>)_serviceProvider.GetService(typeof(IPublisher<TMessage>));
            publisher.Publish(message);
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
        {
            var subscriber = (ISubscriber<TMessage>)_serviceProvider.GetService(typeof(ISubscriber<TMessage>));
            return subscriber.Subscribe(new ActionMessageHandler<TMessage>(handler));
        }

        private sealed class ActionMessageHandler<TMessage> : IMessageHandler<TMessage>
        {
            private readonly Action<TMessage> _handler;

            public ActionMessageHandler(Action<TMessage> handler)
            {
                _handler = handler;
            }

            public void Handle(TMessage message)
            {
                _handler.Invoke(message);
            }
        }
    }
}
