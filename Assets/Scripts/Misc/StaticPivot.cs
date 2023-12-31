using UnityEngine;

public class StaticPivot : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 positionOffset;
    private Vector3 defaultRotation;

    private void OnEnable()
    {
        positionOffset = target.position - transform.position;
        defaultRotation = transform.eulerAngles;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position - positionOffset; 
        transform.eulerAngles = defaultRotation;
    }
}
