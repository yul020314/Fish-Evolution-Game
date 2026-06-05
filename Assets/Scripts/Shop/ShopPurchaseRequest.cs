namespace FishEvolution.Shop
{
    public readonly struct ShopPurchaseRequest
    {
        public ShopPurchaseRequest(string itemId)
        {
            ItemId = itemId;
        }

        public string ItemId { get; }
        public bool IsValid => !string.IsNullOrWhiteSpace(ItemId);
    }
}
