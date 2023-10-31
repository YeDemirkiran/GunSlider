using UnityEngine;

public class Obstacle
{
    public GameObject gameObject { get; private set; }
    public Transform transform { get { return gameObject.transform; } }

    public BoundsCalculator boundsCalculator { get; private set; }
    public Bounds bounds { get { return boundsCalculator.CalculateBounds(); } }

    public Obstacle(GameObject gameObject, BoundsCalculator boundsCalculator)
    {
        this.gameObject = gameObject;
        this.boundsCalculator = boundsCalculator;
    }

    // CURRENTLY CAN ONLY CALCULATE ON THE SAME HEIGHT, NO Y-AXIS
    // I DON'T NEED IT
    public Vector3 CalculateClosestPoint(Vector3 originPosition)
    {
        Vector3 pointCoordinates = Vector3.zero;

        // Z-AXIS
        Vector2 boundsFront = bounds.GetFront();
        Vector2 boundsBack = bounds.GetBack();

        // X-AXIS
        Vector2 boundsRight = bounds.GetRight();
        Vector2 boundsLeft = bounds.GetLeft();

        // Distances
        float frontDistance = Vector3.Distance(originPosition, boundsFront);
        float backDistance = Vector3.Distance(originPosition, boundsBack);
        float rightDistance = Vector3.Distance(originPosition, boundsRight);
        float leftDistance = Vector3.Distance(originPosition, boundsLeft);

        // Front or back?
        if (frontDistance < backDistance) pointCoordinates.z = 1f;
        else pointCoordinates.z = -1f;

        // Right or left?
        if (rightDistance < leftDistance) pointCoordinates.x = 1f;
        else pointCoordinates.x = -1f;

        return bounds.GetPoint(pointCoordinates);
    }

    public Vector3 CalculateClosestPoint(Vector3 originPosition, out Vector3 coordinates)
    {
        Vector2 originVec2 = originPosition.ToVector2(Axis.y);
        Vector3 pointCoordinates = Vector3.zero;

        // Z-AXIS
        Vector2 boundsFront = bounds.GetFront().ToVector2(Axis.y);
        Vector2 boundsBack = bounds.GetBack().ToVector2(Axis.y);

        // X-AXIS
        Vector2 boundsRight = bounds.GetRight().ToVector2(Axis.y);
        Vector2 boundsLeft = bounds.GetLeft().ToVector2(Axis.y);

        // Distances
        float frontDistance = Vector2.Distance(originVec2, boundsFront);
        float backDistance = Vector2.Distance(originVec2, boundsBack);
        float rightDistance = Vector2.Distance(originVec2, boundsRight);
        float leftDistance = Vector2.Distance(originVec2, boundsLeft);

        //Debug.Log("Front : " + boundsFront);
        //Debug.Log("back : " + boundsBack);
        //Debug.Log("right : " + boundsRight);
        //Debug.Log("left : " + boundsLeft);

        //Debug.Log("Front Distance: " + frontDistance);
        //Debug.Log("back Distance: " + backDistance);
        //Debug.Log("right Distance: " + rightDistance);
        //Debug.Log("left Distance: " + leftDistance);

        // Front or back?
        if (frontDistance < backDistance) { pointCoordinates.z = 1f;}
        else { pointCoordinates.z = -1f; }

        // Right or left?
        if (rightDistance < leftDistance) { pointCoordinates.x = 1f;}
        else { pointCoordinates.x = -1f; }

        coordinates = pointCoordinates;

        return bounds.GetPoint(pointCoordinates);
    }
}

public class ObstacleDetector : MonoBehaviour
{
    [SerializeField] private CivillianController controller;
    [SerializeField] private BoundsCalculator ownBoundsCalculator;
    [SerializeField] private float detectorDistance = 10f;

    [SerializeField] private LayerMask layerMask;

    Vector3 halfExtents;

    private void Start()
    {
        halfExtents = ownBoundsCalculator.CalculateBounds().extents;
    }

    // Update is called once per frame
    void Update()
    {     
        if (GameManager.isPaused) { return; }

        if (Physics.BoxCast(transform.position, halfExtents, transform.forward, out RaycastHit hit, transform.rotation, detectorDistance, layerMask))
        {
            if (hit.transform.root != transform.root)
            {
                if (hit.collider.TryGetComponent(out BoundsCalculator boundsCalculator))
                {
                    //Vector2 position = CalculateClosestPoint(boundsCalculator.CalculateBounds());
                    //Debug.Log("Obstacle detected. Name: " + hit.transform.name + ", Position: " + position);

                    Obstacle obstacle = new Obstacle(hit.transform.gameObject, boundsCalculator);
                    controller.currentObstacle = obstacle;
                }
                else if (hit.transform.root.TryGetComponent(out BoundsCalculator rootBoundsCalculator))
                {
                    Obstacle obstacle = new Obstacle(hit.transform.gameObject, rootBoundsCalculator);
                    controller.currentObstacle = obstacle;
                }
                else
                {
                    //Debug.Log("Object detected! Name: " + hit.transform.name);
                }
            }
        }
        else
        {
            //Debug.Log("Nothing detected.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * detectorDistance));
    }
}