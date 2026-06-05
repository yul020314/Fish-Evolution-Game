namespace FishEvolution.Ads
{
    public readonly struct AdFailedEvent
    {
        public AdFailedEvent(
            AdType adType,
            string reason)
        {
            AdType = adType;
            Reason = reason;
        }

        public AdType AdType { get; }
        public string Reason { get; }
    }
}
