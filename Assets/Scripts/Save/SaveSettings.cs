using UnityEngine;

namespace FishEvolution.Save
{
    public sealed class SaveSettings
    {
        private const string DefaultFileName = "player_progress.json";
        private const float DefaultAutoSaveInterval = 10f;

        public SaveSettings(
            string fileName,
            float autoSaveInterval)
        {
            FileName = string.IsNullOrWhiteSpace(fileName)
                ? DefaultFileName
                : fileName;
            AutoSaveInterval = Mathf.Max(1f, autoSaveInterval);
        }

        public string FileName { get; }
        public float AutoSaveInterval { get; }
    }
}
