using System;
using FishEvolution.Combat;
using FishEvolution.Gameplay;
using FishEvolution.Quest;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Ads
{
    public sealed class AdRewardSystem : IStartable, IDisposable
    {
        private readonly AdSettings _settings;
        private readonly PlayerController _player;
        private readonly ISubscriber<AdCompletedEvent> _adSubscriber;
        private readonly IPublisher<RewardGrantRequest> _rewardPublisher;
        private readonly IPublisher<BuffRequest> _buffPublisher;
        private IDisposable _adSubscription;
        private int _reviveCount;

        public AdRewardSystem(
            AdSettings settings,
            PlayerController player,
            ISubscriber<AdCompletedEvent> adSubscriber,
            IPublisher<RewardGrantRequest> rewardPublisher,
            IPublisher<BuffRequest> buffPublisher)
        {
            _settings = settings;
            _player = player;
            _adSubscriber = adSubscriber;
            _rewardPublisher = rewardPublisher;
            _buffPublisher = buffPublisher;
        }

        public void Start()
        {
            _adSubscription = _adSubscriber.Subscribe(HandleAdCompleted);
        }

        public void Dispose()
        {
            _adSubscription?.Dispose();
            _adSubscription = null;
        }

        private void HandleAdCompleted(AdCompletedEvent adEvent)
        {
            switch (adEvent.AdType)
            {
                case AdType.Revive:
                    TryRevivePlayer();
                    break;
                case AdType.DoubleGold:
                    GrantDoubleGoldReward();
                    break;
                default:
                    GrantRewardGold();
                    break;
            }
        }

        private void GrantRewardGold()
        {
            PublishGoldReward("RewardAd", _settings.RewardGold);
        }

        private void GrantDoubleGoldReward()
        {
            PublishGoldReward("DoubleGoldAd", _settings.DoubleGoldReward);
            _buffPublisher.Publish(
                new BuffRequest(
                    _player,
                    BuffType.Exp,
                    _settings.DoubleGoldExpBonus,
                    _settings.DoubleGoldDuration));
        }

        private void TryRevivePlayer()
        {
            if (_reviveCount >= _settings.DailyReviveLimit ||
                _player == null ||
                !_player.TryGetComponent<HealthComponent>(out var health))
            {
                return;
            }

            _reviveCount++;
            health.Revive();
        }

        private void PublishGoldReward(
            string sourceId,
            int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _rewardPublisher.Publish(
                new RewardGrantRequest(
                    sourceId,
                    new[] { new RewardData(RewardType.Gold, amount) }));
        }
    }
}
