namespace FishEvolution.AI
{
    public sealed class FishAIDecision
    {
        private readonly FishPerception _perception;
        private readonly FishAISettings _settings;

        public FishAIDecision(
            FishPerception perception,
            FishAISettings settings)
        {
            _perception = perception;
            _settings = settings;
        }

        public FishState GetDesiredState()
        {
            if (!_perception.IsPlayerDetected(_settings.DetectionRadius))
            {
                return FishState.Patrol;
            }

            if (_perception.ShouldEscape(
                _settings.EscapeDistance,
                _settings.SizeAdvantageMultiplier))
            {
                return FishState.Escape;
            }

            return _perception.ShouldChase(_settings.SizeAdvantageMultiplier)
                ? GetChaseState()
                : FishState.Patrol;
        }

        private FishState GetChaseState()
        {
            return _perception.GetPlayerDistance() <= _settings.AttackDistance
                ? FishState.Attack
                : FishState.Chase;
        }
    }
}
