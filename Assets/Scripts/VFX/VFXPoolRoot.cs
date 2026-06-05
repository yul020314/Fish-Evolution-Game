using UnityEngine;

namespace FishEvolution.VFX
{
    public sealed class VFXPoolRoot
    {
        public VFXPoolRoot(Transform root)
        {
            Root = root;
        }

        public Transform Root { get; }
    }
}
