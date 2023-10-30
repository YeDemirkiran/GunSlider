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
                    Vector2 position = CalculateClosestPoint(boundsCalculator.CalculateBounds());
                    Debug.Log("Obstacle detected. Name: " + hit.transform.name + ", Position: " + position);

                    Obstacle obstacle = new Obstacle(hit.transform.gameObject, boundsCalculator);
                    controller.currentObstacle = obstacle;
                }
                else
                {
                    Debug.Log("Object detected! Name: " + hit.transform.name);
                }
            }
        }
        else
        {
            Debug.Log("Nothing detected.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * detectorDistance));
    }

    public Vector2 CalculateClosestPoint(Bounds bounds)
    {
        Vector3 position;

        Vector3 pointCoordinates = Vector3.zero;

        Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);

        // Z-AXIS
        Vector2 boundsFrontVec2 = bounds.GetFront().ToVector2(Axis.y);
        Vector2 boundsBackVec2 = bounds.GetBack().ToVector2(Axis.y);

        // X-AXIS
        Vector2 boundsRightVec2 = bounds.GetRight().ToVector2(Axis.y);
        Vector2 boundsLeftVec2 = bounds.GetLeft().ToVector2(Axis.y);

        // Distances
        float frontDistance = Vector2.Distance(transformPosVec2, boundsFrontVec2);
        float backDistance = Vector2.Distance(transformPosVec2, boundsBackVec2);
        float rightDistance = Vector2.Distance(transformPosVec2, boundsRightVec2);
        float leftDistance = Vector2.Distance(transformPosVec2, boundsLeftVec2);

        // Front or back?
        if (frontDistance < backDistance) pointCoordinates.z = 1f;
        else pointCoordinates.z = -1f;

        // Right or left?
        if (rightDistance < leftDistance) pointCoordinates.x = 1f;
        else pointCoordinates.x = -1f;

        position = bounds.GetPoint(pointCoordinates);

        return position;
    }
}