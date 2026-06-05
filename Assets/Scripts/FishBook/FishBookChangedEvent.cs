namespace FishEvolution.FishBook
{
    public readonly struct FishBookChangedEvent
    {
        public FishBookChangedEvent(FishBookEntry[] entries)
        {
            Entries = entries;
        }

        public FishBookEntry[] Entries { get; }
    }
}
