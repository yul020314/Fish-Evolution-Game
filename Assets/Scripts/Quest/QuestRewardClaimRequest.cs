namespace FishEvolution.Quest
{
    public readonly struct QuestRewardClaimRequest
    {
        public QuestRewardClaimRequest(string questId)
        {
            QuestId = questId;
        }

        public string QuestId { get; }
        public bool IsValid => !string.IsNullOrWhiteSpace(QuestId);
    }
}
