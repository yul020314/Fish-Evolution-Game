namespace FishEvolution.Shop
{
    public readonly struct ShopPurchaseCompletedEvent
    {
        public ShopPurchaseCompletedEvent(ShopItemDataSO itemData)
        {
            ItemData = itemData;
        }

        public ShopItemDataSO ItemData { get; }
    }
}
