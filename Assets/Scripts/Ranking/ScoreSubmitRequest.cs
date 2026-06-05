namespace FishEvolution.Ranking
{
    public readonly struct ScoreSubmitRequest
    {
        public ScoreSubmitRequest(string playerName)
        {
            PlayerName = playerName;
        }

        public string PlayerName { get; }
        public bool IsValid => !string.IsNullOrWhiteSpace(PlayerName);
    }
}
