namespace FishEvolution.Audio
{
    public sealed class AudioMixerController
    {
        private AudioVolumeSettings _settings;

        public AudioMixerController(AudioVolumeSettings settings)
        {
            _settings = settings;
        }

        public AudioVolumeSettings Settings => _settings;
        public float BgmVolume => _settings.MixedBgmVolume;
        public float SfxVolume => _settings.MixedSfxVolume;

        public AudioVolumeSettings SetVolume(
            AudioChannel channel,
            float volume)
        {
            _settings = _settings.With(channel, volume);
            return _settings;
        }
    }
}
