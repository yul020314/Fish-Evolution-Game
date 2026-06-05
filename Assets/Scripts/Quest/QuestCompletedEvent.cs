namespace FishEvolution.Quest
{
    public readonly struct QuestCompletedEvent
    {
        public QuestCompletedEvent(QuestRuntimeState questState)
        {
            QuestState = questState;
        }

        public QuestRuntimeState QuestState { get; }
    }
}
