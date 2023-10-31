using UnityEngine;
using static EnemyStates;

public enum CurrentChasedTarget { primaryTarget, secondaryTarget }

public class CivillianController : MonoBehaviour
{
    public Transform target { get; set; }

    private CurrentChasedTarget currentChasedTarget { get; set; } = CurrentChasedTarget.primaryTarget;
    private Vector3 secondaryTargetPosition;

    public Obstacle currentObstacle { get; set; } = null;

    [Header("GENERAL")]
    [SerializeField] EnemyType enemyType;
    [SerializeField] private BotMovement bot;

    [SerializeField] private float turningSpeed = 250f;

    [Range(0f, 180f)][SerializeField] private float maxAngleDifference;
    [SerializeField] private float escapingMinDistance;

    [Header("Object Avoidance")]
    [SerializeField] private BoundsCalculator ownBoundsCalculator;
    [SerializeField] private ObstacleDetector obstacleDetector;

    [SerializeField] private float smallObjectJumpingDistance = 1f, smallObjectJumpingThreshold = 0.5f;

    void Update()
    {
        if (GameManager.isPaused) { return; }


        if (target != null)
        {
            Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);
            Vector2 transformForwardVec2 = transform.forward.ToVector2(Axis.y);

            Vector2 targetPosVec2 = target.position.ToVector2(Axis.y);

            float distance = Vector2.Distance(transformPosVec2, targetPosVec2);

            Vector2 directionVec2 = (targetPosVec2 - transformPosVec2).normalized;

            float verticalAngle = Vector2Extensions.DotAngle(transformForwardVec2, directionVec2);


            if (enemyType == EnemyType.Defensive)
            {
                if (distance > escapingMinDistance)
                {
                    bot.Pray(false);                    

                    if (currentObstacle != null)
                    {
                        Bounds ownBounds = ownBoundsCalculator.CalculateBounds();
                        Bounds obstacleBounds = currentObstacle.bounds;

                        if (CanJumpOverObstacle(ownBounds, obstacleBounds, smallObjectJumpingThreshold))
                        {
                            if (transform.position.IsWithinRange(obstacleBounds.center, smallObjectJumpingDistance))
                            {
                                bot.Jump();
                            }
                        }
                        else
                        {
                            //Debug.Log("Transform: " + transform.position);

                            Vector3 closestPoint = currentObstacle.CalculateClosestPoint(transform.position, out Vector3 faceCoordinates);
                            Vector3 targetPoint = default;

                            Vector3 ownBoundsSize = ownBounds.size;       

                            Vector2 pointDirectionVec2 = (closestPoint - transform.position).normalized.ToVector2(Axis.y);

                            if (transform.position.x > closestPoint.x)
                            {
                                Debug.Log("Right");
                            }
                            else
                            {
                                Debug.Log("Left");
                            }

                            if (transform.position.z > closestPoint.z)
                            {
                                Debug.Log("Front");
                            }
                            else
                            {
                                Debug.Log("Back");
                            }

                            //float verticalDot = Vector2.Dot(pointDirectionVec2, transform.forward.ToVector2(Axis.y));
                            //float horizontalDot = Vector2.Dot(pointDirectionVec2, transform.right.ToVector2(Axis.y));

                            //// On the left of the point
                            //if (horizontalDot > 0f)
                            //{
                            //    ownBoundsSize.y = ownBoundsSize.x = 0f;

                            //    targetPoint = closestPoint + (ownBoundsSize * faceCoordinates.z);
                            //    Debug.Log("wuwuwuuw");
                            //}

                            //// On the right of the point
                            //else
                            //{
                            //    ownBoundsSize.y = ownBoundsSize.z = 0f;

                            //    targetPoint = closestPoint + (ownBoundsSize * faceCoordinates.x);
                            //    Debug.Log("quququqqu");
                            //}

                            //Debug.Log("VERT: " + verticalDot);
                            //Debug.Log("HORT: " + horizontalDot);

                            targetPoint.y = transform.position.y;

                            //secondaryTargetPosition = transform.position + Vector3.Scale(ownBounds.size, faceCoordinates);
                            secondaryTargetPosition = targetPoint;

                            currentChasedTarget = CurrentChasedTarget.secondaryTarget;

                            //Debug.Log("closestPoint: " + closestPoint);

                            //Debug.Log("COORDINATES: " + faceCoordinates);
                            //Debug.Log("SIZE: " + ownBoundsSize);
                            Debug.DrawLine(closestPoint, targetPoint, Color.blue);

                            Debug.Break();
                            //Debug.DrawLine(transform.position, transform.position + pointDirectionVec2.ToVector3(Axis.y, 0f), Color.red);
                            Debug.DrawLine(transform.position, closestPoint, Color.red);
                            //Debug.DrawLine(transform.position, transform.position + Vector3.Scale(ownBounds.size, faceCoordinates), Color.magenta);
                        }
                    }

                    // Reset it. The Obstacle Detector will give it us again, and if there is none, this will stay null.

                    switch (currentChasedTarget)
                    {
                        case CurrentChasedTarget.primaryTarget:
                            TurnAwayFromPoint(target.position, maxAngleDifference / 2f);
                            bot.Move(1f, 0f);
                            break;

                        case CurrentChasedTarget.secondaryTarget:
                            currentObstacle = null;

                            Debug.Log("Null");

                            if (MoveTowards(secondaryTargetPosition))
                            {
                                currentChasedTarget = CurrentChasedTarget.primaryTarget;
                            }
                            else
                            {
                                TurnTowardsPoint(secondaryTargetPosition, 1f);
                            }

                            break;
                    }
                }

                else
                {
                    // When we are about to pray, we are most likely turned away from the enemy
                    // Instead of instant turn, we turn slowly
                    if (verticalAngle > 0f + (maxAngleDifference / 2f))
                    {
                        bot.Pray(false);

                        TurnTowardsPoint(target.position, maxAngleDifference / 2f);
                    }

                    // We are facing the enemy
                    // God help this poor soul, or forgive its sins in case the player decides to blast its brains out lmao
                    else
                    {
                        bot.Pray(true);
                    }
                }
            }

            Debug.Log(currentChasedTarget);
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
            Debug.Log("ehu");
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

    #region ROTATING
    void TurnTowardsPoint(Vector3 point, float margin, float speed = 0f)
    {
        if (speed == 0f) speed = turningSpeed;

        Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);
        Vector2 transformForwardVec2 = transform.forward.ToVector2(Axis.y);
        Vector2 transformSideVec2 = transform.right.ToVector2(Axis.y);

        Vector2 pointPosVec2 = point.ToVector2(Axis.y);

        Vector2 directionBetweenPointVec2 = (pointPosVec2 - transformPosVec2).normalized;

        float verticalAngleBetweenTarget = Vector2Extensions.DotAngle(transformForwardVec2, directionBetweenPointVec2);

        float horizontalDot = Vector2.Dot(directionBetweenPointVec2, transformSideVec2);

        if (verticalAngleBetweenTarget > 0f + margin)
        {
            bot.Rotate(Vector3.up, speed * Mathf.Sign(horizontalDot));
        }
    }

    void TurnTowardsPoint(Vector2 point, float margin)
    {
        Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);
        Vector2 transformForwardVec2 = transform.forward.ToVector2(Axis.y);
        Vector2 transformSideVec2 = transform.right.ToVector2(Axis.y);

        Vector2 pointPosVec2 = point;

        Vector2 directionBetweenPointVec2 = (pointPosVec2 - transformPosVec2).normalized;

        float verticalAngleBetweenTarget = Vector2Extensions.DotAngle(transformForwardVec2, directionBetweenPointVec2);

        float horizontalDot = Vector2.Dot(directionBetweenPointVec2, transformSideVec2);

        if (verticalAngleBetweenTarget > 0f + margin)
        {
            bot.Rotate(Vector3.up, turningSpeed * Mathf.Sign(horizontalDot));
        }
    }

    void TurnAwayFromPoint(Vector3 point, float margin)
    {
        Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);
        Vector2 transformForwardVec2 = transform.forward.ToVector2(Axis.y);
        Vector2 transformSideVec2 = transform.right.ToVector2(Axis.y);

        Vector2 pointPosVec2 = point.ToVector2(Axis.y);

        Vector2 directionBetweenPointVec2 = (pointPosVec2 - transformPosVec2).normalized;

        float verticalAngleBetweenTarget = Vector2Extensions.DotAngle(transformForwardVec2, directionBetweenPointVec2);

        float horizontalDot = Vector2.Dot(directionBetweenPointVec2, transformSideVec2);

        if (verticalAngleBetweenTarget < 180f - margin)
        {
            bot.Rotate(Vector3.up, turningSpeed * -Mathf.Sign(horizontalDot));
        }
    }
    #endregion

    #region CALCULATIONS
    private bool CanJumpOverObstacle(Bounds ownBounds, Bounds obstacleBounds, float threshold)
    {
        if (obstacleBounds.GetTop().y - ownBounds.GetBottom().y < threshold)
        {
            return true;
        }

        return false;
    } 

    
    #endregion
}