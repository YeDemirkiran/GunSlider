using UnityEngine;

[RequireComponent(typeof(LaserColor))]
public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform laserLight;

    [SerializeField] private float laserMaxDistance = 500f;
    [SerializeField] private LayerMask layerMask;
    private RaycastHit laserHit;


    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out laserHit, laserMaxDistance, layerMask))
        {
            lineRenderer.SetPosition(1, Vector3.forward * laserHit.distance);

            if (!laserLight.gameObject.activeInHierarchy)
            {
                laserLight.gameObject.SetActive(true);
            }

            laserLight.position = laserHit.point;
        } else
        {
            lineRenderer.SetPosition(1, Vector3.forward * laserMaxDistance);

            if (laserLight.gameObject.activeInHierarchy)
            {
                laserLight.gameObject.SetActive(false);
            }
        }
        //lineRenderer.SetPosition(0, transform.position);
    }
}