using System;
using FishEvolution.Config;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace FishEvolution.Map
{
    public sealed class FogSystem : MonoBehaviour
    {
        [SerializeField] private FogAreaBinding[] _fogAreas = new FogAreaBinding[0];
        [SerializeField] private bool _showFogOnAwake = true;

        private IDisposable _mapChangedSubscription;
        private IDisposable _mapUnlockedSubscription;

        [Inject]
        public void Construct(
            ISubscriber<MapChangedEvent> mapChangedSubscriber,
            ISubscriber<MapUnlockedEvent> mapUnlockedSubscriber)
        {
            _mapChangedSubscription = mapChangedSubscriber.Subscribe(HandleMapChanged);
            _mapUnlockedSubscription = mapUnlockedSubscriber.Subscribe(HandleMapUnlocked);
        }

        private void Awake()
        {
            if (_showFogOnAwake)
            {
                SetAllFogVisible(true);
            }
        }

        private void OnDestroy()
        {
            _mapChangedSubscription?.Dispose();
            _mapUnlockedSubscription?.Dispose();
            _mapChangedSubscription = null;
            _mapUnlockedSubscription = null;
        }

        public void RevealArea(MapAreaType areaType)
        {
            if (TryGetBinding(areaType, out var binding))
            {
                binding.SetVisible(false);
            }
        }

        private void HandleMapChanged(MapChangedEvent mapChangedEvent)
        {
            RevealArea(mapChangedEvent.CurrentArea);
        }

        private void HandleMapUnlocked(MapUnlockedEvent mapUnlockedEvent)
        {
            RevealArea(mapUnlockedEvent.AreaType);
        }

        private void SetAllFogVisible(bool isVisible)
        {
            for (var index = 0; index < _fogAreas.Length; index++)
            {
                if (_fogAreas[index] != null)
                {
                    _fogAreas[index].SetVisible(isVisible);
                }
            }
        }

        private bool TryGetBinding(
            MapAreaType areaType,
            out FogAreaBinding binding)
        {
            for (var index = 0; index < _fogAreas.Length; index++)
            {
                binding = _fogAreas[index];
                if (binding != null &&
                    binding.IsValid &&
                    binding.AreaType == areaType)
                {
                    return true;
                }
            }

            binding = null;
            return false;
        }
    }
}
