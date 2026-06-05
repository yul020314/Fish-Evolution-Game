using FishEvolution.Config;

namespace FishEvolution.UI
{
    public sealed class HudTextFormatter
    {
        private readonly string _levelPrefix;
        private readonly string _experiencePrefix;
        private readonly string _goldPrefix;
        private readonly string _skillPrefix;
        private readonly string _mapPrefix;
        private readonly string _noneText;

        public HudTextFormatter(
            string levelPrefix,
            string experiencePrefix,
            string goldPrefix,
            string skillPrefix,
            string mapPrefix,
            string noneText)
        {
            _levelPrefix = levelPrefix;
            _experiencePrefix = experiencePrefix;
            _goldPrefix = goldPrefix;
            _skillPrefix = skillPrefix;
            _mapPrefix = mapPrefix;
            _noneText = noneText;
        }

        public string GetLevelText(int level)
        {
            return $"{_levelPrefix}{level}";
        }

        public string GetExperienceText(
            int currentExperience,
            int requiredExperience)
        {
            return requiredExperience > 0
                ? $"{_experiencePrefix}{currentExperience}/{requiredExperience}"
                : $"{_experiencePrefix}{currentExperience}";
        }

        public string GetGoldText(int gold)
        {
            return $"{_goldPrefix}{gold}";
        }

        public string GetSkillText(SkillType skill)
        {
            var skillName = skill == SkillType.None ? _noneText : skill.ToString();
            return $"{_skillPrefix}{skillName}";
        }

        public string GetMapText(string mapName)
        {
            return $"{_mapPrefix}{mapName}";
        }
    }
}
