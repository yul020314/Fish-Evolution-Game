using System;
using FishEvolution.Quest;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Shop
{
    public sealed class ShopWallet : IStartable, IDisposable
    {
        private readonly ISubscriber<RewardGrantedEvent> _rewardSubscriber;
        private IDisposable _rewardSubscription;

        public ShopWallet(ISubscriber<RewardGrantedEvent> rewardSubscriber)
        {
            _rewardSubscriber = rewardSubscriber;
        }

        public int Gold { get; private set; }
        public int Diamond { get; private set; }

        public void Start()
        {
            _rewardSubscription = _rewardSubscriber.Subscribe(HandleRewardGranted);
        }

        public void Dispose()
        {
            _rewardSubscription?.Dispose();
            _rewardSubscription = null;
        }

        public bool CanSpend(
            ShopCurrencyType currencyType,
            int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            return currencyType == ShopCurrencyType.Diamond
                ? Diamond >= amount
                : Gold >= amount;
        }

        public bool TrySpend(
            ShopCurrencyType currencyType,
            int amount)
        {
            if (!CanSpend(currencyType, amount))
            {
                return false;
            }

            if (currencyType == ShopCurrencyType.Diamond)
            {
                Diamond -= amount;
                return true;
            }

            Gold -= amount;
            return true;
        }

        private void HandleRewardGranted(RewardGrantedEvent reward)
        {
            Gold += reward.Gold;
            Diamond += reward.Diamond;
        }
    }
}
