using System.Collections.Generic;
using FishEvolution.Gameplay;
using UnityEngine;

namespace FishEvolution.Pool
{
    public sealed class FishPool
    {
        private readonly FishController _prefab;
        private readonly Transform _parent;
        private readonly Stack<FishController> _inactiveFish = new Stack<FishController>();

        public FishPool(FishController prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public void Prewarm(int count)
        {
            for (var index = 0; index < count; index++)
            {
                Release(CreateFish());
            }
        }

        public FishController Get()
        {
            var fish = _inactiveFish.Count > 0
                ? _inactiveFish.Pop()
                : CreateFish();

            fish.gameObject.SetActive(true);
            return fish;
        }

        public void Release(FishController fish)
        {
            if (fish == null)
            {
                return;
            }

            fish.transform.SetParent(_parent, false);
            fish.gameObject.SetActive(false);
            _inactiveFish.Push(fish);
        }

        public void Clear()
        {
            while (_inactiveFish.Count > 0)
            {
                var fish = _inactiveFish.Pop();
                Object.Destroy(fish.gameObject);
            }
        }

        private FishController CreateFish()
        {
            var fish = Object.Instantiate(_prefab, _parent);
            fish.gameObject.SetActive(false);
            return fish;
        }
    }
}
