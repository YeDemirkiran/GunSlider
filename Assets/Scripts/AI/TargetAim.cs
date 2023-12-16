using System.Collections;
using UnityEngine;

public class TargetAim : MonoBehaviour
{
    public Transform parent;
    public Hitpoint target {  get; set; }

    public bool aimingDone { get; private set; } =  true;

    Coroutine aimRoutine;

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