using FishEvolution.Config;

namespace FishEvolution.FishBook
{
    public readonly struct FishBookEntry
    {
        public FishBookEntry(
            FishDataSO fishData,
            bool isUnlocked)
        {
            FishData = fishData;
            IsUnlocked = isUnlocked;
        }

        public FishDataSO FishData { get; }
        public bool IsUnlocked { get; }
        public string FishId => FishData != null ? FishData.FishId : string.Empty;
        public string FishName => FishData != null ? FishData.FishName : string.Empty;
    }
}
