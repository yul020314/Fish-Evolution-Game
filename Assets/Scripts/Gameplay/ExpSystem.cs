using UnityEngine;

namespace FishEvolution.Gameplay
{
    public sealed class ExpSystem
    {
        private int _currentExperience;

        public int CurrentExperience => _currentExperience;

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _currentExperience = Mathf.Max(0, _currentExperience + amount);
        }

        public void SetExperience(int amount)
        {
            _currentExperience = Mathf.Max(0, amount);
        }

        public bool ConsumeExperience(int amount)
        {
            if (amount <= 0 || _currentExperience < amount)
            {
                return false;
            }

            _currentExperience -= amount;
            return true;
        }
    }
}
