namespace FishEvolution.FishBook
{
    public readonly struct FishBookSelectionRequest
    {
        public FishBookSelectionRequest(string fishId)
        {
            FishId = fishId;
        }

        public string FishId { get; }
        public bool IsValid => !string.IsNullOrWhiteSpace(FishId);
    }
}
