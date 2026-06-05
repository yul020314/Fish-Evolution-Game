using System.Collections.Generic;
using FishEvolution.Config;

namespace FishEvolution.Shop
{
    public sealed class UnlockInventory
    {
        private readonly HashSet<string> _fishIds = new HashSet<string>();
        private readonly HashSet<string> _skinIds = new HashSet<string>();

        public bool IsFishUnlocked(FishDataSO fishData)
        {
            return fishData != null && _fishIds.Contains(fishData.FishId);
        }

        public bool IsSkinUnlocked(string skinId)
        {
            return !string.IsNullOrWhiteSpace(skinId) &&
                _skinIds.Contains(skinId);
        }

        public bool UnlockFish(FishDataSO fishData)
        {
            if (fishData == null ||
                string.IsNullOrWhiteSpace(fishData.FishId))
            {
                return false;
            }

            return _fishIds.Add(fishData.FishId);
        }

        public bool UnlockSkin(string skinId)
        {
            if (string.IsNullOrWhiteSpace(skinId))
            {
                return false;
            }

            return _skinIds.Add(skinId);
        }
    }
}
