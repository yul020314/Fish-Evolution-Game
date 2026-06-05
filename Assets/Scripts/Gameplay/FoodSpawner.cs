using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FishEvolution.Config;
using FishEvolution.Pool;
using MessagePipe;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace FishEvolution.Gameplay
{
    public sealed class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject _foodPrefabReference;
        [SerializeField] private FoodDataSO[] _foodData = new FoodDataSO[0];
        [SerializeField] private int _spawnCount = 25;
        [SerializeField] private Vector2 _spawnCenter = Vector2.zero;
        [SerializeField] private Vector2 _spawnSize = new Vector2(20f, 12f);
        [SerializeField] private Transform _poolRoot;

        private FoodPool _foodPool;
        private AsyncOperationHandle<GameObject> _foodPrefabHandle;
        private IPublisher<FoodConsumedEvent> _foodConsumedPublisher;

        [Inject]
        public void Construct(IPublisher<FoodConsumedEvent> foodConsumedPublisher)
        {
            _foodConsumedPublisher = foodConsumedPublisher;
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
            LoadFoodPrefabAsync(destroyCancellationToken).Forget();
        }

        private void OnDestroy()
        {
            _foodPool?.Clear();
            ReleaseFoodPrefabHandle();
        }

        private async UniTask LoadFoodPrefabAsync(CancellationToken cancellationToken)
        {
            if (!CanLoadPrefab())
            {
                return;
            }

            try
            {
                _foodPrefabHandle = _foodPrefabReference.LoadAssetAsync<GameObject>();
                var prefab = await _foodPrefabHandle.ToUniTask(cancellationToken: cancellationToken);
                InitializePool(prefab);
                SpawnInitialFoods();
            }
            catch (OperationCanceledException)
            {
                ReleaseFoodPrefabHandle();
            }
        }

        private void SpawnInitialFoods()
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

            if (!prefab.TryGetComponent<FoodController>(out var foodPrefab))
            {
                return;
            }

            _foodPool = new FoodPool(foodPrefab, _poolRoot);
            _foodPool.Prewarm(GetSpawnCount());
        }

        private void SpawnOne()
        {
            var food = _foodPool.Get();
            food.transform.position = GetRandomPosition();
            food.Initialize(GetRandomFoodData(), HandleFoodConsumed);
        }

        private void HandleFoodConsumed(
            FoodController food,
            PlayerController player)
        {
            PublishFoodConsumed(food, player);
            _foodPool.Release(food);

            if (CanSpawn())
            {
                SpawnOne();
            }
        }

        private void PublishFoodConsumed(
            FoodController food,
            PlayerController player)
        {
            if (_foodConsumedPublisher == null ||
                food == null ||
                player == null ||
                food.FoodData == null)
            {
                return;
            }

            _foodConsumedPublisher.Publish(new FoodConsumedEvent(food.FoodData, player));
        }

        private FoodDataSO GetRandomFoodData()
        {
            var startIndex = UnityEngine.Random.Range(0, GetFoodDataCount());
            for (var offset = 0; offset < GetFoodDataCount(); offset++)
            {
                var index = (startIndex + offset) % GetFoodDataCount();
                if (_foodData[index] != null)
                {
                    return _foodData[index];
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

        private bool CanSpawn()
        {
            return _foodPool != null && GetSpawnCount() > 0 && HasFoodData();
        }

        private bool CanLoadPrefab()
        {
            return _foodPrefabReference != null && _foodPrefabReference.RuntimeKeyIsValid();
        }

        private void ReleaseFoodPrefabHandle()
        {
            if (_foodPrefabHandle.IsValid())
            {
                Addressables.Release(_foodPrefabHandle);
                _foodPrefabHandle = default;
            }
        }

        private int GetSpawnCount()
        {
            return Mathf.Max(0, _spawnCount);
        }

        private int GetFoodDataCount()
        {
            return _foodData != null ? _foodData.Length : 0;
        }

        private bool HasFoodData()
        {
            if (_foodData == null)
            {
                return false;
            }

            for (var index = 0; index < _foodData.Length; index++)
            {
                if (_foodData[index] != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
