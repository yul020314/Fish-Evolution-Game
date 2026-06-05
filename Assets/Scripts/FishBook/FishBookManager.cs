using System;
using FishEvolution.Config;
using FishEvolution.Shop;
using MessagePipe;
using VContainer.Unity;

namespace FishEvolution.FishBook
{
    public sealed class FishBookManager : IStartable, IDisposable
    {
        private readonly FishDataSO[] _fishData;
        private readonly UnlockInventory _unlockInventory;
        private readonly ISubscriber<ShopPurchaseCompletedEvent> _purchaseSubscriber;
        private readonly ISubscriber<FishBookSelectionRequest> _selectionSubscriber;
        private readonly IPublisher<FishBookChangedEvent> _changedPublisher;
        private readonly IPublisher<FishBookSelectionChangedEvent> _selectionPublisher;
        private IDisposable _purchaseSubscription;
        private IDisposable _selectionSubscription;
        private FishBookEntry[] _entries;

        public FishBookManager(
            FishDataSO[] fishData,
            UnlockInventory unlockInventory,
            ISubscriber<ShopPurchaseCompletedEvent> purchaseSubscriber,
            ISubscriber<FishBookSelectionRequest> selectionSubscriber,
            IPublisher<FishBookChangedEvent> changedPublisher,
            IPublisher<FishBookSelectionChangedEvent> selectionPublisher)
        {
            _fishData = fishData ?? new FishDataSO[0];
            _unlockInventory = unlockInventory;
            _purchaseSubscriber = purchaseSubscriber;
            _selectionSubscriber = selectionSubscriber;
            _changedPublisher = changedPublisher;
            _selectionPublisher = selectionPublisher;
            _entries = new FishBookEntry[0];
        }

        public FishBookEntry[] Entries => _entries;

        public void Start()
        {
            RefreshEntries();
            _purchaseSubscription = _purchaseSubscriber.Subscribe(HandlePurchaseCompleted);
            _selectionSubscription = _selectionSubscriber.Subscribe(HandleSelectionRequest);
        }

        public void Dispose()
        {
            _purchaseSubscription?.Dispose();
            _selectionSubscription?.Dispose();
            _purchaseSubscription = null;
            _selectionSubscription = null;
        }

        public bool TryGetEntry(
            string fishId,
            out FishBookEntry entry)
        {
            for (var index = 0; index < _entries.Length; index++)
            {
                entry = _entries[index];
                if (entry.FishId == fishId)
                {
                    return true;
                }
            }

            entry = default;
            return false;
        }

        private void HandlePurchaseCompleted(ShopPurchaseCompletedEvent purchaseEvent)
        {
            if (purchaseEvent.ItemData == null ||
                purchaseEvent.ItemData.ItemType != ShopItemType.FishUnlock)
            {
                return;
            }

            RefreshEntries();
        }

        private void HandleSelectionRequest(FishBookSelectionRequest request)
        {
            if (!request.IsValid ||
                !TryGetEntry(request.FishId, out var entry))
            {
                return;
            }

            _selectionPublisher.Publish(
                new FishBookSelectionChangedEvent(entry));
        }

        private void RefreshEntries()
        {
            _entries = new FishBookEntry[_fishData.Length];
            for (var index = 0; index < _fishData.Length; index++)
            {
                _entries[index] = CreateEntry(_fishData[index]);
            }

            _changedPublisher.Publish(new FishBookChangedEvent(_entries));
        }

        private FishBookEntry CreateEntry(FishDataSO fishData)
        {
            var isUnlocked = fishData != null &&
                (fishData.UnlockCost <= 0 ||
                _unlockInventory.IsFishUnlocked(fishData));
            return new FishBookEntry(fishData, isUnlocked);
        }
    }
}
