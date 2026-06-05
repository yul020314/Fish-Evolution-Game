using FishEvolution.Gameplay;
using UnityEngine;

namespace FishEvolution.Save
{
    public sealed class SaveManager
    {
        private readonly JsonSave _jsonSave;
        private readonly PlayerController _player;
        private readonly PlayerGrowthController _growth;

        public SaveManager(
            JsonSave jsonSave,
            PlayerController player,
            PlayerGrowthController growth)
        {
            _jsonSave = jsonSave;
            _player = player;
            _growth = growth;
        }

        public bool Load()
        {
            if (!_jsonSave.TryLoad(out var saveData))
            {
                return false;
            }

            Apply(saveData);
            return true;
        }

        public bool Save()
        {
            return _jsonSave.Save(Capture());
        }

        private SaveData Capture()
        {
            var data = new SaveData();
            data.Player.Level = _growth != null ? _growth.CurrentLevel : 1;
            data.Player.Experience = _growth != null ? _growth.CurrentExperience : 0;
            data.Player.Position = GetPlayerPosition();
            return data;
        }

        private void Apply(SaveData saveData)
        {
            if (saveData?.Player == null)
            {
                return;
            }

            _growth?.RestoreProgress(
                saveData.Player.Level,
                saveData.Player.Experience);
            _player?.RestorePosition(saveData.Player.Position.ToVector3());
        }

        private SaveVector3 GetPlayerPosition()
        {
            var position = _player != null
                ? _player.transform.position
                : Vector3.zero;
            return SaveVector3.FromVector3(position);
        }
    }
}
