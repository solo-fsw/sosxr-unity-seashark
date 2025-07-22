using UnityEngine;


public static class SOSXRDefaultValues
{
    public static int Int => 42;
    public static float Float => 3.14f;
    public static bool Bool => true;
    public static string String => "Hello World!";
    public static double Double => 2.718;
    public static byte Byte => 64;
    public static char Char => 'A';

    public static Vector2 Vector2 => new(1f, 1f);
    public static Vector3 Vector3 => new(1f, 1f, 1f);
    public static Vector4 Vector4 => new(1f, 1f, 1f, 1f);
    public static Color Color => Color.cyan;
    public static Quaternion Quaternion => Quaternion.identity;
    public static Transform Transform => null;
    public static GameObject GameObject => null;
    public static AnimationCurve Curve => AnimationCurve.EaseInOut(0, 0, 1, 1);
    public static Bounds Bounds => new(Vector3.zero, Vector3.one);
    public static Rect Rect => new(0, 0, 100, 100);
    public static LayerMask LayerMask => 0;


    public static T Get<T>()
    {
        var type = typeof(T);

        return type switch
               {
                   _ when type == typeof(int) => (T) (object) Int,
                   _ when type == typeof(float) => (T) (object) Float,
                   _ when type == typeof(bool) => (T) (object) Bool,
                   _ when type == typeof(string) => (T) (object) String,
                   _ when type == typeof(double) => (T) (object) Double,
                   _ when type == typeof(byte) => (T) (object) Byte,
                   _ when type == typeof(char) => (T) (object) Char,
                   _ when type == typeof(Vector2) => (T) (object) Vector2,
                   _ when type == typeof(Vector3) => (T) (object) Vector3,
                   _ when type == typeof(Vector4) => (T) (object) Vector4,
                   _ when type == typeof(Color) => (T) (object) Color,
                   _ when type == typeof(Quaternion) => (T) (object) Quaternion,
                   _ when type == typeof(Transform) => (T) (object) Transform,
                   _ when type == typeof(GameObject) => (T) (object) GameObject,
                   _ when type == typeof(AnimationCurve) => (T) (object) Curve,
                   _ when type == typeof(Bounds) => (T) (object) Bounds,
                   _ when type == typeof(Rect) => (T) (object) Rect,
                   _ when type == typeof(LayerMask) => (T) (object) LayerMask,
                   _ => default
               };
    }
}