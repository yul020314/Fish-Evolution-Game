using System;
using FishEvolution.Config;
using FishEvolution.Gameplay;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace FishEvolution.Map
{
    public sealed class MapManager : MonoBehaviour
    {
        [SerializeField] private MapDataSO[] _maps = new MapDataSO[0];
        [SerializeField] private MapAreaType _initialArea = MapAreaType.ShallowSea;

        private MapRuntimeState[] _states;
        private PlayerGrowthController _growthController;
        private IPublisher<MapChangedEvent> _mapChangedPublisher;
        private IPublisher<MapUnlockedEvent> _mapUnlockedPublisher;
        private IPublisher<MapLockedEvent> _mapLockedPublisher;
        private IDisposable _mapChangeSubscription;
        private IDisposable _levelUpSubscription;
        private MapDataSO _currentMap;

        public MapDataSO CurrentMap => _currentMap;

        [Inject]
        public void Construct(
            PlayerGrowthController growthController,
            ISubscriber<MapChangeRequest> mapChangeSubscriber,
            ISubscriber<PlayerLevelUpEvent> levelUpSubscriber,
            IPublisher<MapChangedEvent> mapChangedPublisher,
            IPublisher<MapUnlockedEvent> mapUnlockedPublisher,
            IPublisher<MapLockedEvent> mapLockedPublisher)
        {
            _growthController = growthController;
            _mapChangedPublisher = mapChangedPublisher;
            _mapUnlockedPublisher = mapUnlockedPublisher;
            _mapLockedPublisher = mapLockedPublisher;
            _mapChangeSubscription = mapChangeSubscriber.Subscribe(HandleMapChangeRequest);
            _levelUpSubscription = levelUpSubscriber.Subscribe(HandleLevelUp);
        }

        private void Awake()
        {
            CreateStates();
            RefreshUnlockedMaps();
            TrySetCurrentArea(_initialArea);
        }

        private void OnDestroy()
        {
            _mapChangeSubscription?.Dispose();
            _levelUpSubscription?.Dispose();
            _mapChangeSubscription = null;
            _levelUpSubscription = null;
        }

        public bool IsUnlocked(MapAreaType areaType)
        {
            return TryGetState(areaType, out var state) && state.IsUnlocked;
        }

        public bool TrySetCurrentArea(MapAreaType areaType)
        {
            if (!TryGetState(areaType, out var state) || !state.IsUnlocked)
            {
                PublishLocked(areaType);
                return false;
            }

            var previousArea = _currentMap != null
                ? _currentMap.AreaType
                : state.AreaType;
            _currentMap = state.MapData;
            _mapChangedPublisher?.Publish(
                new MapChangedEvent(previousArea, _currentMap));
            return true;
        }

        public bool TryRequestAreaChange(
            MapAreaType areaType,
            PlayerController player)
        {
            if (player == null)
            {
                return false;
            }

            return TrySetCurrentArea(areaType);
        }

        private void HandleMapChangeRequest(MapChangeRequest request)
        {
            if (!request.IsValid)
            {
                return;
            }

            TrySetCurrentArea(request.AreaType);
        }

        private void HandleLevelUp(PlayerLevelUpEvent levelUpEvent)
        {
            if (_growthController == null ||
                levelUpEvent.Player != _growthController.GetComponent<PlayerController>())
            {
                return;
            }

            RefreshUnlockedMaps();
        }

        private void CreateStates()
        {
            var count = _maps != null ? _maps.Length : 0;
            _states = new MapRuntimeState[count];
            for (var index = 0; index < count; index++)
            {
                if (_maps[index] != null)
                {
                    _states[index] = new MapRuntimeState(_maps[index]);
                }
            }
        }

        private void RefreshUnlockedMaps()
        {
            var level = GetPlayerLevel();
            for (var index = 0; index < _states.Length; index++)
            {
                TryRefreshState(_states[index], level);
            }
        }

        private void TryRefreshState(
            MapRuntimeState state,
            int playerLevel)
        {
            if (state == null)
            {
                return;
            }

            var shouldUnlock = playerLevel >= state.MapData.UnlockLevel;
            if (state.IsUnlocked == shouldUnlock)
            {
                return;
            }

            state.SetUnlocked(shouldUnlock);
            if (shouldUnlock)
            {
                _mapUnlockedPublisher?.Publish(
                    new MapUnlockedEvent(state.MapData));
            }
        }

        private void PublishLocked(MapAreaType areaType)
        {
            if (TryGetState(areaType, out var state))
            {
                _mapLockedPublisher?.Publish(
                    new MapLockedEvent(state.MapData));
            }
        }

        private bool TryGetState(
            MapAreaType areaType,
            out MapRuntimeState state)
        {
            for (var index = 0; index < _states.Length; index++)
            {
                state = _states[index];
                if (state != null && state.AreaType == areaType)
                {
                    return true;
                }
            }

            state = null;
            return false;
        }

        private int GetPlayerLevel()
        {
            return _growthController != null
                ? _growthController.CurrentLevel
                : 1;
        }
    }
}
