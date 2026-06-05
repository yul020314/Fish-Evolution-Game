namespace FishEvolution.Ranking
{
    public readonly struct ScoreChangedEvent
    {
        public ScoreChangedEvent(
            int score,
            int delta)
        {
            Score = score;
            Delta = delta;
        }

        public int Score { get; }
        public int Delta { get; }
    }
}
