using UnityEngine;

public enum Axis { x, y, z }

// MOVE THESE TO A PROPER PLACE LATER

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

    public static float DotAngle(Vector3 forward, Vector3 direction, bool inDegrees = true)
    {
        float dot = Vector3.Dot(forward, direction);

        float angle = Mathf.Acos(dot / (forward.magnitude * direction.magnitude));

        return inDegrees ? angle * Mathf.Rad2Deg : angle;
    }
}