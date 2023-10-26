using UnityEngine;
using UnityEngine.Events;
using static EnemyStates;

public class AggressiveAI : MonoBehaviour
{
    public Transform target { get; set; }

    private UnityEvent actionEvent;

    [Header("GENERAL")]

    [SerializeField] EnemyType enemyType;
    [SerializeField] AttackType attackType;

    private AttackState attackState = AttackState.Aiming;

    [SerializeField] private BotMovement bot;
    [SerializeField] private float turningSpeed = 250f;

    [SerializeField] private AimTarget aimTarget;

    private bool inRange = false;

    [Header("Ranged")]
    [SerializeField] float rangedAttackRadius;
    [Range(0f, 180f)][SerializeField] private float rangedAngleDifference;

    [SerializeField] private GunController gunController;

    [SerializeField] float aimErrorMargin = 0.25f, runningActionTimeMultiplier = 3f;

    [Tooltip("Min-Max time")][SerializeField] private Vector2 actionTimeMinMax;
    private float currentActionTime, actionTimer;

    [Tooltip("Min-Max time")][SerializeField] private Vector2 aimTimeMinMax;
    private float currentAimTime, aimTimer;

    [SerializeField] private float aimSpeed;

    [Header("SEQUENTIAL ATTACK")]
    [SerializeField] private Vector2 attackIntervalMinMax;
    private float currentAttackTime, attackTimer;

    [SerializeField][Range(0f, 1f)] private float nextAttackChance;
    private float currentAttackChance;

    [SerializeField] private int maxAttackCount;
    private int currentAttackCount;


    private void Update()
    {
        if (target == null) { return; }

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
            if (attackType == AttackType.Ranged)
            {
                // Always turn towards the player, independent of what we are currently doing.
                // So it will be on the top
                if (verticalAngle > 0f + (rangedAngleDifference / 2f))
                {
                    bot.Rotate(Vector3.up, turningSpeed * Mathf.Sign(horizontalDot));
                }


                // Approach if we are outside the range
                if (distance > rangedAttackRadius)
                {
                    bot.Move(1f, 0f);
                    inRange = false;
                }

                else
                {
                    // Run this once
                    if (!inRange)
                    {
                        inRange = true;

                        bot.Move(0f, 0f);

                        currentActionTime = Random.Range(actionTimeMinMax.x, actionTimeMinMax.y);
                        currentAimTime = Random.Range(aimTimeMinMax.x, aimTimeMinMax.y);

                        attackState = AttackState.Aiming;
                    }

                    // Attack states

                    switch (attackState)
                    {
                        case AttackState.Aiming:
                            Aim();
                            break;

                        case AttackState.Attacking:
                            Attack();
                            break;

                        default:
                            break;
                    }                        
                }
            }
        }
    }

    private void Aim()
    {
        aimTimer += Time.deltaTime;

        if (aimTimer > currentAimTime)
        {
            if (aimTarget.aimingDone)
            {
                DynamicAim.ChooseTarget(ref aimTarget, target.GetComponent<BodyHitpoints>());
                aimTarget.Aim(false, aimSpeed);
            }
            else
            {
                aimTimer = 0f;
                currentAimTime = Random.Range(aimTimeMinMax.x, aimTimeMinMax.y);

                attackState = AttackState.Attacking;
            }  
        }
    }

    private void Attack()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer > currentActionTime)
        {
            if (gunController.currentAmmo <= 0)
            {
                // Play the animation. Not implemented yet.
                gunController.Reload();
            }
            else
            {
                if (attackTimer > currentAttackTime)
                {
                    gunController.Shoot();


                    currentAttackChance = Random.Range(0f, 1f);

                    if (currentAttackCount < maxAttackCount && currentAttackChance <= nextAttackChance)
                    {
                        attackTimer = 0f;
                        currentAttackTime = Random.Range(attackIntervalMinMax.x, attackIntervalMinMax.y);

                        currentAttackCount++;
                    }
                    else
                    {
                        attackState = AttackState.Aiming;
                        currentAttackCount = 0;

                        actionTimer = 0f;
                        currentActionTime = Random.Range(actionTimeMinMax.x, actionTimeMinMax.y);
                    }

                } 
                else
                {
                    attackTimer += Time.deltaTime;
                }
            }
        }
    }
}