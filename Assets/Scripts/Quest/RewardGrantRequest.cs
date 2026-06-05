namespace FishEvolution.Quest
{
    public readonly struct RewardGrantRequest
    {
        public RewardGrantRequest(
            string sourceId,
            RewardData[] rewards)
        {
            SourceId = sourceId;
            Rewards = rewards;
        }

        public string SourceId { get; }
        public RewardData[] Rewards { get; }
        public bool IsValid => !string.IsNullOrWhiteSpace(SourceId) &&
            Rewards != null &&
            Rewards.Length > 0;
    }
}
