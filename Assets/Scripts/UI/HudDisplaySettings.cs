using FishEvolution.Config;
using FishEvolution.Gameplay;
using MessagePipe;
using System;
using UnityEngine;
using VContainer.Unity;

namespace FishEvolution.UI
{
    public sealed class HudDisplaySettings
    {
        public HudDisplaySettings(
            string fontName,
            string levelPrefix,
            string experiencePrefix,
            string goldPrefix,
            string skillPrefix,
            string mapPrefix,
            string defaultMapName,
            string noneText,
            int initialGold,
            Color panelColor,
            Color textColor,
            Color barBackgroundColor,
            Color barFillColor,
            Color mapColor,
            Color playerMarkerColor)
        {
            FontName = fontName;
            LevelPrefix = levelPrefix;
            ExperiencePrefix = experiencePrefix;
            GoldPrefix = goldPrefix;
            SkillPrefix = skillPrefix;
            MapPrefix = mapPrefix;
            DefaultMapName = defaultMapName;
            NoneText = noneText;
            InitialGold = initialGold;
            PanelColor = panelColor;
            TextColor = textColor;
            BarBackgroundColor = barBackgroundColor;
            BarFillColor = barFillColor;
            MapColor = mapColor;
            PlayerMarkerColor = playerMarkerColor;
        }

        public string FontName { get; }
        public string LevelPrefix { get; }
        public string ExperiencePrefix { get; }
        public string GoldPrefix { get; }
        public string SkillPrefix { get; }
        public string MapPrefix { get; }
        public string DefaultMapName { get; }
        public string NoneText { get; }
        public int InitialGold { get; }
        public Color PanelColor { get; }
        public Color TextColor { get; }
        public Color BarBackgroundColor { get; }
        public Color BarFillColor { get; }
        public Color MapColor { get; }
        public Color PlayerMarkerColor { get; }

        public static HudDisplaySettings FromData(HudDisplayDataSO data)
        {
            if (data == null)
            {
                return null;
            }

            return new HudDisplaySettings(
                data.FontName,
                data.LevelPrefix,
                data.ExperiencePrefix,
                data.GoldPrefix,
                data.SkillPrefix,
                data.MapPrefix,
                data.DefaultMapName,
                data.NoneText,
                data.InitialGold,
                data.PanelColor,
                data.TextColor,
                data.BarBackgroundColor,
                data.BarFillColor,
                data.MapColor,
                data.PlayerMarkerColor);
        }
    }

    public sealed class GameplayHudController : IStartable, ITickable, IDisposable
    {
        private readonly HudDisplaySettings _settings;
        private readonly PlayerController _player;
        private readonly PlayerGrowthController _growth;
        private readonly FishSpawner _fishSpawner;
        private readonly ISubscriber<FoodConsumedEvent> _foodConsumedSubscriber;
        private readonly ISubscriber<PlayerLevelUpEvent> _levelUpSubscriber;
        private readonly ISubscriber<PlayerProgressChangedEvent> _progressChangedSubscriber;

        private GameplayHudBinding _binding;
        private HudTextFormatter _formatter;
        private IDisposable _foodConsumedSubscription;
        private IDisposable _levelUpSubscription;
        private IDisposable _progressChangedSubscription;
        private bool _isHudDirty;

        public GameplayHudController(
            HudDisplaySettings settings,
            PlayerController player,
            PlayerGrowthController growth,
            FishSpawner fishSpawner,
            ISubscriber<FoodConsumedEvent> foodConsumedSubscriber,
            ISubscriber<PlayerLevelUpEvent> levelUpSubscriber,
            ISubscriber<PlayerProgressChangedEvent> progressChangedSubscriber)
        {
            _settings = settings;
            _player = player;
            _growth = growth;
            _fishSpawner = fishSpawner;
            _foodConsumedSubscriber = foodConsumedSubscriber;
            _levelUpSubscriber = levelUpSubscriber;
            _progressChangedSubscriber = progressChangedSubscriber;
        }

        public void Start()
        {
            _formatter = CreateFormatter();
            _binding = CreateView();
            SubscribeEvents();
            RefreshHud();
        }

        public void Tick()
        {
            if (_isHudDirty)
            {
                RefreshHud();
                return;
            }

            RefreshMinimap();
        }

        public void Dispose()
        {
            _foodConsumedSubscription?.Dispose();
            _levelUpSubscription?.Dispose();
            _progressChangedSubscription?.Dispose();
            _foodConsumedSubscription = null;
            _levelUpSubscription = null;
            _progressChangedSubscription = null;
            DestroyHud();
        }

        private void SubscribeEvents()
        {
            _foodConsumedSubscription = _foodConsumedSubscriber.Subscribe(HandleFoodConsumed);
            _levelUpSubscription = _levelUpSubscriber.Subscribe(HandleLevelUp);
            _progressChangedSubscription = _progressChangedSubscriber.Subscribe(
                HandleProgressChanged);
        }

        private void HandleFoodConsumed(FoodConsumedEvent message)
        {
            if (message.Player == _player)
            {
                _isHudDirty = true;
            }
        }

        private void HandleLevelUp(PlayerLevelUpEvent message)
        {
            if (message.Player == _player)
            {
                _isHudDirty = true;
            }
        }

        private void HandleProgressChanged(PlayerProgressChangedEvent message)
        {
            if (message.Player == _player)
            {
                _isHudDirty = true;
            }
        }

        private void RefreshHud()
        {
            if (_formatter == null)
            {
                return;
            }

            RefreshGrowthText();
            RefreshResourceText();
            RefreshMinimap();
            _isHudDirty = false;
        }

        private void RefreshGrowthText()
        {
            var currentExperience = _growth != null ? _growth.CurrentExperience : 0;
            var requiredExperience = _growth != null ? _growth.RequiredExperience : 0;
            SetText(_binding.LevelText, _formatter.GetLevelText(GetCurrentLevel()));
            SetText(
                _binding.ExperienceText,
                _formatter.GetExperienceText(currentExperience, requiredExperience));
            _binding.ExperienceBar?.SetValue(currentExperience, requiredExperience);
        }

        private void RefreshResourceText()
        {
            SetText(_binding.GoldText, _formatter.GetGoldText(_settings.InitialGold));
            SetText(_binding.SkillText, _formatter.GetSkillText(GetCurrentSkill()));
            SetText(_binding.MapText, _formatter.GetMapText(_settings.DefaultMapName));
        }

        private void RefreshMinimap()
        {
            if (_binding.PlayerMarker == null || _player == null || _fishSpawner == null)
            {
                return;
            }

            var mapSize = _binding.PlayerMarker.parent as RectTransform;
            if (mapSize == null)
            {
                return;
            }

            _binding.PlayerMarker.anchoredPosition = GetMarkerPosition(mapSize.rect.size);
        }

        private Vector2 GetMarkerPosition(Vector2 mapSize)
        {
            var center = _fishSpawner.SpawnCenter;
            var size = _fishSpawner.SpawnSize;
            var playerPosition = (Vector2)_player.transform.position;
            var normalized = new Vector2(
                GetNormalized(playerPosition.x, center.x, size.x),
                GetNormalized(playerPosition.y, center.y, size.y));
            return new Vector2(
                normalized.x * mapSize.x - mapSize.x * 0.5f,
                normalized.y * mapSize.y - mapSize.y * 0.5f);
        }

        private static float GetNormalized(
            float value,
            float center,
            float size)
        {
            if (size <= 0f)
            {
                return 0.5f;
            }

            var min = center - size * 0.5f;
            return Mathf.Clamp01((value - min) / size);
        }

        private int GetCurrentLevel()
        {
            return _growth != null ? _growth.CurrentLevel : 1;
        }

        private SkillType GetCurrentSkill()
        {
            return _player != null && _player.FishData != null
                ? _player.FishData.Skill
                : SkillType.None;
        }

        private static void SetText(UnityEngine.UI.Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private HudTextFormatter CreateFormatter()
        {
            return new HudTextFormatter(
                _settings.LevelPrefix,
                _settings.ExperiencePrefix,
                _settings.GoldPrefix,
                _settings.SkillPrefix,
                _settings.MapPrefix,
                _settings.NoneText);
        }

        private GameplayHudBinding CreateView()
        {
            var view = new GameplayHudView(
                _settings.FontName,
                _settings.PanelColor,
                _settings.TextColor,
                _settings.BarBackgroundColor,
                _settings.BarFillColor,
                _settings.MapColor,
                _settings.PlayerMarkerColor);
            return view.Create();
        }

        private void DestroyHud()
        {
            if (_binding.Root != null)
            {
                UnityEngine.Object.Destroy(_binding.Root);
            }
        }
    }
}
