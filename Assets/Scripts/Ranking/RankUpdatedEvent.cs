namespace FishEvolution.Ranking
{
    public readonly struct RankUpdatedEvent
    {
        public RankUpdatedEvent(RankRecord[] records)
        {
            Records = records;
        }

        public RankRecord[] Records { get; }
    }
}
