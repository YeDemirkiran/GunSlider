using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class OABoundingBox : MonoBehaviour
{
    [SerializeField] private BoundsCalculator boundsCalculator;

    public Vector3 center {  get; private set; }
    public Vector3[,] boundCorners { get; private set; }

    private Vector3 previousPosition, previousRotation;

    // Unity's Bounds calculation does not track the transform change in the same frame
    // So, we set the rotation to zero, wait for one frame and then do the rest

    IEnumerator Start()
    {
        Quaternion initialRotation = transform.rotation;

        transform.rotation = Quaternion.identity;
        previousRotation = transform.eulerAngles;

        yield return null;

        Bounds bounds = boundsCalculator.CalculateBounds();

        center = bounds.center;

        Vector3 topFrontRight = bounds.GetPoint(1f, 1f, 1f);
        Vector3 topFrontLeft = bounds.GetPoint(-1f, 1f, 1f);
        Vector3 topBackRight = bounds.GetPoint(1f, 1f, -1f);
        Vector3 topBackLeft = bounds.GetPoint(-1f, 1f, -1f);

        Vector3 bottomFrontRight = bounds.GetPoint(1f, -1f, 1f);
        Vector3 bottomFrontLeft = bounds.GetPoint(-1f, -1f, 1f);
        Vector3 bottomBackRight = bounds.GetPoint(1f, -1f, -1f);
        Vector3 bottomBackLeft = bounds.GetPoint(-1f, -1f, -1f);

        boundCorners = new Vector3[2,4] { { topFrontRight, topFrontLeft, topBackRight, topBackLeft }, // Top face corners
                                    { bottomFrontRight, bottomFrontLeft, bottomBackRight, bottomBackLeft }}; // Bottom face corners

        transform.eulerAngles = initialRotation.eulerAngles;
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (boundCorners != null)
        {
            Move();
            Rotate(0f);
        }        
    }

    void Rotate(float differenceThreshold)
    {
        differenceThreshold *= differenceThreshold;

        Vector3 rotationDifference = transform.eulerAngles - previousRotation;

        if (rotationDifference.sqrMagnitude > differenceThreshold)
        {
            // Loop through each corner and apply the rotation difference
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    boundCorners[i, j] = RotatePointAroundPivot(boundCorners[i, j], transform.position, rotationDifference);

                    //boundCorners[i, j] = Quaternion.AngleAxis(rotationDifference.x, transform.forward) * boundCorners[i, j];
                    //boundCorners[i, j] = Quaternion.AngleAxis(rotationDifference.y, transform.up) * boundCorners[i, j];
                    //boundCorners[i, j] = Quaternion.AngleAxis(rotationDifference.z, transform.right) * boundCorners[i, j];
                }
            }

            // Update previousRotation to the current rotation
            previousRotation = transform.eulerAngles;
        }      
    }

    void Move()
    {
        Vector3 positionDifferece = transform.position - previousPosition;

        // Loop through each corner and apply the rotation difference
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                boundCorners[i, j] += positionDifferece;
            }
        }

        // Update previousRotation to the current rotation
        previousPosition = transform.position;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 direction = point - pivot;

        direction = Quaternion.Euler(angles) * direction;

        point = direction + pivot;

        return point;
    }

    private void OnDrawGizmosSelected()
    {
        if (boundCorners != null)
        {
            // VERTICAL EDGES
            Gizmos.DrawLine(boundCorners[0, 0], boundCorners[1, 0]); // FRONT RIGHT
            Gizmos.DrawLine(boundCorners[0, 1], boundCorners[1, 1]); // FRONT LEFT
            Gizmos.DrawLine(boundCorners[0, 2], boundCorners[1, 2]); // BACK RIGHT
            Gizmos.DrawLine(boundCorners[0, 3], boundCorners[1, 3]); // BACK LEFT

            // HORIZONTAL TOP LINES
            Gizmos.DrawLine(boundCorners[0, 0], boundCorners[0, 1]); // Top front right to top front left
            Gizmos.DrawLine(boundCorners[0, 0], boundCorners[0, 2]); // **  **    **    to top back right

            Gizmos.DrawLine(boundCorners[0, 3], boundCorners[0, 1]); // Top back left to top front left
            Gizmos.DrawLine(boundCorners[0, 3], boundCorners[0, 2]); // **  **    **    to top back right

            // HORIZONTAL BOTTOM LINES
            Gizmos.DrawLine(boundCorners[1, 0], boundCorners[1, 1]); // Bottom front right to bottom front left
            Gizmos.DrawLine(boundCorners[1, 0], boundCorners[1, 2]); // **  **    **    to bottom back right

            Gizmos.DrawLine(boundCorners[1, 3], boundCorners[1, 1]); // Bottom back left to bottom front left
            Gizmos.DrawLine(boundCorners[1, 3], boundCorners[1, 2]); // **  **    **    to bottom back right
        }    
    }
}