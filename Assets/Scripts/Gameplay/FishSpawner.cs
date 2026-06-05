using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FishEvolution.AI;
using FishEvolution.Combat;
using FishEvolution.Config;
using FishEvolution.Pool;
using MessagePipe;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace FishEvolution.Gameplay
{
    public sealed class FishSpawner : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject _fishPrefabReference;
        [SerializeField] private FishDataSO[] _fishData = new FishDataSO[0];
        [SerializeField] private int _spawnCount = 18;
        [SerializeField] private Vector2 _spawnCenter = Vector2.zero;
        [SerializeField] private Vector2 _spawnSize = new Vector2(24f, 14f);
        [SerializeField] private Transform _poolRoot;

        private FishPool _fishPool;
        private AsyncOperationHandle<GameObject> _fishPrefabHandle;
        private PlayerController _player;
        private IPublisher<DamageRequest> _damagePublisher;
        private IPublisher<EatRequest> _eatPublisher;
        private IDisposable _eatCompletedSubscription;

        [Inject]
        public void Construct(
            PlayerController player,
            IPublisher<DamageRequest> damagePublisher,
            IPublisher<EatRequest> eatPublisher,
            ISubscriber<EatCompletedEvent> eatCompletedSubscriber)
        {
            _player = player;
            _damagePublisher = damagePublisher;
            _eatPublisher = eatPublisher;
            SubscribeEatCompleted(eatCompletedSubscriber);
        }

        private void Awake()
        {
            if (_poolRoot == null)
            {
                _poolRoot = transform;
            }
        }

        private void Start()
        {
            LoadFishPrefabAsync(destroyCancellationToken).Forget();
        }

        private void OnDestroy()
        {
            _eatCompletedSubscription?.Dispose();
            _eatCompletedSubscription = null;
            _fishPool?.Clear();
            ReleaseFishPrefabHandle();
        }

        private async UniTask LoadFishPrefabAsync(CancellationToken cancellationToken)
        {
            if (!CanLoadPrefab())
            {
                return;
            }

            try
            {
                _fishPrefabHandle = _fishPrefabReference.LoadAssetAsync<GameObject>();
                var prefab = await _fishPrefabHandle.ToUniTask(cancellationToken: cancellationToken);
                InitializePool(prefab);
                SpawnInitialFish();
            }
            catch (OperationCanceledException)
            {
                ReleaseFishPrefabHandle();
            }
        }

        private void SpawnInitialFish()
        {
            if (!CanSpawn())
            {
                return;
            }

            var spawnCount = GetSpawnCount();
            for (var index = 0; index < spawnCount; index++)
            {
                SpawnOne();
            }
        }

        private void InitializePool(GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            if (!prefab.TryGetComponent<FishController>(out var fishPrefab))
            {
                return;
            }

            _fishPool = new FishPool(fishPrefab, _poolRoot);
            _fishPool.Prewarm(GetSpawnCount());
        }

        private void SpawnOne()
        {
            var fish = _fishPool.Get();
            fish.transform.position = GetRandomPosition();
            fish.transform.rotation = GetRandomRotation();
            fish.Initialize(GetRandomFishData());
            InitializeCombat(fish);
            InitializeCollision(fish);
            InitializeAI(fish);
        }

        public void ReleaseAndRespawn(FishController fish)
        {
            if (fish == null || _fishPool == null)
            {
                return;
            }

            _fishPool.Release(fish);
            if (CanSpawn())
            {
                SpawnOne();
            }
        }

        private void SubscribeEatCompleted(ISubscriber<EatCompletedEvent> subscriber)
        {
            _eatCompletedSubscription?.Dispose();
            _eatCompletedSubscription = subscriber?.Subscribe(HandleEatCompleted);
        }

        private void HandleEatCompleted(EatCompletedEvent message)
        {
            if (message.Target == null ||
                !message.Target.TryGetComponent<FishController>(out var fish))
            {
                return;
            }

            ReleaseAndRespawn(fish);
        }

        private void InitializeCombat(FishController fish)
        {
            if (fish == null || fish.FishData == null)
            {
                return;
            }

            if (fish.TryGetComponent<HealthComponent>(out var health))
            {
                health.Initialize(fish.FishData.HP);
            }

            if (fish.TryGetComponent<AttackComponent>(out var attack))
            {
                attack.Initialize(fish.FishData.Attack, _damagePublisher);
            }
        }

        private void InitializeCollision(FishController fish)
        {
            if (fish == null ||
                !fish.TryGetComponent<CollisionSystem>(out var collisionSystem))
            {
                return;
            }

            collisionSystem.Initialize(_eatPublisher);
        }

        private void InitializeAI(FishController fish)
        {
            if (fish == null ||
                !fish.TryGetComponent<FishAIController>(out var aiController))
            {
                return;
            }

            aiController.Initialize(_player, _spawnCenter, _spawnSize);
        }

        private FishDataSO GetRandomFishData()
        {
            var startIndex = UnityEngine.Random.Range(0, GetFishDataCount());
            for (var offset = 0; offset < GetFishDataCount(); offset++)
            {
                var index = (startIndex + offset) % GetFishDataCount();
                if (_fishData[index] != null)
                {
                    return _fishData[index];
                }
            }

            return null;
        }

        private Vector3 GetRandomPosition()
        {
            var halfSize = _spawnSize * 0.5f;
            var x = UnityEngine.Random.Range(_spawnCenter.x - halfSize.x, _spawnCenter.x + halfSize.x);
            var y = UnityEngine.Random.Range(_spawnCenter.y - halfSize.y, _spawnCenter.y + halfSize.y);
            return new Vector3(x, y, 0f);
        }

        private Quaternion GetRandomRotation()
        {
            var angle = UnityEngine.Random.Range(-15f, 15f);
            return Quaternion.Euler(0f, 0f, angle);
        }

        private bool CanSpawn()
        {
            return _fishPool != null && GetSpawnCount() > 0 && HasFishData();
        }

        private bool CanLoadPrefab()
        {
            return _fishPrefabReference != null && _fishPrefabReference.RuntimeKeyIsValid();
        }

        private void ReleaseFishPrefabHandle()
        {
            if (_fishPrefabHandle.IsValid())
            {
                Addressables.Release(_fishPrefabHandle);
                _fishPrefabHandle = default;
            }
        }

        private int GetSpawnCount()
        {
            return Mathf.Max(0, _spawnCount);
        }

        private int GetFishDataCount()
        {
            return _fishData != null ? _fishData.Length : 0;
        }

        private bool HasFishData()
        {
            if (_fishData == null)
            {
                return false;
            }

            for (var index = 0; index < _fishData.Length; index++)
            {
                if (_fishData[index] != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
