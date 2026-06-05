using System.Collections.Generic;
using UnityEngine;

namespace FishEvolution.VFX
{
    public sealed class EffectPool
    {
        private readonly ParticleSystem _prefab;
        private readonly Transform _parent;
        private readonly Stack<ParticleSystem> _inactiveEffects;

        public EffectPool(
            ParticleSystem prefab,
            Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _inactiveEffects = new Stack<ParticleSystem>();
        }

        public void Prewarm(int count)
        {
            for (var index = 0; index < count; index++)
            {
                Release(CreateEffect());
            }
        }

        public ParticleSystem Get()
        {
            var effect = _inactiveEffects.Count > 0
                ? _inactiveEffects.Pop()
                : CreateEffect();

            effect.gameObject.SetActive(true);
            return effect;
        }

        public void Release(ParticleSystem effect)
        {
            if (effect == null)
            {
                return;
            }

            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effect.transform.SetParent(_parent, false);
            effect.gameObject.SetActive(false);
            _inactiveEffects.Push(effect);
        }

        public void Clear()
        {
            while (_inactiveEffects.Count > 0)
            {
                var effect = _inactiveEffects.Pop();
                Object.Destroy(effect.gameObject);
            }
        }

        private ParticleSystem CreateEffect()
        {
            var effect = Object.Instantiate(_prefab, _parent);
            effect.gameObject.SetActive(false);
            return effect;
        }
    }
}
