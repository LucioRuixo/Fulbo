using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 Horizontal(this Vector3 value) => new Vector3(value.x, 0f, value.z);
}
