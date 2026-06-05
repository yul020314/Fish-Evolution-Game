namespace FishEvolution.VFX
{
    public sealed class VFXPlaybackSettings
    {
        private readonly VFXCue[] _cues;

        public VFXPlaybackSettings(VFXCue[] cues)
        {
            _cues = cues ?? new VFXCue[0];
        }

        public VFXCue[] Cues => _cues;

        public bool TryGetCue(
            VFXType type,
            out VFXCue cue)
        {
            for (var index = 0; index < _cues.Length; index++)
            {
                cue = _cues[index];
                if (cue != null && cue.Type == type && cue.IsValid)
                {
                    return true;
                }
            }

            cue = null;
            return false;
        }
    }
}
