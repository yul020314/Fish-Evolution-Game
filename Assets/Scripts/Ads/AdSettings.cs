using System;
using UnityEngine;

namespace FishEvolution.Ads
{
    [Serializable]
    public sealed class AdSettings
    {
        [SerializeField] private int _rewardGold = 100;
        [SerializeField] private int _doubleGoldReward = 50;
        [SerializeField] private float _doubleGoldDuration = 600f;
        [SerializeField] private float _doubleGoldExpBonus = 1f;
        [SerializeField] private int _dailyReviveLimit = 3;

        public int RewardGold => Mathf.Max(0, _rewardGold);
        public int DoubleGoldReward => Mathf.Max(0, _doubleGoldReward);
        public float DoubleGoldDuration => Mathf.Max(0f, _doubleGoldDuration);
        public float DoubleGoldExpBonus => Mathf.Max(0f, _doubleGoldExpBonus);
        public int DailyReviveLimit => Mathf.Max(0, _dailyReviveLimit);
    }
}
