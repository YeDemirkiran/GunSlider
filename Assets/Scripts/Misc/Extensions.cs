using System.Collections.Generic;
using UnityEngine;

public enum Axis { x, y, z }
public enum Axis2D { x, y }

// MOVE THESE TO A PROPER PLACE LATER

public static class ListExtensions
{
    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
public static class ArrayExtensions
{
    public static T GetRandom<T>(this T[] list)
    {
        return list[Random.Range(0, list.Length)];
    }
}

public static class Vector2Extensions
{
    public static Vector3 ToVector3(this Vector2 vector, Axis addedAxis, float addedAxisValue)
    {
        
        Vector3 vec3;

        switch (addedAxis)
        {
            case Axis.x:
                vec3.x = addedAxisValue;
                vec3.y = vector.x;
                vec3.z = vector.y;

                break;

            case Axis.y:
                vec3.x = vector.x;
                vec3.y = addedAxisValue;
                vec3.z = vector.y;

                break;

            case Axis.z:
                vec3.x = vector.x;
                vec3.y = vector.y;
                vec3.z = addedAxisValue;

                break;

            default:
                vec3 = Vector3.zero;
                break;
        }

        return vec3;
    }

    public static bool IsWithinRange(this Vector2 origin, Vector2 targetPoint, float maxDistance)
    {
        if (Vector2.Distance(origin, targetPoint) < maxDistance)
        {
            return true;
        }

        return false;
    }

    public static float DotAngle(Vector2 forward, Vector2 direction, bool inDegrees = true)
    {
        float dot = Vector2.Dot(forward, direction);

        float angle = Mathf.Acos(dot / (forward.magnitude * direction.magnitude));
        return inDegrees ? angle * Mathf.Rad2Deg : angle;
    }
}
public static class Vector3Extensions
{
    public static Vector2 ToVector2(this Vector3 vector, Axis removedAxis)
    {
        Vector2 vec2;

        switch (removedAxis)
        {
            case Axis.x:
                vec2.x = vector.y;
                vec2.y = vector.z;

                break;

            case Axis.y:
                vec2.x = vector.x;
                vec2.y = vector.z;

                break;

            case Axis.z:
                vec2.x = vector.x;
                vec2.y = vector.y;

                break;

            default:
                vec2 = Vector2.zero;
                break;
        }

        return vec2;
    }

    public static Vector3 Clamp(this Vector3 vector, Vector3 mins, Vector3 maxs)
    {
        Vector3 vec = vector;

        vec.x = Mathf.Clamp(vec.x, mins.x, maxs.x);
        vec.y = Mathf.Clamp(vec.y, mins.y, maxs.y);
        vec.z = Mathf.Clamp(vec.z, mins.y, maxs.z);

        return vec;
    }

    public static bool IsWithinRange(this Vector3 origin, Vector3 targetPoint, float maxDistance)
    {
        if (Vector3.Distance(origin, targetPoint) < maxDistance)
        {
            return true;
        }

        return false;
    }

    public static float DotAngle(Vector3 forward, Vector3 direction, bool inDegrees = true)
    {
        float dot = Vector3.Dot(forward, direction);

        float angle = Mathf.Acos(dot / (forward.magnitude * direction.magnitude));

        return inDegrees ? angle * Mathf.Rad2Deg : angle;
    }

    public static Vector3 Random(Vector3 min, Vector3 max)
    {
        Vector3 vector = new Vector3();

        vector.x = UnityEngine.Random.Range(min.x, max.x);
        vector.y = UnityEngine.Random.Range(min.y, max.y);
        vector.z = UnityEngine.Random.Range(min.z, max.z);

        return vector;
    } 
}

public static class BoundsExtensions
{
    public static Vector3 GetBottom(this Bounds bounds)
    {
        return bounds.center - (Vector3.up * bounds.extents.y);
    }
    public static Vector3 GetTop(this Bounds bounds)
    {
        return bounds.center + (Vector3.up * bounds.extents.y);
    }

    public static Vector3 GetRight(this Bounds bounds)
    {
        return bounds.center + (Vector3.right * bounds.extents.x);
    }
    public static Vector3 GetLeft(this Bounds bounds)
    {
        return bounds.center - (Vector3.right * bounds.extents.x);
    }

    public static Vector3 GetFront(this Bounds bounds)
    {
        //Debug.Log("FRONT FUNCTION");

        //Debug.Log("Center: " + bounds.center);
        //Debug.Log("Extents: " + bounds.extents.z);

        return bounds.center + (Vector3.forward * bounds.extents.z);
    }
    public static Vector3 GetBack(this Bounds bounds)
    {
        //Debug.Log("BACK FUNCTION");

        //Debug.Log("Center: " + bounds.center);
        //Debug.Log("Extents: " + bounds.extents.z);

        return bounds.center - (Vector3.forward * bounds.extents.z);
    }

    // CROSS DIRECTIONS
    public static Vector3 GetPoint(this Bounds bounds, Vector3 coordinates)
    {
        coordinates = coordinates.Clamp(new Vector3(-1f, -1f, -1f), new Vector3(1f, 1f, 1f));

        float right = bounds.center.x + (bounds.extents.x * coordinates.x);
        float up = bounds.center.y + (bounds.extents.y * coordinates.y);
        float forward = bounds.center.z + (bounds.extents.z * coordinates.z);

        return new Vector3(right, up, forward);
    }

    public static Vector3 GetPoint(this Bounds bounds, float x, float y, float z)
    {
        Vector3 coordinates = new Vector3(x, y, z);

        coordinates = coordinates.Clamp(new Vector3(-1f, -1f, -1f), new Vector3(1f, 1f, 1f));

        float right = bounds.center.x + (bounds.extents.x * coordinates.x);
        float up = bounds.center.y + (bounds.extents.y * coordinates.y);
        float forward = bounds.center.z + (bounds.extents.z * coordinates.z);

        return new Vector3(right, up, forward);
    }
}