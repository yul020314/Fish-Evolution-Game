using UnityEngine;

namespace FishEvolution.Audio
{
    public sealed class AudioSourceBinding
    {
        public AudioSourceBinding(
            AudioSource bgmSource,
            AudioSource sfxSource)
        {
            BgmSource = bgmSource;
            SfxSource = sfxSource;
        }

        public AudioSource BgmSource { get; }
        public AudioSource SfxSource { get; }
    }
}
