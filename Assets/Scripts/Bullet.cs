using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private Transform parentOnReset;
    [SerializeField] private Vector3 localPositionOnReset;
    [SerializeField] private Vector3 localRotationOnReset;
    [SerializeField] private float maxDistanceFromParent;

    [SerializeField] private int maxHit = 2;
    private int currentHit = 0;

    [SerializeField] private float shakeOnHit, shakeOnEnemyHit;

    [HideInInspector] public bool hasFired = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
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
        Debug.Log("Hit! Current hit left: " + (maxHit - currentHit));
    }
}