using Unity.VisualScripting;
using UnityEngine;
using static EnemyStates;

public class EnemyStates
{
    public enum EnemyType { Passive = 0, Aggressve = 1, Defensive = 2}
    public enum EnemyState { Idle = 0, Chase = 1, Attack = 2}
    public enum AttackType { Melee = 0, Ranged = 1 }
}

public class EnemyController : MonoBehaviour
{
    public Transform player { get; set; }

    [SerializeField] EnemyType enemyType;
    [SerializeField] EnemyState enemyState;
    [SerializeField] AttackType attackType;

    [SerializeField] private BotMovement botMovement;

    [Header("Ranged Attack")]
    [SerializeField] float rangedAttackMaxDistance;
    
    [SerializeField] bool changePositionOnHit;

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
            Debug.Log($"Bot named '{gameObject}' is within the player");

            Vector2 transformVec2 = new Vector2(transform.position.x, transform.position.z);
            Vector2 playerVec2 = new Vector2(player.position.x, player.position.z);

            float distance = Vector2.Distance(transformVec2, playerVec2);
            Vector2 distanceVec = new Vector2(Mathf.Abs(transformVec2.x - playerVec2.x), 
                Mathf.Abs(transformVec2.y - playerVec2.y));

            Vector3 direction = (player.position - transform.position).normalized;
            transform.eulerAngles = Vector3.Scale(Quaternion.LookRotation(direction).eulerAngles, Vector3.up);

            if (distance > rangedAttackMaxDistance)
            {
                botMovement.Move(1f, 0f);
            }
            else
            {
                botMovement.Move(0f, 0f);
                Debug.Log("Ranged Attack within the range. Shooting at the player!");
            }
        }
        else
        {
            Debug.Log($"Bot named '{gameObject}' is not within the player. Patrolling...");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, rangedAttackMaxDistance);
    }
}