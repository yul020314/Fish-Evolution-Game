using UnityEngine;
using UnityEngine.Audio;

namespace FishEvolution.Audio
{
    public sealed class AudioPlaybackSettings
    {
        public AudioPlaybackSettings(
            AudioClip bgmClip,
            AudioClip eatSfx,
            AudioClip levelUpSfx,
            AudioClip skillSfx,
            AudioMixerGroup bgmMixerGroup,
            AudioMixerGroup sfxMixerGroup,
            AudioVolumeSettings volumeSettings,
            bool playBgmOnStart)
        {
            BgmClip = bgmClip;
            EatSfx = eatSfx;
            LevelUpSfx = levelUpSfx;
            SkillSfx = skillSfx;
            BgmMixerGroup = bgmMixerGroup;
            SfxMixerGroup = sfxMixerGroup;
            VolumeSettings = volumeSettings;
            PlayBgmOnStart = playBgmOnStart;
        }

        public AudioClip BgmClip { get; }
        public AudioClip EatSfx { get; }
        public AudioClip LevelUpSfx { get; }
        public AudioClip SkillSfx { get; }
        public AudioMixerGroup BgmMixerGroup { get; }
        public AudioMixerGroup SfxMixerGroup { get; }
        public AudioVolumeSettings VolumeSettings { get; }
        public bool PlayBgmOnStart { get; }

        public static AudioPlaybackSettings CreateDefault()
        {
            return new AudioPlaybackSettings(
                null,
                null,
                null,
                null,
                null,
                null,
                new AudioVolumeSettings(1f, 0.75f, 1f),
                true);
        }
    }
}
