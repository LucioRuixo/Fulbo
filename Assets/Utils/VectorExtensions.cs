using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 VectorTo(this Vector3 value, Vector3 to) => to - value;

    public static Vector3 DirectionTo(this Vector3 value, Vector3 to) => value.VectorTo(to).normalized;

    public static float DistanceTo(this Vector3 value, Vector3 to) => value.VectorTo(to).magnitude;

    public static float SqrDistanceTo(this Vector3 value, Vector3 to) => value.VectorTo(to).sqrMagnitude;

    public static Vector3 Horizontal(this Vector3 value) => new Vector3(value.x, 0f, value.z);

    public static Vector3 HorizontalDirection(this Vector3 value) => value.Horizontal().normalized;

    public static Vector3 HorizontalVectorTo(this Vector3 value, Vector3 to) => value.VectorTo(to).Horizontal();

    public static Vector3 HorizontalDirectionTo(this Vector3 value, Vector3 to) => value.DirectionTo(to).Horizontal().normalized;

    public static float HorizontalDistanceTo(this Vector3 value, Vector3 to) => value.HorizontalVectorTo(to).magnitude;

    public static float HorizontalSqrDistanceTo(this Vector3 value, Vector3 to) => value.HorizontalVectorTo(to).sqrMagnitude;
}
