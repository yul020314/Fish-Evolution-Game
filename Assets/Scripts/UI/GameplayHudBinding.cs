using UnityEngine;
using UnityEngine.UI;

namespace FishEvolution.UI
{
    public readonly struct GameplayHudBinding
    {
        public GameplayHudBinding(
            GameObject root,
            Text levelText,
            Text experienceText,
            Text goldText,
            Text skillText,
            Text mapText,
            HudProgressBar experienceBar,
            RectTransform playerMarker)
        {
            Root = root;
            LevelText = levelText;
            ExperienceText = experienceText;
            GoldText = goldText;
            SkillText = skillText;
            MapText = mapText;
            ExperienceBar = experienceBar;
            PlayerMarker = playerMarker;
        }

        public GameObject Root { get; }
        public Text LevelText { get; }
        public Text ExperienceText { get; }
        public Text GoldText { get; }
        public Text SkillText { get; }
        public Text MapText { get; }
        public HudProgressBar ExperienceBar { get; }
        public RectTransform PlayerMarker { get; }
    }
}
