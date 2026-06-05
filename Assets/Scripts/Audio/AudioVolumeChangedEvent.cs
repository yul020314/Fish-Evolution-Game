namespace FishEvolution.Audio
{
    public readonly struct AudioVolumeChangedEvent
    {
        public AudioVolumeChangedEvent(AudioVolumeSettings settings)
        {
            Settings = settings;
        }

        public AudioVolumeSettings Settings { get; }
    }
}
