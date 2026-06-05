using System;
using FishEvolution.Gameplay;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Tutorial
{
    public sealed class TutorialSystem : IStartable, IDisposable
    {
        private readonly GuideStep[] _steps;
        private readonly ISubscriber<PlayerMovedEvent> _movedSubscriber;
        private readonly ISubscriber<FoodConsumedEvent> _foodSubscriber;
        private readonly ISubscriber<PlayerLevelUpEvent> _levelSubscriber;
        private readonly ISubscriber<SkillUsedEvent> _skillSubscriber;
        private readonly IPublisher<TutorialStartedEvent> _startedPublisher;
        private readonly IPublisher<TutorialStepChangedEvent> _stepPublisher;
        private readonly IPublisher<TutorialCompletedEvent> _completedPublisher;
        private IDisposable _movedSubscription;
        private IDisposable _foodSubscription;
        private IDisposable _levelSubscription;
        private IDisposable _skillSubscription;
        private int _currentIndex;
        private bool _isRunning;
        private bool _isCompleted;

        public TutorialSystem(
            GuideStep[] steps,
            ISubscriber<PlayerMovedEvent> movedSubscriber,
            ISubscriber<FoodConsumedEvent> foodSubscriber,
            ISubscriber<PlayerLevelUpEvent> levelSubscriber,
            ISubscriber<SkillUsedEvent> skillSubscriber,
            IPublisher<TutorialStartedEvent> startedPublisher,
            IPublisher<TutorialStepChangedEvent> stepPublisher,
            IPublisher<TutorialCompletedEvent> completedPublisher)
        {
            _steps = steps ?? new GuideStep[0];
            _movedSubscriber = movedSubscriber;
            _foodSubscriber = foodSubscriber;
            _levelSubscriber = levelSubscriber;
            _skillSubscriber = skillSubscriber;
            _startedPublisher = startedPublisher;
            _stepPublisher = stepPublisher;
            _completedPublisher = completedPublisher;
        }

        public void Start()
        {
            SubscribeEvents();
            StartTutorialIfNeeded();
        }

        public void Dispose()
        {
            _movedSubscription?.Dispose();
            _foodSubscription?.Dispose();
            _levelSubscription?.Dispose();
            _skillSubscription?.Dispose();
        }

        private void SubscribeEvents()
        {
            _movedSubscription = _movedSubscriber.Subscribe(
                _ => TryCompleteStep(GuideStepType.Move));
            _foodSubscription = _foodSubscriber.Subscribe(
                _ => TryCompleteStep(GuideStepType.EatFood));
            _levelSubscription = _levelSubscriber.Subscribe(
                _ => TryCompleteStep(GuideStepType.LevelUp));
            _skillSubscription = _skillSubscriber.Subscribe(
                _ => TryCompleteStep(GuideStepType.UseSkill));
        }

        private void StartTutorialIfNeeded()
        {
            if (_isCompleted || _steps.Length == 0)
            {
                return;
            }

            _isRunning = true;
            _currentIndex = 0;
            _startedPublisher.Publish(new TutorialStartedEvent(CurrentStep));
            _stepPublisher.Publish(new TutorialStepChangedEvent(CurrentStep));
        }

        private void TryCompleteStep(GuideStepType stepType)
        {
            if (!_isRunning ||
                CurrentStep == null ||
                CurrentStep.StepType != stepType)
            {
                return;
            }

            Advance();
        }

        private void Advance()
        {
            _currentIndex++;
            if (_currentIndex >= _steps.Length)
            {
                CompleteTutorial();
                return;
            }

            _stepPublisher.Publish(
                new TutorialStepChangedEvent(CurrentStep));
        }

        private void CompleteTutorial()
        {
            _isRunning = false;
            _isCompleted = true;
            _completedPublisher.Publish(new TutorialCompletedEvent());
        }

        private GuideStep CurrentStep =>
            _currentIndex >= 0 && _currentIndex < _steps.Length
                ? _steps[_currentIndex]
                : null;
    }
}
