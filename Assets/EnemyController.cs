using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    public Transform player { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        player = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Debug.Log(true);
        }
        else
        {
            Debug.Log(false);
        }
    }
}