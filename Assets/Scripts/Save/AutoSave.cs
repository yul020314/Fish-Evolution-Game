using System;
using UnityEngine;
using VContainer.Unity;

namespace FishEvolution.Save
{
    public sealed class AutoSave : IStartable, ITickable, IDisposable
    {
        private readonly SaveManager _saveManager;
        private readonly SaveSettings _settings;
        private float _nextSaveTime;
        private bool _isLoaded;

        public AutoSave(
            SaveManager saveManager,
            SaveSettings settings)
        {
            _saveManager = saveManager;
            _settings = settings;
        }

        public void Start()
        {
            _nextSaveTime = Time.time + _settings.AutoSaveInterval;
        }

        public void Tick()
        {
            EnsureLoaded();
            if (Time.time < _nextSaveTime)
            {
                return;
            }

            _saveManager.Save();
            _nextSaveTime = Time.time + _settings.AutoSaveInterval;
        }

        public void Dispose()
        {
            _saveManager.Save();
        }

        private void EnsureLoaded()
        {
            if (_isLoaded)
            {
                return;
            }

            _saveManager.Load();
            _isLoaded = true;
        }
    }
}
