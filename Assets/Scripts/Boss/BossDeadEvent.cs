using FishEvolution.Config;
using UnityEngine;

namespace FishEvolution.Boss
{
    public readonly struct BossDeadEvent
    {
        public BossDeadEvent(
            BossDataSO bossData,
            GameObject boss,
            GameObject killer)
        {
            BossData = bossData;
            Boss = boss;
            Killer = killer;
        }

        public BossDataSO BossData { get; }
        public GameObject Boss { get; }
        public GameObject Killer { get; }
    }
}
