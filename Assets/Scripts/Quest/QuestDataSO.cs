using UnityEngine;

namespace FishEvolution.Quest
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Fish Evolution/Quest/Quest Data")]
    public sealed class QuestDataSO : ScriptableObject
    {
        [SerializeField] private string _questId = string.Empty;
        [SerializeField] private string _questName = string.Empty;
        [SerializeField] private QuestCategory _category = QuestCategory.Daily;
        [SerializeField] private QuestObjectiveType _objectiveType = QuestObjectiveType.EatFood;
        [SerializeField] private int _targetAmount = 1;
        [SerializeField] private RewardData[] _rewards = new RewardData[0];

        public string QuestId => _questId;
        public string QuestName => _questName;
        public QuestCategory Category => _category;
        public QuestObjectiveType ObjectiveType => _objectiveType;
        public int TargetAmount => Mathf.Max(1, _targetAmount);
        public RewardData[] Rewards => _rewards;
    }
}
