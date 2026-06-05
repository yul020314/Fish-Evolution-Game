using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FishEvolution.VFX
{
    [Serializable]
    public sealed class VFXCue
    {
        [SerializeField] private VFXType _type;
        [SerializeField] private AssetReferenceGameObject _prefabReference;
        [SerializeField] private int _prewarmCount = 4;
        [SerializeField] private float _fallbackLifetime = 1.5f;
        [SerializeField] private Vector3 _offset;

        public VFXType Type => _type;
        public AssetReferenceGameObject PrefabReference => _prefabReference;
        public int PrewarmCount => Mathf.Max(0, _prewarmCount);
        public float FallbackLifetime => Mathf.Max(0.1f, _fallbackLifetime);
        public Vector3 Offset => _offset;
        public bool IsValid => _prefabReference != null &&
            _prefabReference.RuntimeKeyIsValid();
    }
}
