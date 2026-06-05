using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

namespace FishEvolution.VFX
{
    public sealed class VFXPoolEntry
    {
        public VFXPoolEntry(
            VFXCue cue,
            EffectPool pool,
            AsyncOperationHandle<GameObject> prefabHandle)
        {
            Cue = cue;
            Pool = pool;
            PrefabHandle = prefabHandle;
        }

        public VFXCue Cue { get; }
        public EffectPool Pool { get; }
        public AsyncOperationHandle<GameObject> PrefabHandle { get; }
    }
}
