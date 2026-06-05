using FishEvolution.Config;

namespace FishEvolution.Gameplay
{
    public readonly struct FoodConsumedEvent
    {
        public FoodConsumedEvent(FoodDataSO foodData, PlayerController player)
        {
            FoodData = foodData;
            Player = player;
        }

        public FoodDataSO FoodData { get; }
        public PlayerController Player { get; }
        public int Experience => FoodData != null ? FoodData.Experience : 0;
    }
}
