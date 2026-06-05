using FishEvolution.VFX;
using UnityEngine;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "VFXSettings", menuName = "Fish Evolution/Config/VFX Settings")]
    public sealed class VFXSettingsSO : ScriptableObject
    {
        [SerializeField] private VFXCue[] _cues = new VFXCue[0];

        public VFXPlaybackSettings CreatePlaybackSettings()
        {
            return new VFXPlaybackSettings(_cues);
        }
    }
}
