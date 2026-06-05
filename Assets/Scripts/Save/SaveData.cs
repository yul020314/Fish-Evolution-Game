using UnityEngine;

namespace FishEvolution.Save
{
    public sealed class SaveData
    {
        public int Version { get; set; } = 1;
        public PlayerSaveData Player { get; set; } = new PlayerSaveData();
    }

    public sealed class PlayerSaveData
    {
        public int Level { get; set; } = 1;
        public int Experience { get; set; }
        public SaveVector3 Position { get; set; } = SaveVector3.Zero;
    }

    public readonly struct SaveVector3
    {
        public SaveVector3(
            float x,
            float y,
            float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public static SaveVector3 Zero => new SaveVector3(0f, 0f, 0f);

        public static SaveVector3 FromVector3(Vector3 value)
        {
            return new SaveVector3(value.x, value.y, value.z);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
