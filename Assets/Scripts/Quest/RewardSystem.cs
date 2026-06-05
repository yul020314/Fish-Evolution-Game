using System;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Quest
{
    public sealed class RewardSystem : IStartable, IDisposable
    {
        private readonly ISubscriber<RewardGrantRequest> _rewardSubscriber;
        private readonly IPublisher<RewardGrantedEvent> _rewardGrantedPublisher;
        private IDisposable _rewardSubscription;

        public RewardSystem(
            ISubscriber<RewardGrantRequest> rewardSubscriber,
            IPublisher<RewardGrantedEvent> rewardGrantedPublisher)
        {
            _rewardSubscriber = rewardSubscriber;
            _rewardGrantedPublisher = rewardGrantedPublisher;
        }

        public int Gold { get; private set; }
        public int Diamond { get; private set; }

        public void Start()
        {
            _rewardSubscription = _rewardSubscriber.Subscribe(HandleRewardGrant);
        }

        public void Dispose()
        {
            _rewardSubscription?.Dispose();
            _rewardSubscription = null;
        }

        private void HandleRewardGrant(RewardGrantRequest request)
        {
            if (!request.IsValid)
            {
                return;
            }

            var gold = 0;
            var diamond = 0;
            SumRewards(request.Rewards, ref gold, ref diamond);
            Gold += gold;
            Diamond += diamond;
            _rewardGrantedPublisher.Publish(
                new RewardGrantedEvent(request.SourceId, gold, diamond));
        }

        private static void SumRewards(
            RewardData[] rewards,
            ref int gold,
            ref int diamond)
        {
            for (var index = 0; index < rewards.Length; index++)
            {
                var reward = rewards[index];
                if (reward == null)
                {
                    continue;
                }

                AddReward(reward, ref gold, ref diamond);
            }
        }

        private static void AddReward(
            RewardData reward,
            ref int gold,
            ref int diamond)
        {
            if (reward.RewardType == RewardType.Diamond)
            {
                diamond += reward.Amount;
                return;
            }

            gold += reward.Amount;
        }
    }
}
