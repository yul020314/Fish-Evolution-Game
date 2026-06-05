using UnityEngine;

namespace FishEvolution.Gameplay
{
    public sealed class LevelSystem
    {
        private readonly int _maxLevel;
        private readonly int _experienceMultiplier;
        private readonly float _experiencePower;
        private int _currentLevel;

        public LevelSystem(
            int initialLevel,
            int maxLevel,
            int experienceMultiplier,
            float experiencePower)
        {
            _maxLevel = Mathf.Max(1, maxLevel);
            _experienceMultiplier = Mathf.Max(1, experienceMultiplier);
            _experiencePower = Mathf.Max(0.01f, experiencePower);
            _currentLevel = Mathf.Clamp(initialLevel, 1, _maxLevel);
        }

        public int CurrentLevel => _currentLevel;
        public int MaxLevel => _maxLevel;
        public bool IsMaxLevel => _currentLevel >= _maxLevel;

        public int GetRequiredExperience()
        {
            if (IsMaxLevel)
            {
                return 0;
            }

            var required = _experienceMultiplier * Mathf.Pow(_currentLevel, _experiencePower);
            return Mathf.FloorToInt(required);
        }

        public bool TryLevelUp(ExpSystem expSystem)
        {
            if (expSystem == null || IsMaxLevel)
            {
                return false;
            }

            var requiredExperience = GetRequiredExperience();
            if (!expSystem.ConsumeExperience(requiredExperience))
            {
                return false;
            }

            _currentLevel++;
            return true;
        }
    }
}
