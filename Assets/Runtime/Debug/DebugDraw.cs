using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;

public class DebugDraw
{
    [BurstDiscard]
    public static void Circle(float3 center, float3 normal, float radius, Color color)
    {
        float3 v1;
        float3 v2;
        CalculatePlaneVectorsFromNormal(normal, out v1, out v2);
        CircleInternal(center, v1, v2, radius, color);
    }

    [BurstDiscard]
    public static void Line(float3 from, float3 to, Color color, float duration = 0)
    {
        Debug.DrawLine(from, to, color, duration);
    }

    [BurstDiscard]
    public static void Sphere(float3 center, float radius)
    {
        Sphere(center, radius, Color.white);
    }

    [BurstDiscard]
    public static void Sphere(float3 center, float radius, Color color, float duration = 0)
    {
        CircleInternal(center, math.right(), math.up(), radius, color, duration);
        CircleInternal(center, math.forward(), math.up(), radius, color, duration);
        CircleInternal(center, math.right(), math.forward(), radius, color, duration);
    }

    public static void Cross(float2 point, Color color, float duration = 0)
    {
        DebugDraw.Cross(new float3(point, 0), 0.5f, color, duration);
    }

    public static void Cross(float3 point, float duration = 0)
    {
        DebugDraw.Cross(point, 0.5f, Color.white, duration);
    }

    public static void Cross(float3 point, Color color, float duration = 0)
    {
        DebugDraw.Cross(point, 0.5f, color, duration);
    }

    public static void Cross(float3 point, float width, Color color, float duration = 0)
    {
        width *= 0.5f;
        Debug.DrawLine(point + new float3(width, 0f, width), point + new float3(-width, 0f, -width), color, duration);
        Debug.DrawLine(point + new float3(-width, 0f, width), point + new float3(width, 0f, -width), color, duration);
    }

    [BurstDiscard]
    public static void Capsule(float3 center, float3 dir, float radius, float height, Color color, float duration = 0)
    {
        var cylinderHeight = height - radius * 2;
        var v = Vector3.Angle(dir, math.up()) > 0.001 ? math.cross(dir, math.up()) : math.cross(dir, math.left());

        CircleInternal(center + dir * cylinderHeight * 0.5f, dir, v, radius, color, duration);
        CircleInternal(center - dir * cylinderHeight * 0.5f, dir, v, radius, color, duration);

        Cylinder(center, dir, radius, cylinderHeight * 0.5f, color, duration);
    }

    [BurstDiscard]
    public static void Cylinder(float3 center, float3 normal, float radius, float halfHeight, Color color, float duration = 0)
    {
        float3 v1;
        float3 v2;
        CalculatePlaneVectorsFromNormal(normal, out v1, out v2);

        var offset = normal * halfHeight;
        CircleInternal(center - offset, v1, v2, radius, color, duration);
        CircleInternal(center + offset, v1, v2, radius, color, duration);

        const int segments = 20;
        float arc = Mathf.PI * 2.0f / segments;
        for (var i = 0; i < segments; i++)
        {
            float3 p = center + v1 * math.cos(arc * i) * radius + v2 * math.sin(arc * i) * radius;
            Debug.DrawLine(p - offset, p + offset, color, duration);
        }
    }

    [BurstDiscard]
    private static void CircleInternal(float3 center, float3 v1, float3 v2, float radius, Color color, float duration = 0)
    {
        const int segments = 20;
        float arc = Mathf.PI * 2.0f / segments;
        float3 p1 = center + v1 * radius;
        for (var i = 1; i <= segments; i++)
        {
            float3 p2 = center + v1 * math.cos(arc * i) * radius + v2 * math.sin(arc * i) * radius;
            Debug.DrawLine(p1, p2, color, duration);
            p1 = p2;
        }
    }

    private static void CalculatePlaneVectorsFromNormal(float3 normal, out float3 v1, out float3 v2)
    {
        if (math.abs(math.dot(normal, math.up())) < 0.99)
        {
            v1 = math.normalize(math.cross(math.up(), normal));
            v2 = math.cross(normal, v1);
        }
        else
        {
            v1 = math.normalize(math.cross(math.left(), normal));
            v2 = math.cross(normal, v1);
        }
    }

    public static void Arrow(float3 pos, float angle, Color color, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
    {
        var angleRot = Quaternion.AngleAxis(angle, math.up());
        var dir = angleRot * math.forward();
        Arrow(pos, dir, color, length, tipSize, width);
    }

    public static void Arrow(float3 pos, float2 direction, Color color, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
    {
        var dir = new float3(direction.x, 0f, direction.y);
        Arrow(pos, dir, color, length, tipSize, width);
    }

    public static void Arrow(float3 pos, float3 direction, Color color, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
    {
        direction = math.normalize(direction);

        var sideLen = length - length * tipSize;
        var widthOffset = math.cross(direction, math.up()) * width;

        var baseLeft = pos + widthOffset * 0.3f;
        var baseRight = pos - widthOffset * 0.3f;
        var tip = pos + direction * length;
        var upCornerInRight = pos - widthOffset * 0.3f + direction * sideLen;
        var upCornerInLeft = pos + widthOffset * 0.3f + direction * sideLen;
        var upCornerOutRight = pos - widthOffset * 0.5f + direction * sideLen;
        var upCornerOutLeft = pos + widthOffset * 0.5f + direction * sideLen;

        Debug.DrawLine(baseLeft, baseRight, color);
        Debug.DrawLine(baseRight, upCornerInRight, color);
        Debug.DrawLine(upCornerInRight, upCornerOutRight, color);
        Debug.DrawLine(upCornerOutRight, tip, color);
        Debug.DrawLine(tip, upCornerOutLeft, color);
        Debug.DrawLine(upCornerOutLeft, upCornerInLeft, color);
        Debug.DrawLine(upCornerInLeft, baseLeft, color);
    }
}
