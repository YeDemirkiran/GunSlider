using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset, lookAtOffset;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private bool lookAtTarget = true;
    //[SerializeField] private float delayTime, transitionTime;

    //private float timer = 0f, transitionTimer = 0f;

    //private Vector3 initialPosition, targetPosition;
    //private bool setTarget = false;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 smoothPosition = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
        transform.position = smoothPosition;

        if (lookAtTarget) transform.LookAt(target.position + lookAtOffset);
    }
}