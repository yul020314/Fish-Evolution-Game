namespace FishEvolution.Quest
{
    public readonly struct QuestProgressChangedEvent
    {
        public QuestProgressChangedEvent(QuestRuntimeState questState)
        {
            QuestState = questState;
        }

        public QuestRuntimeState QuestState { get; }
    }
}
