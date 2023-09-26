using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{ 
    private enum EnemyBehaviour { Offensive, Defensive }

    // PATROLLING: The BOT will patrol on the area the player is last seen
    // CHASING: Chasing the PLAYER when it spots
    // ATTACKING: Attacking the player when within the radius

    private enum OffensiveState { Patrolling, Chasing, Attacking }

    // ESCAPING: The BOT escapes and tries to evade the PLAYER's projectiles
    //           while trying to find a spot to hide.
    //
    // HIDING: Well, obvious.
    //
    // ESCAPEATTACKING: It runs backwards and attacks as it escapes.

    private enum DefensiveState { Escaping, Hiding, EscapeAttacking }

    private EnemyBehaviour currentBehaviour;

    //[SerializeField] private NavMeshAgent agent; 
    [SerializeField] private BotMovement bot; 
    [SerializeField] private Transform target;
    [SerializeField] private float pathCalculateCooldown = 5f;
    private float calculationTimer = 0f;
    private NavMeshPath path;

    private Vector3 currentTarget;
    private Vector2 primaryPosition, secondaryPosition;
    private int currentTargetIndex = 0;

    [SerializeField] private float rotationSpeed;
    private float rotationLerp = 0f;

    private void Start()
    {
        currentBehaviour = EnemyBehaviour.Offensive;

        path = new NavMeshPath();

        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

        //foreach (Vector3 corner in path.corners)
        //{
        //    Debug.Log(corner);
        //}

        //Vector2 position = new Vector2(transform.position.x, transform.position.z);
        //Vector2 targetPosition = new Vector2(currentTarget.x, currentTarget.z);
        //Vector2 direction = (position - targetPosition).normalized;

        //currentTargetIndex = 1;
        currentTarget = path.corners[currentTargetIndex];
        //transform.rotation = Quaternion.LookRotation(-direction);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBehaviour == EnemyBehaviour.Offensive)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.z);
            Vector2 targetPosition = new Vector2(currentTarget.x, currentTarget.z);

            if (Vector2.Distance(position, targetPosition) < 1f)
            {
                Debug.Log("1");

                if (currentTargetIndex + 1 < path.corners.Length)
                {
                    currentTargetIndex++;

                    currentTarget = path.corners[currentTargetIndex];

                    if (Mathf.Abs(transform.position.x - currentTarget.x) < Mathf.Abs(transform.position.z - currentTarget.z))
                    {
                        primaryPosition = new Vector2(currentTarget.x, transform.position.z);
                    }
                    else
                    {
                        primaryPosition = new Vector2(transform.position.x, currentTarget.z);
                    }

                    secondaryPosition = new Vector2(currentTarget.x, currentTarget.z);

                    Debug.Log("FULL TARGET: " + currentTarget);
                    Debug.Log("CURRENT POSITION: " + transform.position);
                    Debug.Log("X ABS: " + Mathf.Abs(transform.position.x - currentTarget.x));
                    Debug.Log("Y ABS: " + Mathf.Abs(transform.position.z - currentTarget.z));
                    Debug.Log("PRIMARY: " + primaryPosition);
                    Debug.Log("SECONDARY: " + secondaryPosition);
                }
                else
                {
                    bot.AnimatorAssigns(0f, 0f, false);
                    return;
                }
            }
            else
            {
                Debug.Log("2");
                float primaryDistance = Vector2.Distance(position, primaryPosition);

                Vector3 direction = (primaryPosition - position).normalized;

                if (primaryDistance > 0.1f)
                {
                    Debug.Log("2.1");

                    if (Vector3.Distance(transform.forward, direction) > 0.1f)
                    {
                        Debug.Log("2.1.1");

                        rotationLerp += Time.deltaTime * rotationSpeed;
                        Vector3 lookRotation = Vector3.Scale(Quaternion.LookRotation(direction).eulerAngles, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(lookRotation), rotationLerp);

                        //Debug.Log(rotationLerp);
                    }
                    else
                    {
                        rotationLerp = 0f;

                        Debug.Log("2.1.2");

                        bot.Move(direction.x, direction.y);
                        bot.AnimatorAssigns(direction.x, -direction.y, false);
                        bot.Gravity();
                    } 
                }
                else
                {
                    Debug.Log("2.2");
                    primaryPosition = secondaryPosition;
                }
            }
        }
        

        //if (calculationTimer < pathCalculateCooldown)
        //{
        //    calculationTimer += Time.deltaTime;

        //    Vector2 position = new Vector2(transform.position.x, transform.position.z);
        //    Vector2 targetPosition = new Vector2(currentTarget.x, currentTarget.z);
        //    Vector2 direction = (position - targetPosition).normalized;

        //    if (Vector2.Distance(position, targetPosition) < 2f)
        //    {
        //        if (currentTargetIndex + 1 < path.corners.Length)
        //        {
        //            currentTargetIndex++;

        //            currentTarget = path.corners[currentTargetIndex];
                    
        //            Debug.Log("hohohoh");
        //        }
        //        else
        //        {
        //            bot.AnimatorAssigns(0f, 0f, false);
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        bot.Move(1f, 0f);
        //        bot.Gravity();
        //        bot.AnimatorAssigns(1f, 0f, false);

        //        Debug.Log("CURRENT DISTANCE: " + Vector3.Distance(transform.position, currentTarget));
        //        Debug.Log("CURRENT TARGET: " + currentTarget);
        //        Debug.Log("CURRENT DIRECTION: " + direction);
        //    }

        //    transform.rotation = Quaternion.LookRotation(-direction);
        //    transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
        //}
        //else
        //{
        //    calculationTimer = 0;
        //    currentTargetIndex = 0;

        //    NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        //}
    }
}
