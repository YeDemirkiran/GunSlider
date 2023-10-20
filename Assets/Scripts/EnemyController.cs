using Unity.VisualScripting;
using UnityEngine;
using static EnemyStates;

public class EnemyStates
{
    public enum EnemyType { Aggressive = 0, Defensive = 1 }
    public enum EnemyState { Idle = 0, Chase = 1, Attack = 2}
    public enum AttackType { Melee = 0, Ranged = 1 }
}

public class EnemyController : MonoBehaviour
{
    public Transform player { get; set; }

    [SerializeField] EnemyType enemyType;
    [SerializeField] EnemyState enemyState;
    [SerializeField] AttackType attackType;

    [SerializeField] private BotMovement bot;

    [Header("AGGRESSIVE")]

    [Header("Melee")]
    [SerializeField] private float meleeAttackRadius;

    [Header("Ranged")]
    [SerializeField] private GunController gunController;
    [SerializeField] float rangedAttackRadius;
    [Tooltip("Min-Max time")][SerializeField] private Vector2 actionTimeMinMax, aimIntervalMinMax;
    private float currentActionTime, currentAimTime;
    [SerializeField] float aimErrorMargin = 0.25f;
    private float actionTimer = 0f, aimTimer = 0f;
    
    [SerializeField] bool changePositionOnHit;

    private bool inRange = false;

    [Header("DEFENSIVE")]
    [Range(0f, 180f)][SerializeField] private float maxAngleDifference;
    [SerializeField] private float escapingMinDistance;

    // Start is called before the first frame update
    void Awake()
    {
        player = null;
    }

    // Update is called once per frame
    void Update()
    {
        // PSEUDO CODE
        // If Player is visible
        //   TYPE: AGGRESSIVE
        //   {
        //     ATTACK TYPE: Melee
        //     {
        //       If near the player: Melee Attack
        //       If not: Approach 
        //     }
        //
        //     ATTACK TYPE: Ranged
        //     { 
        //       ?If within the firing radius: 
        //         ?If player is aiming at you:
        //           ?If you don't have a cover: Find a cover
        //           ?If not: Occasionally leave the cover, shoot and go back
        //          
        //         ?If not:
        //           -If you're covered: Leave the cover
        //           -Approach the player
        //           -Aim & shoot
        //           -Check again if the player is aiming at you
        //
        //       ?If not: Approach
        //     }
        //     
        //   TYPE: DEFENSIVE
        //   {
        //     If there are any ways to escape: Run in the opposite side of the player 
        //     If not: Kneel and beg for mercy
        //   }

        // NEW 
        if (player != null)
        {
            // We're on the same Y axis so strip that away
            Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);
            Vector2 transformForwardVec2 = transform.forward.ToVector2(Axis.y);
            Vector2 transformSideVec2 = transform.right.ToVector2(Axis.y);

            Vector2 playerPosVec2 = player.position.ToVector2(Axis.y);

            float distance = Vector2.Distance(transformPosVec2, playerPosVec2);
            Vector2 direction = (playerPosVec2 - transformPosVec2).normalized;

            float verticalDot = Vector2.Dot(direction, transformForwardVec2);
            float verticalAngle = Mathf.Acos(verticalDot / (direction.magnitude * transformForwardVec2.magnitude));

            float horizontalDot = Vector2.Dot(direction, transformSideVec2);

            // Turn to degrees
            verticalAngle *= Mathf.Rad2Deg;

            //Debug.Log("DOT: " + horizontalDot);
            //return;

            if (enemyType == EnemyType.Aggressive)
            {
                if (attackType == AttackType.Melee)
                {
                    if (distance > meleeAttackRadius)
                    {
                        // If there is an attack still happening, cut it
                        // Play a short version of the attack to give an immersion of the AI changing its mind mid-attack

                        // Approach the player
                    }

                    else
                    {
                        // Melee attack
                        // Properties: It must attack with irregular intervals, and choose from several melee attack options
                    }
                }

                else if (attackType == AttackType.Ranged)
                {
                    if (distance > rangedAttackRadius)
                    {
                        // If there is an attack still happening, cut it
                        // Play a short version of the attack to give an immersion of the AI changing its mind mid-attack

                        // Approach the player
                    }

                    else
                    {
                        if (true) //if the player is aiming at you A.K.A compare directions
                        {
                            if (true) // if you don't have a cover
                            {
                                //FindCover();
                            }
                            else
                            {
                                // Occasionally leave the cover and shoot and go back
                            }
                        }

                        else
                        {
                            if (true) // If you have a cover
                            {
                                // Leave the cover
                            }
                            else
                            {
                                // Aprroach the player
                                // Occasionally aim and shoot at the player
                            }
                        }
                    }
                }
            }

            else if (enemyType == EnemyType.Defensive)
            {
                if (distance > escapingMinDistance)
                {
                    bot.Pray(false);

                    if (verticalAngle < 180f - (maxAngleDifference / 2f))
                    {
                        transform.Rotate(Vector3.up * Time.deltaTime * 250f * -Mathf.Sign(horizontalDot));
                    }

                    bot.Move(1f, 0f);
                }
                else
                {
                    if (verticalAngle > 0f + (maxAngleDifference / 2f))
                    {
                        bot.Pray(false);

                        transform.Rotate(Vector3.up * Time.deltaTime * 250f * Mathf.Sign(horizontalDot));
                        bot.Move(0f, 0f);
                    }
                    else
                    {
                        bot.Pray(true);
                    }
                }

                // CalculateSurroundings(); calculate if there is an exit route on the opposite side of the player
                // But since I don't know how to do this, we will followed a simpler solution above

                //if (true) // If there are 
                //{
                //    // Escape bitch
                //}
                //else
                //{
                //    // Plea();
                //}
            }
        }
        
        //// OLD
        //if (player != null)
        //{
        //    Debug.Log($"Bot named '{gameObject}' is within the player");

        //    Vector2 transformVec2 = new Vector2(transform.position.x, transform.position.z);
        //    Vector2 playerVec2 = new Vector2(player.position.x, player.position.z);

        //    if (enemyState == EnemyState.Attack)
        //    {
        //        if (attackType == AttackType.Ranged)
        //        {
        //            float distance = Vector2.Distance(transformVec2, playerVec2);
        //            Vector2 distanceVec = new Vector2(Mathf.Abs(transformVec2.x - playerVec2.x),
        //                Mathf.Abs(transformVec2.y - playerVec2.y));

        //            Vector3 direction = (player.position - transform.position).normalized;

        //            transform.eulerAngles = Vector3.Scale(Quaternion.LookRotation(direction).eulerAngles, Vector3.up);

        //            if (distance > rangedAttackRadius)
        //            {
        //                botMovement.Move(1f, 0f);
        //                inRange = false;
        //            }
        //            else
        //            {
        //                if (!inRange)
        //                {
        //                    inRange = true;

        //                    botMovement.Move(0f, 0f);

        //                    currentAimTime = Random.Range(aimIntervalMinMax.x, aimIntervalMinMax.y);
        //                    currentActionTime = Random.Range(actionTimeMinMax.x, actionTimeMinMax.y);
        //                }


        //                if (aimTimer > currentAimTime)
        //                {
        //                    gunController.LookAtTarget(player.position, aimErrorMargin);

        //                    aimTimer = 0f;
        //                    currentAimTime = Random.Range(aimIntervalMinMax.x, aimIntervalMinMax.y);
        //                }
        //                else
        //                {
        //                    aimTimer += Time.deltaTime;
        //                }

        //                if (actionTimer > currentActionTime)
        //                {
        //                    if (gunController.currentAmmo <= 0)
        //                    {
        //                        gunController.Reload();
        //                    }
        //                    else
        //                    {
        //                        gunController.Shoot();
        //                    }

        //                    actionTimer = 0f;
        //                    currentActionTime = Random.Range(actionTimeMinMax.x, actionTimeMinMax.y);
        //                }
        //                else
        //                {
        //                    actionTimer += Time.deltaTime;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log($"Bot named '{gameObject}' is not within the player. Patrolling...");
        //        }
        //    }
        //}
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, rangedAttackRadius);
    }
}

public enum Axis { x, y, z }

// MOVE THESE TO A PROPER PLACE LATER

public static class Vector2Extensions
{
    public static Vector3 ToVector3(this Vector2 vector, Axis addedAxis, float addedAxisValue)
    {
        Vector3 vec3;

        switch (addedAxis)
        {
            case Axis.x:
                vec3.x = addedAxisValue;
                vec3.y = vector.x;
                vec3.z = vector.y;

                break;

            case Axis.y:
                vec3.x = vector.x;
                vec3.y = addedAxisValue;
                vec3.z = vector.y;

                break;

            case Axis.z:
                vec3.x = vector.x;
                vec3.y = vector.y;
                vec3.z = addedAxisValue;

                break;

            default:
                vec3 = Vector3.zero;
                break;
        }

        return vec3;
    }
}

public static class Vector3Extensions
{
    public static Vector2 ToVector2(this Vector3 vector, Axis removedAxis)
    {
        Vector2 vec2;

        switch (removedAxis)
        {
            case Axis.x:
                vec2.x = vector.y;
                vec2.y = vector.z;

                break;

            case Axis.y:
                vec2.x = vector.x;
                vec2.y = vector.z;

                break;

            case Axis.z:
                vec2.x = vector.x;
                vec2.y = vector.y;

                break;

            default:
                vec2 = Vector2.zero; 
                break;
        }

        return vec2;
    }
}