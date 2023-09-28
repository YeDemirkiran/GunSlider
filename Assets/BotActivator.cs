using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotActivator : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyController enemy))
        {
            enemy.player = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out EnemyController enemy))
        {
            enemy.player = null;
        }
    }
}
