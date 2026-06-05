using FishEvolution.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "AudioSettings", menuName = "Fish Evolution/Config/Audio Settings")]
    public sealed class AudioSettingsSO : ScriptableObject
    {
        [SerializeField] private AudioClip _bgmClip;
        [SerializeField] private AudioClip _eatSfx;
        [SerializeField] private AudioClip _levelUpSfx;
        [SerializeField] private AudioClip _skillSfx;
        [SerializeField] private AudioMixerGroup _bgmMixerGroup;
        [SerializeField] private AudioMixerGroup _sfxMixerGroup;
        [SerializeField, Range(0f, 1f)] private float _masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float _bgmVolume = 0.75f;
        [SerializeField, Range(0f, 1f)] private float _sfxVolume = 1f;
        [SerializeField] private bool _playBgmOnStart = true;

        public AudioClip BgmClip => _bgmClip;
        public AudioClip EatSfx => _eatSfx;
        public AudioClip LevelUpSfx => _levelUpSfx;
        public AudioClip SkillSfx => _skillSfx;
        public AudioMixerGroup BgmMixerGroup => _bgmMixerGroup;
        public AudioMixerGroup SfxMixerGroup => _sfxMixerGroup;
        public bool PlayBgmOnStart => _playBgmOnStart;

        public AudioPlaybackSettings CreatePlaybackSettings()
        {
            return new AudioPlaybackSettings(
                _bgmClip,
                _eatSfx,
                _levelUpSfx,
                _skillSfx,
                _bgmMixerGroup,
                _sfxMixerGroup,
                CreateVolumeSettings(),
                _playBgmOnStart);
        }

        public AudioVolumeSettings CreateVolumeSettings()
        {
            return new AudioVolumeSettings(
                _masterVolume,
                _bgmVolume,
                _sfxVolume);
        }
    }
}
