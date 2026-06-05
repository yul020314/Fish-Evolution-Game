using UnityEngine;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Fish Evolution/Config/Item Data")]
    public sealed class ItemDataSO : ScriptableObject
    {
        [SerializeField] private string _itemId = string.Empty;
        [SerializeField] private string _itemName = string.Empty;
        [SerializeField] private ItemType _itemType = ItemType.Gold;
        [SerializeField] private int _priceGold = 0;
        [SerializeField] private int _priceDiamond = 0;
        [SerializeField] private bool _isConsumable = true;

        public string ItemId => _itemId;
        public string ItemName => _itemName;
        public ItemType ItemType => _itemType;
        public int PriceGold => _priceGold;
        public int PriceDiamond => _priceDiamond;
        public bool IsConsumable => _isConsumable;
    }
}
