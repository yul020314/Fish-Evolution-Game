using System;
using System.Collections.Generic;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Ranking
{
    public sealed class RankSystem : IStartable, IDisposable
    {
        private readonly ScoreSystem _scoreSystem;
        private readonly ISubscriber<ScoreSubmitRequest> _submitSubscriber;
        private readonly IPublisher<RankUpdatedEvent> _rankPublisher;
        private readonly List<RankRecord> _records;
        private readonly int _maxRecords;
        private IDisposable _submitSubscription;

        public RankSystem(
            ScoreSystem scoreSystem,
            ISubscriber<ScoreSubmitRequest> submitSubscriber,
            IPublisher<RankUpdatedEvent> rankPublisher,
            int maxRecords)
        {
            _scoreSystem = scoreSystem;
            _submitSubscriber = submitSubscriber;
            _rankPublisher = rankPublisher;
            _records = new List<RankRecord>();
            _maxRecords = Math.Max(1, maxRecords);
        }

        public void Start()
        {
            _submitSubscription = _submitSubscriber.Subscribe(HandleSubmit);
        }

        public void Dispose()
        {
            _submitSubscription?.Dispose();
            _submitSubscription = null;
            _records.Clear();
        }

        public RankRecord[] GetRecords()
        {
            return _records.ToArray();
        }

        private void HandleSubmit(ScoreSubmitRequest request)
        {
            if (!request.IsValid)
            {
                return;
            }

            _records.Add(
                new RankRecord(
                    request.PlayerName,
                    _scoreSystem.CurrentScore,
                    DateTime.UtcNow));
            SortAndTrim();
            _rankPublisher.Publish(
                new RankUpdatedEvent(GetRecords()));
        }

        private void SortAndTrim()
        {
            _records.Sort(CompareRecords);
            while (_records.Count > _maxRecords)
            {
                _records.RemoveAt(_records.Count - 1);
            }
        }

        private static int CompareRecords(
            RankRecord left,
            RankRecord right)
        {
            var scoreCompare = right.Score.CompareTo(left.Score);
            return scoreCompare != 0
                ? scoreCompare
                : left.RecordedAt.CompareTo(right.RecordedAt);
        }
    }
}
