namespace FishEvolution.Quest
{
    public sealed class QuestRuntimeState
    {
        public QuestRuntimeState(QuestDataSO questData)
        {
            QuestData = questData;
        }

        public QuestDataSO QuestData { get; }
        public int Progress { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsRewardClaimed { get; private set; }

        public bool AddProgress(int amount)
        {
            if (IsCompleted || amount <= 0)
            {
                return false;
            }

            Progress += amount;
            if (Progress < QuestData.TargetAmount)
            {
                return false;
            }

            Progress = QuestData.TargetAmount;
            IsCompleted = true;
            return true;
        }

        public void SetRewardClaimed()
        {
            IsRewardClaimed = true;
        }
    }
}
