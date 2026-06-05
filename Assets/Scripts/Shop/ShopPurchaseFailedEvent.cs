namespace FishEvolution.Shop
{
    public readonly struct ShopPurchaseFailedEvent
    {
        public ShopPurchaseFailedEvent(
            string itemId,
            string reason)
        {
            ItemId = itemId;
            Reason = reason;
        }

        public string ItemId { get; }
        public string Reason { get; }
    }
}
