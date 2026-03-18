using UnityEngine;

public class Functions : MonoBehaviour
{
    public static Vector2 MidPoint(Vector2 start, Vector2 end)
    {
        return Vector2.Lerp(start, end, 0.5f);
    }

    public static Vector2 Bezier(Vector2 start, Vector2 end, Vector2 mid, float t)
    {
        return Vector2.Lerp(Vector2.Lerp(start, mid, t), Vector2.Lerp(mid, end, t), t);
    }
}
