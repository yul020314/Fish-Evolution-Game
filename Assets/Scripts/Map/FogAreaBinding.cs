using System;
using FishEvolution.Config;
using UnityEngine;

namespace FishEvolution.Map
{
    [Serializable]
    public sealed class FogAreaBinding
    {
        [SerializeField] private MapAreaType _areaType;
        [SerializeField] private GameObject _fogRoot;

        public MapAreaType AreaType => _areaType;
        public bool IsValid => _fogRoot != null;

        public void SetVisible(bool isVisible)
        {
            if (_fogRoot != null)
            {
                _fogRoot.SetActive(isVisible);
            }
        }
    }
}
