using System.Collections.Generic;
using FishEvolution.Gameplay;
using UnityEngine;

namespace FishEvolution.Pool
{
    public sealed class FoodPool
    {
        private readonly FoodController _prefab;
        private readonly Transform _parent;
        private readonly Stack<FoodController> _inactiveFoods;

        public FoodPool(FoodController prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _inactiveFoods = new Stack<FoodController>();
        }

        public FoodPool(
            FoodController prefab,
            Transform parent,
            int prewarmCapacity)
        {
            _prefab = prefab;
            _parent = parent;
            _inactiveFoods = new Stack<FoodController>(
                Mathf.Max(0, prewarmCapacity));
        }

        public void Prewarm(int count)
        {
            for (var index = 0; index < count; index++)
            {
                Release(CreateFood());
            }
        }

        public FoodController Get()
        {
            var food = _inactiveFoods.Count > 0
                ? _inactiveFoods.Pop()
                : CreateFood();

            food.gameObject.SetActive(true);
            return food;
        }

        public void Release(FoodController food)
        {
            if (food == null)
            {
                return;
            }

            food.transform.SetParent(_parent, false);
            food.gameObject.SetActive(false);
            _inactiveFoods.Push(food);
        }

        public void Clear()
        {
            while (_inactiveFoods.Count > 0)
            {
                var food = _inactiveFoods.Pop();
                Object.Destroy(food.gameObject);
            }
        }

        private FoodController CreateFood()
        {
            var food = Object.Instantiate(_prefab, _parent);
            food.gameObject.SetActive(false);
            return food;
        }
    }
}
