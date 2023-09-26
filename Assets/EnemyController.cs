using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private BotMovement bot;
    [SerializeField] private Transform target;
    [SerializeField] private float turningSpeed = 1f, recalculateCooldown;
    private float calculateTimer;
    private NavMeshPath path;

    private Coroutine currentDestinating = null;
    private bool canTurn = true, completedTurn = false;

    private int currentIndex;
    private Vector3 currentDestination;
    private Vector3 previousTargetPosition;

    private float currentDistanceCheck = 0f, normalDistanceCheck = 0.25f, playerDistanceCheck = 2f;

    private void Start()
    {
        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

        currentIndex = 0;
        currentDestination = path.corners[currentIndex];
        canTurn = true;
        completedTurn = false;

        currentDistanceCheck = normalDistanceCheck;
    }

    private void Update()
    {
        if (calculateTimer < recalculateCooldown)
        {
            calculateTimer += Time.deltaTime;
        }
        else
        {
            if (Vector3.Distance(previousTargetPosition, target.position) > playerDistanceCheck)
            {
                calculateTimer = 0f;

                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

                currentIndex = 0;
                currentDestination = path.corners[currentIndex];
                canTurn = true;
                completedTurn = false;

                bot.Move(0f, 0f);
                bot.AnimatorAssigns(0f, 0f, false);

                previousTargetPosition = target.position;
            }
        }

        Vector2 transformPosVec2 = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetPosVec2 = new Vector2(currentDestination.x, currentDestination.z);
        float distance = Vector2.Distance(transformPosVec2, targetPosVec2);

        if (distance < currentDistanceCheck)
        {
            if (currentIndex + 1 < path.corners.Length)
            {
                if (path.corners.Length - 2 > currentIndex)
                {
                    currentDistanceCheck = normalDistanceCheck;
                }
                else
                {
                    currentDistanceCheck = playerDistanceCheck;                    
                }

                currentIndex++;
                currentDestination = path.corners[currentIndex];                
            }
            else
            {
                bot.Move(0f, 0f);
                bot.AnimatorAssigns(0f, 0f, false);
            }

            canTurn = true;
            completedTurn = false;
        }
        else
        {
            if (!completedTurn)
            {
                if (canTurn)
                {
                    if (currentDestinating != null)
                    {
                        StopCoroutine(currentDestinating);
                    }

                    currentDestinating = StartCoroutine(RotateToDestination(path.corners[1]));

                    canTurn = false;

                    bot.Move(0f, 0f);
                    bot.AnimatorAssigns(0f, 0f, false);
                }
            }
            else
            {
                Debug.Log("MOVE TO TARGET");
                bot.Move(1f, 0f);
                bot.AnimatorAssigns(1f, 0f, false);
            }    
        }

        Debug.Log("HEHEHHEEH: " + currentDistanceCheck);

        bot.Gravity();
    }

    public IEnumerator RotateToDestination(Vector3 destination)
    {
        float turningLerp = 0f;
        Quaternion initialRotation = transform.rotation;

        Vector3 direction = (destination - transform.position).normalized;
        Quaternion targetDirection = Quaternion.LookRotation(direction);
        targetDirection = Quaternion.Euler(Vector3.Scale(targetDirection.eulerAngles, Vector3.up));

        while (turningLerp < 1f)
        {
            transform.rotation = Quaternion.Lerp(initialRotation, targetDirection, turningLerp);

            turningLerp += Time.deltaTime * turningSpeed;

            yield return null;
        }

        canTurn = false;
        completedTurn = true;
    }
}