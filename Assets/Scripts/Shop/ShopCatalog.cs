namespace FishEvolution.Shop
{
    public sealed class ShopCatalog
    {
        private readonly ShopItemDataSO[] _items;

        public ShopCatalog(ShopItemDataSO[] items)
        {
            _items = items ?? new ShopItemDataSO[0];
        }

        public bool TryGetItem(
            string itemId,
            out ShopItemDataSO itemData)
        {
            for (var index = 0; index < _items.Length; index++)
            {
                itemData = _items[index];
                if (itemData != null && itemData.ItemId == itemId)
                {
                    return true;
                }
            }

            itemData = null;
            return false;
        }
    }
}
