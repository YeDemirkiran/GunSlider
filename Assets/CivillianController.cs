using UnityEngine;
using static EnemyStates;

public class CivillianController : MonoBehaviour
{
    public Transform target { get; set; }

    public Obstacle currentObstacle { get; set; }

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
                        Bounds obstacleBounds = currentObstacle.bounds;
                        Bounds ownBounds = ownBoundsCalculator.CalculateBounds();

                        if (obstacleBounds.GetTop().y - ownBounds.GetBottom().y < smallObjectJumpingThreshold)
                        {
                            if (Vector3.Distance(currentObstacle.transform.position, transform.position) < smallObjectJumpingDistance)
                            {
                                bot.Jump();
                            }

                            bot.Move(1f, 0f);
                        }
                        else
                        {
                            Vector3 closestPoint = currentObstacle.CalculateClosestPoint(transform.position);
                            Vector3 targetPoint = closestPoint + ownBounds.size;

                            Vector2 obstacleDirectionVec2 = (obstacleBounds.center - transform.position).ToVector2(Axis.y);
                            float horizontalDot = Vector2.Dot(obstacleDirectionVec2, transform.right.ToVector2(Axis.y));

                            //TurnTowardsPoint(targetPoint, 0f);

                            bot.Move(0f, -horizontalDot);

                            Debug.DrawLine(closestPoint, targetPoint, Color.blue);
                            Debug.DrawLine(transform.position, closestPoint, Color.red);

                            //Debug.Log("CLOSEST: " + closestPoint);
                            //Debug.Log("TARGET: " + targetPoint);
                            //Debug.Break();
                        }
                    }
                    else
                    {
                        TurnAwayFromPoint(target.position, maxAngleDifference / 2f);
                        bot.Move(1f, 0f);
                    } 
                    

                    // Reset it. The Obstacle Detector will give it us again, and if there is none, this will stay null.
                    currentObstacle = null;
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
        }    
    }

    void TurnTowardsPoint(Vector3 point, float margin)
    {
        Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);
        Vector2 transformForwardVec2 = transform.forward.ToVector2(Axis.y);
        Vector2 transformSideVec2 = transform.right.ToVector2(Axis.y);

        Vector2 pointPosVec2 = point.ToVector2(Axis.y);

        Vector2 directionBetweenPointVec2 = (pointPosVec2 - transformPosVec2).normalized;

        float verticalAngleBetweenTarget = Vector2Extensions.DotAngle(transformForwardVec2, directionBetweenPointVec2);

        float horizontalDot = Vector2.Dot(directionBetweenPointVec2, transformSideVec2);

        if (verticalAngleBetweenTarget > 0f + margin)
        {
            bot.Rotate(Vector3.up, turningSpeed * Mathf.Sign(horizontalDot));
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
}