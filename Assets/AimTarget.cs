using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AimTarget : MonoBehaviour
{
    [SerializeField] private Transform target, parent;
    [SerializeField] private float aimSpeed, delay;
    private float delayTimer;

    Vector3 direction;

    private Coroutine aimRoutine;

    // Update is called once per frame
    void Update()
    {
        if (delay > 0f)
        {
            if (delayTimer > delay)
            {
                if (aimRoutine != null) StopCoroutine(aimRoutine);

                aimRoutine = StartCoroutine(Aim());

                delayTimer = 0f;
            }
            else
            {
                if (aimRoutine == null) delayTimer += Time.deltaTime;
            }
        }

        else
        {
            direction = (target.position - parent.position).normalized;
            transform.position = parent.position + direction;
        }

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private IEnumerator Aim()
    {
        float lerp = 0f;

        direction = (target.position - parent.position).normalized;

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = parent.position + direction;

        float distance = Vector3.Distance(initialPosition, targetPosition);

        while (lerp <= 1f)
        {
            lerp += Time.deltaTime * (aimSpeed / distance);
            transform.position = Vector3.Slerp(initialPosition, targetPosition, lerp);

            yield return null;
        }

        aimRoutine = null;
    }
}