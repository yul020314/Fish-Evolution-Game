using System;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace FishEvolution.Gameplay
{
    public sealed class EatSystem : IStartable, IDisposable
    {
        private readonly ISubscriber<EatRequest> _eatSubscriber;
        private readonly IPublisher<EatCompletedEvent> _eatCompletedPublisher;
        private readonly SizeCheck _sizeCheck;

        private IDisposable _eatSubscription;

        public EatSystem(
            ISubscriber<EatRequest> eatSubscriber,
            IPublisher<EatCompletedEvent> eatCompletedPublisher,
            SizeCheck sizeCheck)
        {
            _eatSubscriber = eatSubscriber;
            _eatCompletedPublisher = eatCompletedPublisher;
            _sizeCheck = sizeCheck;
        }

        public void Start()
        {
            _eatSubscription = _eatSubscriber.Subscribe(HandleEatRequest);
        }

        public void Dispose()
        {
            _eatSubscription?.Dispose();
            _eatSubscription = null;
        }

        private void HandleEatRequest(EatRequest request)
        {
            if (!CanProcess(request))
            {
                return;
            }

            var eaterScale = _sizeCheck.GetSize(request.Eater.transform);
            var targetScale = _sizeCheck.GetSize(request.Target.transform);
            ConsumeTarget(request.Target);
            _eatCompletedPublisher.Publish(
                new EatCompletedEvent(
                    request.Eater,
                    request.Target,
                    eaterScale,
                    targetScale));
        }

        private bool CanProcess(EatRequest request)
        {
            return request.IsValid &&
                request.Target.activeInHierarchy &&
                _sizeCheck.CanEat(
                    request.Eater.transform,
                    request.Target.transform);
        }

        private void ConsumeTarget(GameObject target)
        {
            if (target != null && target.activeSelf)
            {
                target.SetActive(false);
            }
        }
    }
}
