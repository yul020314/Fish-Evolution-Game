using System;
using FishEvolution.Boss;
using FishEvolution.Gameplay;
using FishEvolution.Map;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Quest
{
    public sealed class QuestManager : IStartable, IDisposable
    {
        private readonly QuestRuntimeState[] _quests;
        private readonly ISubscriber<FoodConsumedEvent> _foodSubscriber;
        private readonly ISubscriber<EatCompletedEvent> _eatSubscriber;
        private readonly ISubscriber<PlayerLevelUpEvent> _levelUpSubscriber;
        private readonly ISubscriber<BossDeadEvent> _bossDeadSubscriber;
        private readonly ISubscriber<MapUnlockedEvent> _mapUnlockedSubscriber;
        private readonly ISubscriber<QuestRewardClaimRequest> _claimSubscriber;
        private readonly IPublisher<QuestProgressChangedEvent> _progressPublisher;
        private readonly IPublisher<QuestCompletedEvent> _completedPublisher;
        private readonly IPublisher<RewardGrantRequest> _rewardPublisher;

        private IDisposable _foodSubscription;
        private IDisposable _eatSubscription;
        private IDisposable _levelUpSubscription;
        private IDisposable _bossDeadSubscription;
        private IDisposable _mapUnlockedSubscription;
        private IDisposable _claimSubscription;

        public QuestManager(
            QuestDataSO[] questData,
            ISubscriber<FoodConsumedEvent> foodSubscriber,
            ISubscriber<EatCompletedEvent> eatSubscriber,
            ISubscriber<PlayerLevelUpEvent> levelUpSubscriber,
            ISubscriber<BossDeadEvent> bossDeadSubscriber,
            ISubscriber<MapUnlockedEvent> mapUnlockedSubscriber,
            ISubscriber<QuestRewardClaimRequest> claimSubscriber,
            IPublisher<QuestProgressChangedEvent> progressPublisher,
            IPublisher<QuestCompletedEvent> completedPublisher,
            IPublisher<RewardGrantRequest> rewardPublisher)
        {
            _quests = CreateQuestStates(questData);
            _foodSubscriber = foodSubscriber;
            _eatSubscriber = eatSubscriber;
            _levelUpSubscriber = levelUpSubscriber;
            _bossDeadSubscriber = bossDeadSubscriber;
            _mapUnlockedSubscriber = mapUnlockedSubscriber;
            _claimSubscriber = claimSubscriber;
            _progressPublisher = progressPublisher;
            _completedPublisher = completedPublisher;
            _rewardPublisher = rewardPublisher;
        }

        public void Start()
        {
            _foodSubscription = _foodSubscriber.Subscribe(HandleFoodConsumed);
            _eatSubscription = _eatSubscriber.Subscribe(HandleEatCompleted);
            _levelUpSubscription = _levelUpSubscriber.Subscribe(HandleLevelUp);
            _bossDeadSubscription = _bossDeadSubscriber.Subscribe(HandleBossDead);
            _mapUnlockedSubscription = _mapUnlockedSubscriber.Subscribe(HandleMapUnlocked);
            _claimSubscription = _claimSubscriber.Subscribe(HandleClaimRequest);
        }

        public void Dispose()
        {
            _foodSubscription?.Dispose();
            _eatSubscription?.Dispose();
            _levelUpSubscription?.Dispose();
            _bossDeadSubscription?.Dispose();
            _mapUnlockedSubscription?.Dispose();
            _claimSubscription?.Dispose();
        }

        public bool TryGetQuest(
            string questId,
            out QuestRuntimeState questState)
        {
            for (var index = 0; index < _quests.Length; index++)
            {
                questState = _quests[index];
                if (questState != null &&
                    questState.QuestData.QuestId == questId)
                {
                    return true;
                }
            }

            questState = null;
            return false;
        }

        private void HandleFoodConsumed(FoodConsumedEvent message)
        {
            AddProgress(QuestObjectiveType.EatFood, 1);
        }

        private void HandleEatCompleted(EatCompletedEvent message)
        {
            AddProgress(QuestObjectiveType.EatFish, 1);
        }

        private void HandleLevelUp(PlayerLevelUpEvent message)
        {
            SetProgressAtLeast(
                QuestObjectiveType.ReachLevel,
                message.CurrentLevel);
        }

        private void HandleBossDead(BossDeadEvent message)
        {
            AddProgress(QuestObjectiveType.DefeatBoss, 1);
        }

        private void HandleMapUnlocked(MapUnlockedEvent message)
        {
            AddProgress(QuestObjectiveType.UnlockMap, 1);
        }

        private void HandleClaimRequest(QuestRewardClaimRequest request)
        {
            if (!request.IsValid ||
                !TryGetQuest(request.QuestId, out var questState) ||
                !CanClaim(questState))
            {
                return;
            }

            questState.SetRewardClaimed();
            _rewardPublisher.Publish(
                new RewardGrantRequest(
                    questState.QuestData.QuestId,
                    questState.QuestData.Rewards));
        }

        private void AddProgress(
            QuestObjectiveType objectiveType,
            int amount)
        {
            for (var index = 0; index < _quests.Length; index++)
            {
                TryAddProgress(_quests[index], objectiveType, amount);
            }
        }

        private void SetProgressAtLeast(
            QuestObjectiveType objectiveType,
            int value)
        {
            for (var index = 0; index < _quests.Length; index++)
            {
                var quest = _quests[index];
                if (quest == null ||
                    quest.QuestData.ObjectiveType != objectiveType)
                {
                    continue;
                }

                var delta = value - quest.Progress;
                TryAddProgress(quest, objectiveType, delta);
            }
        }

        private void TryAddProgress(
            QuestRuntimeState questState,
            QuestObjectiveType objectiveType,
            int amount)
        {
            if (questState == null ||
                questState.QuestData.ObjectiveType != objectiveType ||
                amount <= 0)
            {
                return;
            }

            var completedNow = questState.AddProgress(amount);
            _progressPublisher.Publish(
                new QuestProgressChangedEvent(questState));
            if (completedNow)
            {
                _completedPublisher.Publish(
                    new QuestCompletedEvent(questState));
            }
        }

        private static bool CanClaim(QuestRuntimeState questState)
        {
            return questState.IsCompleted && !questState.IsRewardClaimed;
        }

        private static QuestRuntimeState[] CreateQuestStates(QuestDataSO[] questData)
        {
            var count = questData != null ? questData.Length : 0;
            var states = new QuestRuntimeState[count];
            for (var index = 0; index < count; index++)
            {
                if (questData[index] != null)
                {
                    states[index] = new QuestRuntimeState(questData[index]);
                }
            }

            return states;
        }
    }
}
