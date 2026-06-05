using System;
using UnityEngine;

namespace FishEvolution.Quest
{
    [Serializable]
    public sealed class RewardData
    {
        [SerializeField] private RewardType _rewardType = RewardType.Gold;
        [SerializeField] private int _amount = 100;

        public RewardType RewardType => _rewardType;
        public int Amount => Mathf.Max(0, _amount);
    }
}
