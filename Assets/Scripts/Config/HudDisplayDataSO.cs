using UnityEngine;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "HudDisplayData", menuName = "Fish Evolution/Config/HUD Display Data")]
    public sealed class HudDisplayDataSO : ScriptableObject
    {
        [SerializeField] private string _fontName = "Arial";
        [SerializeField] private string _levelPrefix = "Lv ";
        [SerializeField] private string _experiencePrefix = "EXP ";
        [SerializeField] private string _goldPrefix = "Gold ";
        [SerializeField] private string _skillPrefix = "Skill ";
        [SerializeField] private string _mapPrefix = "Map ";
        [SerializeField] private string _defaultMapName = "Shallow Sea";
        [SerializeField] private string _noneText = "None";
        [SerializeField] private int _initialGold = 0;
        [SerializeField] private Color _panelColor = new Color(0.04f, 0.09f, 0.12f, 0.7f);
        [SerializeField] private Color _textColor = new Color(0.93f, 0.98f, 1f, 1f);
        [SerializeField] private Color _barBackgroundColor = new Color(0.04f, 0.12f, 0.18f, 0.85f);
        [SerializeField] private Color _barFillColor = new Color(0.22f, 0.78f, 0.88f, 1f);
        [SerializeField] private Color _mapColor = new Color(0.04f, 0.18f, 0.22f, 0.78f);
        [SerializeField] private Color _playerMarkerColor = new Color(1f, 0.86f, 0.24f, 1f);

        public string FontName => _fontName;
        public string LevelPrefix => _levelPrefix;
        public string ExperiencePrefix => _experiencePrefix;
        public string GoldPrefix => _goldPrefix;
        public string SkillPrefix => _skillPrefix;
        public string MapPrefix => _mapPrefix;
        public string DefaultMapName => _defaultMapName;
        public string NoneText => _noneText;
        public int InitialGold => _initialGold;
        public Color PanelColor => _panelColor;
        public Color TextColor => _textColor;
        public Color BarBackgroundColor => _barBackgroundColor;
        public Color BarFillColor => _barFillColor;
        public Color MapColor => _mapColor;
        public Color PlayerMarkerColor => _playerMarkerColor;
    }
}
