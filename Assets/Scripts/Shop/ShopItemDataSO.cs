using FishEvolution.Config;
using UnityEngine;

namespace FishEvolution.Shop
{
    [CreateAssetMenu(fileName = "ShopItem", menuName = "Fish Evolution/Shop/Shop Item")]
    public sealed class ShopItemDataSO : ScriptableObject
    {
        [SerializeField] private string _itemId = string.Empty;
        [SerializeField] private string _displayName = string.Empty;
        [SerializeField] private ShopItemType _itemType = ShopItemType.FishUnlock;
        [SerializeField] private ShopCurrencyType _currencyType = ShopCurrencyType.Gold;
        [SerializeField] private int _price = 100;
        [SerializeField] private FishDataSO _fishData;
        [SerializeField] private string _skinId = string.Empty;

        public string ItemId => _itemId;
        public string DisplayName => _displayName;
        public ShopItemType ItemType => _itemType;
        public ShopCurrencyType CurrencyType => _currencyType;
        public int Price => Mathf.Max(0, _price);
        public FishDataSO FishData => _fishData;
        public string SkinId => _skinId;
    }
}
