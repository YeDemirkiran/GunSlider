using UnityEngine;

public class BotActivator : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask enemyLayers;

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayers & 1 << other.gameObject.layer) != 0)
        {
            if (other.TryGetComponent(out EnemyController enemy))
            {
                enemy.player = player;
            }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemyLayers & 1 << other.gameObject.layer) != 0)
        {
            if (other.TryGetComponent(out EnemyController enemy))
            {
                enemy.player = null;
            }
        }
    }
}
