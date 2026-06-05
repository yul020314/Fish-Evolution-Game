using System;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.Shop
{
    public sealed class ShopManager : IStartable, IDisposable
    {
        private const string MissingItemReason = "ItemNotFound";
        private const string AlreadyUnlockedReason = "AlreadyUnlocked";
        private const string NotEnoughCurrencyReason = "NotEnoughCurrency";
        private const string InvalidUnlockReason = "InvalidUnlockTarget";

        private readonly ShopCatalog _catalog;
        private readonly ShopWallet _wallet;
        private readonly UnlockInventory _inventory;
        private readonly ISubscriber<ShopPurchaseRequest> _purchaseSubscriber;
        private readonly IPublisher<ShopPurchaseCompletedEvent> _completedPublisher;
        private readonly IPublisher<ShopPurchaseFailedEvent> _failedPublisher;

        private IDisposable _purchaseSubscription;

        public ShopManager(
            ShopCatalog catalog,
            ShopWallet wallet,
            UnlockInventory inventory,
            ISubscriber<ShopPurchaseRequest> purchaseSubscriber,
            IPublisher<ShopPurchaseCompletedEvent> completedPublisher,
            IPublisher<ShopPurchaseFailedEvent> failedPublisher)
        {
            _catalog = catalog;
            _wallet = wallet;
            _inventory = inventory;
            _purchaseSubscriber = purchaseSubscriber;
            _completedPublisher = completedPublisher;
            _failedPublisher = failedPublisher;
        }

        public void Start()
        {
            _purchaseSubscription = _purchaseSubscriber.Subscribe(HandlePurchaseRequest);
        }

        public void Dispose()
        {
            _purchaseSubscription?.Dispose();
            _purchaseSubscription = null;
        }

        private void HandlePurchaseRequest(ShopPurchaseRequest request)
        {
            if (!request.IsValid ||
                !_catalog.TryGetItem(request.ItemId, out var itemData))
            {
                PublishFailed(request.ItemId, MissingItemReason);
                return;
            }

            if (IsAlreadyUnlocked(itemData))
            {
                PublishFailed(request.ItemId, AlreadyUnlockedReason);
                return;
            }

            if (!CanUnlock(itemData))
            {
                PublishFailed(request.ItemId, InvalidUnlockReason);
                return;
            }

            if (!_wallet.TrySpend(itemData.CurrencyType, itemData.Price))
            {
                PublishFailed(request.ItemId, NotEnoughCurrencyReason);
                return;
            }

            TryUnlock(itemData);
            _completedPublisher.Publish(
                new ShopPurchaseCompletedEvent(itemData));
        }

        private bool IsAlreadyUnlocked(ShopItemDataSO itemData)
        {
            if (itemData.ItemType == ShopItemType.SkinUnlock)
            {
                return _inventory.IsSkinUnlocked(itemData.SkinId);
            }

            return _inventory.IsFishUnlocked(itemData.FishData);
        }

        private bool TryUnlock(ShopItemDataSO itemData)
        {
            if (itemData.ItemType == ShopItemType.SkinUnlock)
            {
                return _inventory.UnlockSkin(itemData.SkinId);
            }

            return _inventory.UnlockFish(itemData.FishData);
        }

        private static bool CanUnlock(ShopItemDataSO itemData)
        {
            if (itemData.ItemType == ShopItemType.SkinUnlock)
            {
                return !string.IsNullOrWhiteSpace(itemData.SkinId);
            }

            return itemData.FishData != null;
        }

        private void PublishFailed(
            string itemId,
            string reason)
        {
            _failedPublisher.Publish(
                new ShopPurchaseFailedEvent(itemId, reason));
        }
    }
}
