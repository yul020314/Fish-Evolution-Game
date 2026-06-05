using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Ads
{
    public sealed class AdManager : IStartable, IDisposable
    {
        private const string PlaybackFailedReason = "PlaybackFailed";

        private readonly IAdService _adService;
        private readonly ISubscriber<AdRequest> _adSubscriber;
        private readonly IPublisher<AdCompletedEvent> _completedPublisher;
        private readonly IPublisher<AdFailedEvent> _failedPublisher;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private IDisposable _adSubscription;
        private bool _isShowing;

        public AdManager(
            IAdService adService,
            ISubscriber<AdRequest> adSubscriber,
            IPublisher<AdCompletedEvent> completedPublisher,
            IPublisher<AdFailedEvent> failedPublisher)
        {
            _adService = adService;
            _adSubscriber = adSubscriber;
            _completedPublisher = completedPublisher;
            _failedPublisher = failedPublisher;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _adSubscription = _adSubscriber.Subscribe(HandleAdRequest);
        }

        public void Dispose()
        {
            _adSubscription?.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void HandleAdRequest(AdRequest request)
        {
            if (_isShowing)
            {
                return;
            }

            ShowAsync(request.AdType, _cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid ShowAsync(
            AdType adType,
            CancellationToken cancellationToken)
        {
            _isShowing = true;
            try
            {
                var completed = await _adService.ShowAsync(adType, cancellationToken);
                PublishResult(adType, completed);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _isShowing = false;
            }
        }

        private void PublishResult(
            AdType adType,
            bool completed)
        {
            if (completed)
            {
                _completedPublisher.Publish(new AdCompletedEvent(adType));
                return;
            }

            _failedPublisher.Publish(
                new AdFailedEvent(adType, PlaybackFailedReason));
        }
    }
}
