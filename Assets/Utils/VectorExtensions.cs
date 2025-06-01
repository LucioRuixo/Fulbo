using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 Horizontal(this Vector3 value) => new Vector3(value.x, 0f, value.z);

    public static Vector3 HorizontalVectorTo(this Vector3 value, Vector3 to) => (to - value).Horizontal();

    public static Vector3 HorizontalDirectionTo(this Vector3 value, Vector3 to) => value.HorizontalVectorTo(to).normalized;

    public static float HorizontalDistanceTo(this Vector3 value, Vector3 to) => value.HorizontalVectorTo(to).magnitude;

    public static float HorizontalSqrDistanceTo(this Vector3 value, Vector3 to) => value.HorizontalVectorTo(to).sqrMagnitude;
}
