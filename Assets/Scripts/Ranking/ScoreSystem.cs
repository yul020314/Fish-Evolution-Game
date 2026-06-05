using System;
using FishEvolution.Boss;
using FishEvolution.Gameplay;
using FishEvolution.Quest;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Ranking
{
    public sealed class ScoreSystem : IStartable, IDisposable
    {
        private readonly ScoreSettings _settings;
        private readonly ISubscriber<FoodConsumedEvent> _foodSubscriber;
        private readonly ISubscriber<EatCompletedEvent> _eatSubscriber;
        private readonly ISubscriber<PlayerLevelUpEvent> _levelSubscriber;
        private readonly ISubscriber<BossDeadEvent> _bossSubscriber;
        private readonly ISubscriber<QuestCompletedEvent> _questSubscriber;
        private readonly IPublisher<ScoreChangedEvent> _scorePublisher;
        private IDisposable _foodSubscription;
        private IDisposable _eatSubscription;
        private IDisposable _levelSubscription;
        private IDisposable _bossSubscription;
        private IDisposable _questSubscription;

        public ScoreSystem(
            ScoreSettings settings,
            ISubscriber<FoodConsumedEvent> foodSubscriber,
            ISubscriber<EatCompletedEvent> eatSubscriber,
            ISubscriber<PlayerLevelUpEvent> levelSubscriber,
            ISubscriber<BossDeadEvent> bossSubscriber,
            ISubscriber<QuestCompletedEvent> questSubscriber,
            IPublisher<ScoreChangedEvent> scorePublisher)
        {
            _settings = settings;
            _foodSubscriber = foodSubscriber;
            _eatSubscriber = eatSubscriber;
            _levelSubscriber = levelSubscriber;
            _bossSubscriber = bossSubscriber;
            _questSubscriber = questSubscriber;
            _scorePublisher = scorePublisher;
        }

        public int CurrentScore { get; private set; }

        public void Start()
        {
            _foodSubscription = _foodSubscriber.Subscribe(
                _ => AddScore(_settings.FoodScore));
            _eatSubscription = _eatSubscriber.Subscribe(
                _ => AddScore(_settings.FishEatScore));
            _levelSubscription = _levelSubscriber.Subscribe(HandleLevelUp);
            _bossSubscription = _bossSubscriber.Subscribe(
                _ => AddScore(_settings.BossScore));
            _questSubscription = _questSubscriber.Subscribe(
                _ => AddScore(_settings.QuestScore));
        }

        public void Dispose()
        {
            _foodSubscription?.Dispose();
            _eatSubscription?.Dispose();
            _levelSubscription?.Dispose();
            _bossSubscription?.Dispose();
            _questSubscription?.Dispose();
        }

        private void HandleLevelUp(PlayerLevelUpEvent levelUpEvent)
        {
            var levelDelta = levelUpEvent.CurrentLevel - levelUpEvent.PreviousLevel;
            AddScore(_settings.LevelScore * Math.Max(1, levelDelta));
        }

        private void AddScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentScore += amount;
            _scorePublisher.Publish(
                new ScoreChangedEvent(CurrentScore, amount));
        }
    }
}
