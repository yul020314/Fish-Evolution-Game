using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FishEvolution.Combat;
using FishEvolution.Gameplay;
using MessagePipe;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer.Unity;

namespace FishEvolution.VFX
{
    public sealed class VFXManager : IStartable, IDisposable
    {
        private readonly VFXPlaybackSettings _settings;
        private readonly VFXPoolRoot _poolRoot;
        private readonly ISubscriber<FoodConsumedEvent> _foodSubscriber;
        private readonly ISubscriber<EatCompletedEvent> _eatSubscriber;
        private readonly ISubscriber<PlayerLevelUpEvent> _levelUpSubscriber;
        private readonly ISubscriber<EntityDeathEvent> _deathSubscriber;
        private readonly ISubscriber<SkillUsedEvent> _skillSubscriber;
        private readonly Dictionary<VFXType, VFXPoolEntry> _pools;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private IDisposable _foodSubscription;
        private IDisposable _eatSubscription;
        private IDisposable _levelUpSubscription;
        private IDisposable _deathSubscription;
        private IDisposable _skillSubscription;

        public VFXManager(
            VFXPlaybackSettings settings,
            VFXPoolRoot poolRoot,
            ISubscriber<FoodConsumedEvent> foodSubscriber,
            ISubscriber<EatCompletedEvent> eatSubscriber,
            ISubscriber<PlayerLevelUpEvent> levelUpSubscriber,
            ISubscriber<EntityDeathEvent> deathSubscriber,
            ISubscriber<SkillUsedEvent> skillSubscriber)
        {
            _settings = settings;
            _poolRoot = poolRoot;
            _foodSubscriber = foodSubscriber;
            _eatSubscriber = eatSubscriber;
            _levelUpSubscriber = levelUpSubscriber;
            _deathSubscriber = deathSubscriber;
            _skillSubscriber = skillSubscriber;
            _pools = new Dictionary<VFXType, VFXPoolEntry>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            CreatePoolsAsync(_cancellationTokenSource.Token).Forget();
            SubscribeEvents();
        }

        public void Dispose()
        {
            _foodSubscription?.Dispose();
            _eatSubscription?.Dispose();
            _levelUpSubscription?.Dispose();
            _deathSubscription?.Dispose();
            _skillSubscription?.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            ClearPools();
        }

        private async UniTaskVoid CreatePoolsAsync(CancellationToken cancellationToken)
        {
            var cues = _settings.Cues;
            for (var index = 0; index < cues.Length; index++)
            {
                await TryCreatePoolAsync(cues[index], cancellationToken);
            }
        }

        private async UniTask TryCreatePoolAsync(
            VFXCue cue,
            CancellationToken cancellationToken)
        {
            if (cue == null || !cue.IsValid || _pools.ContainsKey(cue.Type))
            {
                return;
            }

            var handle = cue.PrefabReference.LoadAssetAsync<GameObject>();
            var prefab = await handle.ToUniTask(cancellationToken: cancellationToken);
            if (!TryGetParticlePrefab(prefab, out var particleSystem))
            {
                Addressables.Release(handle);
                return;
            }

            var pool = new EffectPool(particleSystem, _poolRoot.Root);
            pool.Prewarm(cue.PrewarmCount);
            _pools.Add(cue.Type, new VFXPoolEntry(cue, pool, handle));
        }

        private static bool TryGetParticlePrefab(
            GameObject prefab,
            out ParticleSystem particleSystem)
        {
            particleSystem = prefab != null
                ? prefab.GetComponent<ParticleSystem>()
                : null;
            return particleSystem != null;
        }

        private void SubscribeEvents()
        {
            _foodSubscription = _foodSubscriber.Subscribe(HandleFoodConsumed);
            _eatSubscription = _eatSubscriber.Subscribe(HandleEatCompleted);
            _levelUpSubscription = _levelUpSubscriber.Subscribe(HandleLevelUp);
            _deathSubscription = _deathSubscriber.Subscribe(HandleDeath);
            _skillSubscription = _skillSubscriber.Subscribe(HandleSkillUsed);
        }

        private void HandleFoodConsumed(FoodConsumedEvent message)
        {
            if (message.Player == null)
            {
                return;
            }

            Play(VFXType.Eat, message.Player.transform.position);
        }

        private void HandleEatCompleted(EatCompletedEvent message)
        {
            if (message.Target == null)
            {
                return;
            }

            Play(VFXType.Eat, message.Target.transform.position);
        }

        private void HandleLevelUp(PlayerLevelUpEvent message)
        {
            if (message.Player == null)
            {
                return;
            }

            Play(VFXType.LevelUp, message.Player.transform.position);
        }

        private void HandleDeath(EntityDeathEvent message)
        {
            if (message.Entity == null)
            {
                return;
            }

            Play(VFXType.Death, message.Entity.transform.position);
        }

        private void HandleSkillUsed(SkillUsedEvent message)
        {
            if (message.Player == null)
            {
                return;
            }

            Play(VFXType.Skill, message.Player.transform.position);
        }

        private void Play(
            VFXType type,
            Vector3 position)
        {
            if (!_settings.TryGetCue(type, out var cue) ||
                !_pools.TryGetValue(type, out var entry))
            {
                return;
            }

            var effect = entry.Pool.Get();
            effect.transform.position = position + cue.Offset;
            effect.transform.rotation = Quaternion.identity;
            effect.Play(true);
            ReleaseAfterDelayAsync(
                entry.Pool,
                effect,
                GetLifetime(effect, cue),
                _cancellationTokenSource.Token).Forget();
        }

        private static float GetLifetime(
            ParticleSystem effect,
            VFXCue cue)
        {
            var main = effect.main;
            var duration = main.duration + main.startLifetime.constantMax;
            return Mathf.Max(cue.FallbackLifetime, duration);
        }

        private async UniTaskVoid ReleaseAfterDelayAsync(
            EffectPool pool,
            ParticleSystem effect,
            float delay,
            CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(delay),
                    cancellationToken: cancellationToken);
                pool.Release(effect);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void ClearPools()
        {
            foreach (var entry in _pools.Values)
            {
                entry.Pool.Clear();
                ReleaseHandle(entry.PrefabHandle);
            }

            _pools.Clear();
        }

        private static void ReleaseHandle(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
