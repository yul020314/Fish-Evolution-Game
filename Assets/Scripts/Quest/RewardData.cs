using System;
using UnityEngine;

namespace FishEvolution.Quest
{
    [Serializable]
    public sealed class RewardData
    {
        [SerializeField] private RewardType _rewardType = RewardType.Gold;
        [SerializeField] private int _amount = 100;

        public RewardData()
        {
        }

        public RewardData(
            RewardType rewardType,
            int amount)
        {
            _rewardType = rewardType;
            _amount = amount;
        }

        public RewardType RewardType => _rewardType;
        public int Amount => Mathf.Max(0, _amount);
    }
}
