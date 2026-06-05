namespace FishEvolution.Ads
{
    public readonly struct AdCompletedEvent
    {
        public AdCompletedEvent(AdType adType)
        {
            AdType = adType;
        }

        public AdType AdType { get; }
    }
}
