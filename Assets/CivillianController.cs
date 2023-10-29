using UnityEngine;
using UnityEngine.UIElements;
using static EnemyStates;

public class CivillianController : MonoBehaviour
{
    public Transform target { get; set; }

    [Header("GENERAL")]

    [SerializeField] EnemyType enemyType;

    [SerializeField] private BotMovement bot;

    [SerializeField] private float turningSpeed = 250f;

    [Range(0f, 180f)][SerializeField] private float maxAngleDifference;
    [SerializeField] private float escapingMinDistance;

    void Update()
    {
        if (target != null)
        {
            Vector2 transformPosVec2 = transform.position.ToVector2(Axis.y);
            Vector2 transformForwardVec2 = transform.forward.ToVector2(Axis.y);
            Vector2 transformSideVec2 = transform.right.ToVector2(Axis.y);

            Vector2 targetPosVec2 = target.position.ToVector2(Axis.y);

            float distance = Vector2.Distance(transformPosVec2, targetPosVec2);

            Vector2 directionVec2 = (targetPosVec2 - transformPosVec2).normalized;

            float verticalAngle = Vector2Extensions.DotAngle(transformForwardVec2, directionVec2);

            Debug.Log("Barr: " + transformPosVec2);
            Debug.Log("Barr2: " + targetPosVec2);

            float horizontalDot = Vector2.Dot(directionVec2, transformSideVec2);

            if (enemyType == EnemyType.Defensive)
            {
                if (distance > escapingMinDistance)
                {
                    bot.Pray(false);

                    TurnAwayFromPoint(target.position, maxAngleDifference / 2f);

                    bot.Move(1f, 0f);
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