using System.Collections;
using UnityEngine;

public class AimTarget : MonoBehaviour
{
    public Transform parent;
    [HideInInspector] public Hitpoint target;

    [HideInInspector] public bool aimingDone = true;

    private Coroutine aimRoutine;

    // Update is called once per frame
    void Update()
    {
        //if (delay > 0f)
        //{
        //    if (delayTimer > delay)
        //    {
        //        if (aimRoutine != null) StopCoroutine(aimRoutine);

        //        aimRoutine = StartCoroutine(AimCoroutine());

        //        delayTimer = 0f;
        //    }
        //    else
        //    {
        //        if (aimRoutine == null) delayTimer += Time.deltaTime;
        //    }
        //}

        //else
        //{
        //    direction = (target.transform.position - parent.position).normalized;
        //    transform.position = parent.position + direction;
        //}

        //transform.rotation = Quaternion.LookRotation(direction);
    }

    public void Aim(bool constant, float aimSpeed)
    {
        if (aimRoutine != null) StopCoroutine(aimRoutine);

        aimRoutine = StartCoroutine(AimCoroutine(constant, aimSpeed));
    }

    private IEnumerator AimCoroutine(bool constant, float aimSpeed)
    {
        aimingDone = false;

        float lerp = 0f;

        Vector3 direction = (target.transform.position - parent.position).normalized;

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = parent.position + direction;

        float distance = Vector3.Distance(initialPosition, targetPosition);

        while (lerp <= 1f)
        {
            if (constant) lerp += (aimSpeed / distance) * Time.deltaTime;
            else lerp += Time.deltaTime / aimSpeed;

            transform.position = Vector3.Slerp(initialPosition, targetPosition, lerp);

            yield return null;
        }

        aimRoutine = null;

        aimingDone = true;
    }
}