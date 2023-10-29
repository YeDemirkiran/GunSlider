using UnityEngine;


public class ObstacleDetection : MonoBehaviour
{
    [SerializeField] private BoundsCalculator ownBoundsCalculator;
    [SerializeField] private float detectorDistance = 10f;

    [SerializeField] private LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
       if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit,  detectorDistance, layerMask))
       {
            if (hit.transform.root != transform.root)
            {
                if (hit.collider.TryGetComponent(out BoundsCalculator boundsCalculator))
                {
                    //Debug.Log("Obstacle detected!");
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