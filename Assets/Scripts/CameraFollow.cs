using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset, lookAtOffset;
    [SerializeField] private float offsetTransitionSpeed = 5f;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private bool lookAtTarget = true;

    private Vector3 currentOffset;
    private float offsetMultiplier = 1;

    private void Start()
    {
        currentOffset = offset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        offsetMultiplier += Time.deltaTime * Input.GetAxis("Shift Camera") * offsetTransitionSpeed;
        offsetMultiplier = Mathf.Clamp(offsetMultiplier, -1f, 1f);

        currentOffset.z = offset.z * offsetMultiplier;

        Vector3 smoothPosition = Vector3.Lerp(transform.position, target.position + currentOffset, smoothSpeed * Time.deltaTime);
        transform.position = smoothPosition;


        if (lookAtTarget) transform.LookAt(target.position + (lookAtOffset));
    }
}