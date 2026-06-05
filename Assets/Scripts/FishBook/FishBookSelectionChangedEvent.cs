namespace FishEvolution.FishBook
{
    public readonly struct FishBookSelectionChangedEvent
    {
        public FishBookSelectionChangedEvent(FishBookEntry entry)
        {
            Entry = entry;
        }

        public FishBookEntry Entry { get; }
    }
}
