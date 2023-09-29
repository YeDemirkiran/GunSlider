using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignParent : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 localPosition, localEuler, localScale;

    private void Awake()
    {
        transform.SetParent(parent);
        transform.localPosition = localPosition;
        transform.localEulerAngles = localEuler;
        transform.localScale = localScale;
    }
}
