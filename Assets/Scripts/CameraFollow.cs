using TMPro;
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
        // This script was planned to be runned only if the game is running.
        // I changed my mind and too lazy to move the function out of the block.
        // I realized writing these comments took more time than it would take to take these 
        // out of the block, but writing comments are more fun.

        //if (!GameManager.isPaused)
        if (true)
        {
            //offsetMultiplier += Time.deltaTime * Input.GetAxis("Shift Camera") * offsetTransitionSpeed;
            //offsetMultiplier = Mathf.Clamp(offsetMultiplier, -1f, 1f);

            //currentOffset.z = offset.z * offsetMultiplier;

            //Vector3 m_Offset = Vector3.Scale(offset, target.forward);

            Vector3 m_Offset = target.forward * offset.z;
            m_Offset += target.right * offset.x;
            m_Offset.y = offset.y;

            Vector3 smoothPosition = Vector3.Lerp(transform.position, target.position + m_Offset, smoothSpeed * Time.unscaledDeltaTime);
            transform.position = smoothPosition;

            //if (lookAtTarget) transform.LookAt(target.position + (Vector3.Scale(target.forward, lookAtOffset)));
            if (lookAtTarget) transform.LookAt(target.position);

        }
    }

    private void OnDrawGizmosSelected()
    {
        //Vector3 m_Offset = Vector3.Scale(offset, target.forward);
        Vector3 m_Offset = target.forward * 5f;
        //m_Offset.y = offset.y;

        Gizmos.DrawWireCube(target.position + Vector3.Scale(offset, m_Offset), Vector3.one);
    }
}