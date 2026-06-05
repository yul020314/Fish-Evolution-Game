using System;
using FishEvolution.Gameplay;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace FishEvolution.Audio
{
    public sealed class AudioManager : IStartable, IDisposable
    {
        private readonly AudioPlaybackSettings _settings;
        private readonly AudioMixerController _mixer;
        private readonly AudioSourceBinding _sourceBinding;
        private readonly ISubscriber<AudioVolumeRequest> _volumeSubscriber;
        private readonly IPublisher<AudioVolumeChangedEvent> _volumePublisher;
        private readonly ISubscriber<FoodConsumedEvent> _foodSubscriber;
        private readonly ISubscriber<PlayerLevelUpEvent> _levelUpSubscriber;
        private readonly ISubscriber<SkillUsedEvent> _skillSubscriber;

        private IDisposable _volumeSubscription;
        private IDisposable _foodSubscription;
        private IDisposable _levelUpSubscription;
        private IDisposable _skillSubscription;

        public AudioManager(
            AudioPlaybackSettings settings,
            AudioMixerController mixer,
            AudioSourceBinding sourceBinding,
            ISubscriber<AudioVolumeRequest> volumeSubscriber,
            IPublisher<AudioVolumeChangedEvent> volumePublisher,
            ISubscriber<FoodConsumedEvent> foodSubscriber,
            ISubscriber<PlayerLevelUpEvent> levelUpSubscriber,
            ISubscriber<SkillUsedEvent> skillSubscriber)
        {
            _settings = settings;
            _mixer = mixer;
            _sourceBinding = sourceBinding;
            _volumeSubscriber = volumeSubscriber;
            _volumePublisher = volumePublisher;
            _foodSubscriber = foodSubscriber;
            _levelUpSubscriber = levelUpSubscriber;
            _skillSubscriber = skillSubscriber;
        }

        public AudioVolumeSettings VolumeSettings => _mixer.Settings;

        public void Start()
        {
            ConfigureSources();
            SubscribeEvents();
            ApplyVolume();
            TryPlayBgm();
        }

        public void Dispose()
        {
            _volumeSubscription?.Dispose();
            _foodSubscription?.Dispose();
            _levelUpSubscription?.Dispose();
            _skillSubscription?.Dispose();
            StopBgm();
        }

        public void SetMasterVolume(float volume)
        {
            SetVolume(AudioChannel.Master, volume);
        }

        public void SetBgmVolume(float volume)
        {
            SetVolume(AudioChannel.Bgm, volume);
        }

        public void SetSfxVolume(float volume)
        {
            SetVolume(AudioChannel.Sfx, volume);
        }

        public void PlaySfx(AudioClip clip)
        {
            var sfxSource = _sourceBinding.SfxSource;
            if (clip == null || sfxSource == null)
            {
                return;
            }

            sfxSource.PlayOneShot(clip);
        }

        private void ConfigureSources()
        {
            ConfigureBgmSource();
            ConfigureSfxSource();
        }

        private void ConfigureBgmSource()
        {
            var bgmSource = _sourceBinding.BgmSource;
            if (bgmSource == null)
            {
                return;
            }

            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.clip = _settings.BgmClip;
            bgmSource.outputAudioMixerGroup = _settings.BgmMixerGroup;
        }

        private void ConfigureSfxSource()
        {
            var sfxSource = _sourceBinding.SfxSource;
            if (sfxSource == null)
            {
                return;
            }

            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.outputAudioMixerGroup = _settings.SfxMixerGroup;
        }

        private void SubscribeEvents()
        {
            _volumeSubscription = _volumeSubscriber.Subscribe(HandleVolumeRequest);
            _foodSubscription = _foodSubscriber.Subscribe(HandleFoodConsumed);
            _levelUpSubscription = _levelUpSubscriber.Subscribe(HandleLevelUp);
            _skillSubscription = _skillSubscriber.Subscribe(HandleSkillUsed);
        }

        private void TryPlayBgm()
        {
            if (!_settings.PlayBgmOnStart ||
                _sourceBinding.BgmSource == null ||
                _sourceBinding.BgmSource.clip == null)
            {
                return;
            }

            _sourceBinding.BgmSource.Play();
        }

        private void StopBgm()
        {
            var bgmSource = _sourceBinding.BgmSource;
            if (bgmSource != null && bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
        }

        private void HandleVolumeRequest(AudioVolumeRequest request)
        {
            SetVolume(request.Channel, request.Volume);
        }

        private void SetVolume(
            AudioChannel channel,
            float volume)
        {
            var settings = _mixer.SetVolume(channel, volume);
            ApplyVolume();
            _volumePublisher.Publish(new AudioVolumeChangedEvent(settings));
        }

        private void ApplyVolume()
        {
            if (_sourceBinding.BgmSource != null)
            {
                _sourceBinding.BgmSource.volume = _mixer.BgmVolume;
            }

            if (_sourceBinding.SfxSource != null)
            {
                _sourceBinding.SfxSource.volume = _mixer.SfxVolume;
            }
        }

        private void HandleFoodConsumed(FoodConsumedEvent message)
        {
            PlaySfx(_settings.EatSfx);
        }

        private void HandleLevelUp(PlayerLevelUpEvent message)
        {
            PlaySfx(_settings.LevelUpSfx);
        }

        private void HandleSkillUsed(SkillUsedEvent message)
        {
            PlaySfx(_settings.SkillSfx);
        }
    }
}
