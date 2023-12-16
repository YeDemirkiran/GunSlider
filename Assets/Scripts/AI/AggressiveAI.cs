using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static EnemyStates;

public class Cover : Obstacle
{
    public Collider collider;
    public Vector3 coverSpot;
    public float coverHeight;

    public Cover(GameObject gameObject, BoundsCalculator boundsCalculator) : base(gameObject, boundsCalculator)
    {
        
    }

}

public class AggressiveAI : MonoBehaviour
{
    public Transform target { get; set; }

    private UnityEvent actionEvent;

    [Header("GENERAL")]

    [SerializeField] EnemyType enemyType;
    [SerializeField] AttackType attackType;

    private AttackState attackState = AttackState.Aiming;

    // COVER
    private CoverState coverState = CoverState.Covered;
    private Cover currentCover = null;

    [SerializeField] private Transform coverTest;

    [SerializeField] private BotMovement bot;
    [SerializeField] private float turningSpeed = 250f;

    [SerializeField] private TargetAim targetAim;

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

    [Header("COVER SYSTEM")]
    [SerializeField] private float coverCheckRadius;
    [SerializeField] private LayerMask coverLayerMask;


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

                    // Cover states
                    switch (coverState)
                    {
                        case CoverState.Exposed:

                            break;

                        case CoverState.Covered:

                            if (currentCover == null)
                            {
                                currentCover = FindCover(coverCheckRadius, coverLayerMask);

                                // If the cover is null, it means we don't have a cover nearby. We are fucked so get ready to fight

                                if (currentCover == null)
                                {   
                                    coverState = CoverState.Exposed;
                                }

                                //Cover testedCover = new Cover(coverTest.gameObject, coverTest.GetComponent<BoundsCalculator>());

                                //if (CheckCover(testedCover, Vector3.zero, bot.crouchingHeight, out Vector3 coverPosition, out float coverHeight))
                                //{
                                //    currentCover = testedCover;

                                //    currentCover.coverHeight = coverHeight;
                                //    currentCover.coverSpot = coverPosition;

                                //    Debug.Log("Cover is good, moving towards");
                                //}
                                //else
                                //{
                                //    Debug.Log("Cover is not good");
                                //}
                            }
                            else
                            {
                                if (MoveTowards(currentCover.collider.ClosestPoint(transform.position), 1f))
                                {
                                    Debug.Log("Reached the cover.");

                                    if (currentCover.coverHeight < bot.standingHeight)
                                    {
                                        bot.Crouch();
                                    }
                                }
                                else
                                {
                                    Debug.Log("Not at the cover yet");
                                }
                            }
                            break;

                        default:
                            break;
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

    private Cover FindCover(float radius, LayerMask layerMask)
    {
        Debug.Log("Finding a cover");

        Debug.DrawLine(transform.position, transform.position + transform.forward * radius, Color.magenta);
        Debug.Break();

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        Collider[] collidersOrdered = colliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();

        foreach (var _collider in collidersOrdered)
        {
            if (Vector3.Distance(_collider.transform.position, transform.position) <= radius)
            {
                if (_collider.TryGetComponent(out BoundsCalculator boundsCalculator))
                {
                    Cover cover = new Cover(boundsCalculator.gameObject, boundsCalculator);

                    if (CheckCover(cover, Vector3.zero, bot.crouchingHeight, out Vector3 _coverPosition, out float coverHeight))
                    {
                        cover.coverHeight = coverHeight;
                        cover.coverSpot = _coverPosition;
                        cover.collider = _collider;

                        return cover;
                    }
                }
            }          
        }

        Debug.Log("No Cover in radius.");    

        return null;
    }

    private bool CheckCover(Cover cover, Vector3 referenceGroundPosition, float crouchingHeight, out Vector3 coverPosition, out float coverHeight)
    {
        Bounds coverBounds = cover.bounds;

        // If the cover doesn't cover us, is it even a cover?
        if (CheckCoverHeight(out float _coverHeight))
        {
            // Get the best position between the target and the cover, for hiding

            Vector3 coverCoordinates = GetCoverCoordinates();

            if (CheckCoverWidth(coverCoordinates))
            {
                coverHeight = _coverHeight;
                coverPosition = coverBounds.GetPoint(coverCoordinates);
                return true;
            }
        }

        coverHeight = 0;
        coverPosition = Vector3.zero;
        return false;

        bool CheckCoverHeight(out float coverHeight)
        {
            Vector3 topFace = coverBounds.GetTop();
            Vector3 bottomFace = coverBounds.GetBottom();

            float heightBetweenGroundAndTop = topFace.y - referenceGroundPosition.y;
            float heightBetweenGroundAndBottom = bottomFace.y - referenceGroundPosition.y;

            if (heightBetweenGroundAndBottom < crouchingHeight / 2f)
            {
                if (heightBetweenGroundAndTop > crouchingHeight)
                {
                    coverHeight = heightBetweenGroundAndTop;
                    return true;
                }
            }            

            coverHeight = 0f;
            return false;
        }

        bool CheckCoverWidth(Vector3 selectedSpot)
        {
            Debug.Log("Don't forget to implement cover width checking");

            return true;
        }

        Vector3 GetCoverCoordinates()
        {
            Vector3 point = cover.CalculateClosestPoint(target.position, out Vector3 coordinates);
            Vector3 targetCornerCoordinates = -Vector3.up;

            // RIGHT CORNERS
            if (coordinates.x > 0f)
            {
                // ON THE RIGHT OF THE CORNER
                if (target.position.x > point.x)
                {
                    targetCornerCoordinates.x = -1f;
                }

                // ON THE LEFT OF THE POINT
                else
                {
                    if (target.position.z > point.z)
                    {
                        targetCornerCoordinates.z = -1f;
                    }
                    else
                    {
                        targetCornerCoordinates.z = 1f;
                    }
                }
            }

            // LEFT CORNERS
            else
            {
                // ON THE RIGHT OF THE CORNER
                if (target.position.x > point.x)
                {
                    if (target.position.z > point.z)
                    {
                        targetCornerCoordinates.z = -1f;
                    }
                    else
                    {
                        targetCornerCoordinates.z = 1f;
                    }
                }

                // ON THE LEFT OF THE POINT
                else
                {
                    targetCornerCoordinates.x = 1f;
                }
            }            

            return targetCornerCoordinates;
        }
    }

    #region MOVING

    /// <summary>
    /// First calculates whether the NPC is on the target position. Returns true if so, Returns false and moves towards target,
    /// if not.
    /// </summary>
    private bool MoveTowards(Vector3 targetPosition, float errorMargin = 0.25f, Vector2 inputOverrider = new Vector2())
    {
        if (inputOverrider.x == 0f && inputOverrider.y == 0f)
        {
            inputOverrider = Vector2.one;
        }

        Vector2 transformVec2 = transform.position.ToVector2(Axis.y);
        Vector2 targetVec2 = targetPosition.ToVector2(Axis.y);

        if (transformVec2.IsWithinRange(targetVec2, errorMargin))
        {
            return true;
        }

        Vector2 forwardDirectionVec2 = (targetPosition - transform.position).normalized.ToVector2(Axis.y);
        Vector2 sideDirectionVec2 = (targetPosition - transform.position).normalized.ToVector2(Axis.y);

        float verticalDot = Vector2.Dot(forwardDirectionVec2, transform.forward.ToVector2(Axis.y));
        float horizontalDot = Vector2.Dot(sideDirectionVec2, transform.right.ToVector2(Axis.y));

        verticalDot = Mathf.Abs(verticalDot) < 0.025f ? 0f : verticalDot;
        horizontalDot = Mathf.Abs(horizontalDot) < 0.025f ? 0f : horizontalDot;

        bot.Move(verticalDot * inputOverrider.y, horizontalDot * inputOverrider.x);

        Debug.DrawLine(transformVec2.ToVector3(Axis.y, transform.position.y), targetVec2.ToVector3(Axis.y, transform.position.y), Color.green);

        return false;
    }
    #endregion

    private void Aim()
    {
        aimTimer += Time.deltaTime;

        if (aimTimer > currentAimTime)
        {
            if (targetAim.aimingDone)
            {
                DynamicAim.ChooseTarget(ref targetAim, target.GetComponent<BodyHitpoints>());
                targetAim.Aim(false, aimSpeed);
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