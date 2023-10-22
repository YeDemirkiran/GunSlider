using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    [Header("GENERAL")]
    [SerializeField] private Transform parentOnReset;
    [SerializeField] private Vector3 localPositionOnReset;
    [SerializeField] private Vector3 localRotationOnReset;
    [SerializeField] private float maxDistanceFromParent;

    [SerializeField] private int maxHit = 2;
    private int currentHit = 0;

    [SerializeField] private float shakeOnHit, shakeOnEnemyHit;

    [Header("COLLISION")]
    [SerializeField] private GameObject particleOnHit;
    [SerializeField] private Mesh defaultParticleMesh;
    [SerializeField] private float particleDestroyTime;

    [HideInInspector] public bool hasFired = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!GameManager.isPaused)
        {
            if (currentHit >= maxHit || Vector3.Distance(parentOnReset.position, transform.position) > maxDistanceFromParent)
            {
                transform.SetParent(parentOnReset);
                transform.localPosition = localPositionOnReset;
                transform.localEulerAngles = localRotationOnReset;

                hasFired = false;
                currentHit = 0;
                rb.velocity = Vector3.zero;

                gameObject.SetActive(false);
            }
        }  
    }

    public void Shoot(Vector3 direction)
    {
        hasFired = true;

        rb.AddForce(direction, ForceMode.VelocityChange);
    }

    public void Shoot(float force)
    {
        hasFired = true;

        rb.AddRelativeForce(Vector3.forward * force, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentHit++;

        GameObject collider = collision.gameObject;

        if (collider.TryGetComponent(out CollisionData data))
        {
            foreach (Debris debris in data.debrises)
            {
                int randomParticle = Random.Range(0, debris.debrisParticles.Length);
                GameObject particle = Instantiate(debris.debrisParticles[randomParticle], collision.GetContact(0).point, Quaternion.LookRotation(transform.forward));
            }
        }
    }
}