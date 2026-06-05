using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace FishEvolution.Save
{
    public sealed class JsonSave
    {
        private readonly SaveSettings _settings;

        public JsonSave(SaveSettings settings)
        {
            _settings = settings;
        }

        public bool TryLoad(out SaveData saveData)
        {
            saveData = null;
            var path = GetSavePath();
            if (!File.Exists(path))
            {
                return false;
            }

            return TryRead(path, out saveData);
        }

        public bool Save(SaveData saveData)
        {
            if (saveData == null)
            {
                return false;
            }

            var path = GetSavePath();
            EnsureDirectory(path);
            return TryWrite(path, saveData);
        }

        private string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, _settings.FileName);
        }

        private static bool TryRead(
            string path,
            out SaveData saveData)
        {
            saveData = null;
            try
            {
                var json = File.ReadAllText(path);
                saveData = JsonConvert.DeserializeObject<SaveData>(json);
                return saveData != null;
            }
            catch (Exception exception) when (IsFileException(exception))
            {
                Debug.LogWarning($"Load save failed: {exception.Message}");
                return false;
            }
        }

        private static bool TryWrite(
            string path,
            SaveData saveData)
        {
            try
            {
                var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception exception) when (IsFileException(exception))
            {
                Debug.LogWarning($"Write save failed: {exception.Message}");
                return false;
            }
        }

        private static void EnsureDirectory(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static bool IsFileException(Exception exception)
        {
            return exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is JsonException;
        }
    }
}
