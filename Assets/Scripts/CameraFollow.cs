using TMPro;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(-2.5f, 0.75f, -4f),
        lookAtOffset = new Vector3(0f, 0f, 10f);
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
    void FixedUpdate()
    {
        // This script was planned to be runned only if the game is running.
        // I changed my mind and too lazy to move the function out of the block.
        // I realized writing these comments took more time than it would take to take these 
        // out of the block, but writing comments are more fun.

        //if (!GameManager.isPaused)
        if (true)
        {
            // CONTROL ON THE X AXIS
            offsetMultiplier -= Input.GetAxis("Shift Camera") *  Time.deltaTime * offsetTransitionSpeed;
            offsetMultiplier = Mathf.Clamp(offsetMultiplier, -1f, 1f);

            currentOffset.x = offset.x * offsetMultiplier;

            //Vector3 m_Offset = Vector3.Scale(offset, target.forward);

            Vector3 directionalOffset = new Vector3();
            directionalOffset += target.forward * currentOffset.z;
            directionalOffset += target.right * currentOffset.x;
            directionalOffset.y = offset.y;

            Vector3 directionalLookOffset = target.position;
            directionalLookOffset += target.forward * lookAtOffset.z;
            directionalLookOffset += target.right * lookAtOffset.x;
            directionalLookOffset.y = lookAtOffset.y;

            Vector3 smoothPosition = Vector3.Lerp(transform.position, target.position + directionalOffset, smoothSpeed * Time.unscaledDeltaTime);
            transform.position = smoothPosition;

            //if (lookAtTarget) transform.LookAt(target.position + (Vector3.Scale(target.forward, lookAtOffset)));
            if (lookAtTarget) transform.LookAt(directionalLookOffset);
        }
    }
}