namespace FishEvolution.Audio
{
    public readonly struct AudioVolumeRequest
    {
        public AudioVolumeRequest(
            AudioChannel channel,
            float volume)
        {
            Channel = channel;
            Volume = volume;
        }

        public AudioChannel Channel { get; }
        public float Volume { get; }
    }
}
