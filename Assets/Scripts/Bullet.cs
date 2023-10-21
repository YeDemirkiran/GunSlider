using System.Collections;
using System.Collections.Generic;
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

        Material mat = null;
        Mesh mesh = defaultParticleMesh;

        // IF THE COLLIDED OBJECT HAS A DATA COLLISION COMPONENT, THAT MEANS WE ARE GUARANTEED TO HAVE A MESH IN RETURN

        if (collider.TryGetComponent(out CollisionData data))
        {
            mat = data.meshHolder.GetComponent<Renderer>().material;

            if (!data.meshHolder.TryGetComponent(out SkinnedMeshRenderer skinnedRenderer))
            {
                mesh = data.meshHolder.GetComponent<MeshFilter>().mesh;
            }
        }
        // BUT IF IT DOESN'T HAVE IT AND THE COLLIDER AND THE MESH RENDERER AREN'T ON THE SAME GAMEOBJECT
        // THEN WE HAVE TO GUARANTEE IT OURSELVES
        else
        {
            if (collider.TryGetComponent(out Renderer renderer))
            {
                mat = renderer.material;
            }

            if (collider.TryGetComponent(out MeshFilter meshFilter))
            {
                mesh = meshFilter.mesh;
            }
        }

        GameObject particle = Instantiate(particleOnHit, collision.GetContact(0).point, Quaternion.LookRotation(-transform.forward));
        particle.GetComponent<ParticleSystemRenderer>().material = mat;
        particle.GetComponent<ParticleSystemRenderer>().mesh = mesh;
        //Destroy(particle, particleDestroyTime);
        //Debug.Log("Hit! Current hit left: " + (maxHit - currentHit));
    }
}