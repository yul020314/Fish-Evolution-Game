namespace FishEvolution.Ads
{
    public readonly struct AdRequest
    {
        public AdRequest(AdType adType)
        {
            AdType = adType;
        }

        public AdType AdType { get; }
    }
}
