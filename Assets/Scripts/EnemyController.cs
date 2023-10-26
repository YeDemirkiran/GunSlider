using UnityEngine;
using static EnemyStates;

public class EnemyStates
{
    public enum EnemyType { Aggressive = 0, Defensive = 1 }
    public enum EnemyState { Idle = 0, Chase = 1, Attack = 2}

    public enum AttackType { Melee = 0, Ranged = 1 }
    public enum AttackState { Aiming = 0, Attacking = 1 }
}

public class EnemyController : MonoBehaviour
{
    public Transform target { get; set; }

    [Header("GENERAL")]

    [SerializeField] EnemyType enemyType;
    [SerializeField] EnemyState enemyState;
    [SerializeField] AttackType attackType;

    [SerializeField] private BotMovement bot;

    [SerializeField] private float turningSpeed = 250f;

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
    [Range(0f, 180f)][SerializeField] private float rangedAngleDifference;
    
    [SerializeField] bool changePositionOnHit;

    private bool inRange = false;

    [Header("DEFENSIVE")]
    [Range(0f, 180f)][SerializeField] private float maxAngleDifference;
    [SerializeField] private float escapingMinDistance;

    // Start is called before the first frame update
    void Awake()
    {
        target = null;
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
        if (target != null)
        {
            // We're on the same Y axis so strip that away
            Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);
            Vector2 transformForwardVec2 = transform.forward.ToVector2(Axis.y);
            Vector2 transformSideVec2 = transform.right.ToVector2(Axis.y);

            Vector2 playerPosVec2 = target.position.ToVector2(Axis.y);

            float distance = Vector2.Distance(transformPosVec2, playerPosVec2);

            Vector3 direction = (target.position - transform.position).normalized;
            Vector2 directionVec2 = (playerPosVec2 - transformPosVec2).normalized;

            //Debug.Log("CROSS: " + Vector3.Cross(transform.forward, direction));
            
            float verticalAngle = Vector2Extensions.DotAngle(transformForwardVec2, directionVec2);

            float horizontalDot = Vector2.Dot(directionVec2, transformSideVec2);

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
                    if (verticalAngle > 0f + (rangedAngleDifference / 2f))
                    {
                        bot.Rotate(Vector3.up, turningSpeed * Mathf.Sign(horizontalDot));
                    }

                    if (distance > rangedAttackRadius)
                    {
                        // If there is an attack still happening, cut it
                        // Maybe you can play a short version of the attack to give an immersion of the AI changing its mind mid-attack
                        // NOT IMPLEMENTED YET //

                        // Approach the player                       
                        bot.Move(1f, 0f);
                        inRange = false;
                    }

                    else
                    {
                        if (!inRange)
                        {
                            inRange = true;

                            bot.Move(0f, 0f);

                            currentAimTime = Random.Range(aimIntervalMinMax.x, aimIntervalMinMax.y);
                            currentActionTime = Random.Range(actionTimeMinMax.x, actionTimeMinMax.y);
                        }

                        if (aimTimer > currentAimTime)
                        {
                            gunController.AimAtTarget(target.position, aimErrorMargin);

                            aimTimer = 0f;
                            currentAimTime = Random.Range(aimIntervalMinMax.x, aimIntervalMinMax.y);
                        }
                        else
                        {
                            aimTimer += Time.deltaTime;
                        }

                        if (actionTimer > currentActionTime)
                        {
                            if (gunController.currentAmmo <= 0)
                            {
                                gunController.Reload();
                            }
                            else
                            {
                                gunController.Shoot();
                            }

                            actionTimer = 0f;
                            currentActionTime = Random.Range(actionTimeMinMax.x, actionTimeMinMax.y);
                        }
                        else
                        {
                            actionTimer += Time.deltaTime;
                        }

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
                        bot.Rotate(Vector3.up, turningSpeed * -Mathf.Sign(horizontalDot));
                    }

                    bot.Move(1f, 0f);
                }
                else
                {
                    if (verticalAngle > 0f + (maxAngleDifference / 2f))
                    {
                        bot.Pray(false);

                        bot.Rotate(Vector3.up, turningSpeed * Mathf.Sign(horizontalDot));
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, rangedAttackRadius);
    }
}