using UnityEngine;

namespace FishEvolution.Audio
{
    public readonly struct AudioVolumeSettings
    {
        public AudioVolumeSettings(
            float masterVolume,
            float bgmVolume,
            float sfxVolume)
        {
            MasterVolume = Mathf.Clamp01(masterVolume);
            BgmVolume = Mathf.Clamp01(bgmVolume);
            SfxVolume = Mathf.Clamp01(sfxVolume);
        }

        public float MasterVolume { get; }
        public float BgmVolume { get; }
        public float SfxVolume { get; }
        public float MixedBgmVolume => MasterVolume * BgmVolume;
        public float MixedSfxVolume => MasterVolume * SfxVolume;

        public AudioVolumeSettings With(
            AudioChannel channel,
            float volume)
        {
            var clamped = Mathf.Clamp01(volume);
            return channel switch
            {
                AudioChannel.Master => new AudioVolumeSettings(
                    clamped,
                    BgmVolume,
                    SfxVolume),
                AudioChannel.Bgm => new AudioVolumeSettings(
                    MasterVolume,
                    clamped,
                    SfxVolume),
                AudioChannel.Sfx => new AudioVolumeSettings(
                    MasterVolume,
                    BgmVolume,
                    clamped),
                _ => this
            };
        }
    }
}
