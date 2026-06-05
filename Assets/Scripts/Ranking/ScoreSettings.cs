using System;
using UnityEngine;

namespace FishEvolution.Ranking
{
    [Serializable]
    public sealed class ScoreSettings
    {
        [SerializeField] private int _foodScore = 1;
        [SerializeField] private int _fishEatScore = 10;
        [SerializeField] private int _levelScore = 100;
        [SerializeField] private int _bossScore = 1000;
        [SerializeField] private int _questScore = 150;

        public int FoodScore => Mathf.Max(0, _foodScore);
        public int FishEatScore => Mathf.Max(0, _fishEatScore);
        public int LevelScore => Mathf.Max(0, _levelScore);
        public int BossScore => Mathf.Max(0, _bossScore);
        public int QuestScore => Mathf.Max(0, _questScore);
    }
}
