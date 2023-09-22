using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    //[SerializeField] private float delayTime, transitionTime;

    //private float timer = 0f, transitionTimer = 0f;

    //private Vector3 initialPosition, targetPosition;
    //private bool setTarget = false;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position + offset;

        //if (timer < delayTime) 
        //{
        //    timer += Time.deltaTime;
        //}
        //else
        //{
        //    if (!setTarget)
        //    {
        //        setTarget = true;

        //        transitionTimer = 0f;

        //        initialPosition = transform.position;
        //        targetPosition = target.position;
        //        targetPosition.z = initialPosition.z;
        //    }

        //    if (transform.position != targetPosition)
        //    {
        //        transitionTimer += Time.deltaTime / transitionTime;
        //        transform.position = Vector3.Slerp(initialPosition, targetPosition, transitionTimer);
        //    }
        //    else
        //    {
        //        timer = transitionTimer = 0f;
        //        setTarget = false;
        //    }

        //    transform.LookAt(target);
        //}
    }
}