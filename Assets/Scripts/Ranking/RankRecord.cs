using System;

namespace FishEvolution.Ranking
{
    public readonly struct RankRecord
    {
        public RankRecord(
            string playerName,
            int score,
            DateTime recordedAt)
        {
            PlayerName = playerName;
            Score = score;
            RecordedAt = recordedAt;
        }

        public string PlayerName { get; }
        public int Score { get; }
        public DateTime RecordedAt { get; }
    }
}
