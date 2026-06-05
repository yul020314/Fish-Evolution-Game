namespace FishEvolution.Quest
{
    public readonly struct RewardGrantedEvent
    {
        public RewardGrantedEvent(
            string sourceId,
            int gold,
            int diamond)
        {
            SourceId = sourceId;
            Gold = gold;
            Diamond = diamond;
        }

        public string SourceId { get; }
        public int Gold { get; }
        public int Diamond { get; }
    }
}
