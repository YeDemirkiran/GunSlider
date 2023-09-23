using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private Transform parentOnReset;
    [SerializeField] private Vector3 localPositionOnReset;
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

            hasFired = false;
            currentHit = 0;
            rb.velocity = Vector3.zero;

            gameObject.SetActive(false);
        }
    }

    public void Shoot(Vector3 direction, bool relative)
    {
        hasFired = true;

        if (relative)
        {
            rb.AddRelativeForce(direction);
        }
        else
        {
            rb.AddForce(direction);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentHit++;
        Debug.Log("Hit! Current hit left: " + (maxHit - currentHit));
    }
}