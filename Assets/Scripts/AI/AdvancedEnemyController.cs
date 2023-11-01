using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AdvancedEnemyController : MonoBehaviour
{
    // STATES & BEHAVIOURS
    private enum EnemyBehaviour { Offensive, Defensive, Idle }
    private enum OffensiveBehaviour { Patrol, Chase, RangedAttack, MeleeAttack }
    private enum DefensiveBehaviour { Escape, EscapeAttack, Hide }

    [Header("ENEMY BEHAVIOUR")]
    [SerializeField] private EnemyBehaviour enemyBehaviour = EnemyBehaviour.Offensive;
    [SerializeField] private OffensiveBehaviour offensiveBehaviour = OffensiveBehaviour.Patrol;
    [SerializeField] private DefensiveBehaviour defensiveBehaviour = DefensiveBehaviour.Escape;

    private EnemyBehaviour previousEnemyBehaviour;
    private OffensiveBehaviour previousOffensiveBehaviour;
    private DefensiveBehaviour previousDefensiveBehaviour;

    [Header("GENERAL")]

    // We will add a sphere collider to the player and set this to true when the bot is within
    // the range, so instead of 10+ bots checking if they can attack the player, only the player will check
    [HideInInspector] public bool playerIsVisible;
    private bool previousPlayerVisible;

    [SerializeField] private BotMovement bot;
    [SerializeField] private EntityHealth botHealth;
    [SerializeField] private Transform player;
    [SerializeField] private float turningSpeed = 1f;
    [SerializeField] private float recalculateCooldown;
    private float calculateTimer;
    private NavMeshPath path;

    [Header("OFFENSIVE")]
    [Header("Ranged Attack Settings")]
    [Tooltip("Leave blank to disable ranged attacks")][SerializeField] private GunController rangedWeapon;

    [Tooltip("If the distance is below that value, the bot will begin attack from far.")]
    [SerializeField] private float rangedAttackThreshold;

    [Header("Melee Attack Settings")]
    [Tooltip("Leave blank to disable melee attacks")][SerializeField] private GunController meleeWeapon;

    [Tooltip("If the distance is below that value, the bot will run to the player and melee attack.")]
    [SerializeField] private float meleeAttackThreshold;

    [Header("DEFENSIVE")]
    [Tooltip("If the health is below that percentage, the bot will begin to escape-attack.")]
    [Range(0f, 1f)][SerializeField] private float escapeAttackThreshold;

    [Tooltip("If the health is below that percentage, the bot will begin to fully escape.")]
    [Range(0f, 1f)][SerializeField] private float fullEscapeThreshold;

    // TURNING
    private Coroutine currentTurnAction = null;
    private bool canTurn = true, completedTurn = false;

    private int currentIndex;
    private Vector3 currentDestination,  previousPlayerPosition;

    [SerializeField] private float normalDistanceCheck = 0.25f, playerDistanceCheck = 2f;
    private float currentDistanceCheck = 0f;

    private void Awake()
    {
        playerIsVisible = false;
        previousPlayerVisible = true;

        previousEnemyBehaviour = enemyBehaviour;
        previousOffensiveBehaviour = offensiveBehaviour;
        previousDefensiveBehaviour = defensiveBehaviour;

        // Change their places if for some reason I fucked up and made the fullEscape larger
        if (escapeAttackThreshold < fullEscapeThreshold)
        {
            float temp = escapeAttackThreshold;
            escapeAttackThreshold = fullEscapeThreshold;
            fullEscapeThreshold = temp;
        }
    }

    private void Start()
    {
        path = new NavMeshPath();

        RecalculatePath();

        calculateTimer = 0f;

        currentDistanceCheck = normalDistanceCheck;

        //Debug.Log(path.corners.Length);
    }

    private void Update()
    {
        // IF ONLY THE PLAYER IS WITHING THE RANGE WE WILL DO ANYTHING

        #region PLAYER_VISIBLE_CHECK
        if (previousPlayerVisible != playerIsVisible)
        {
            if (previousEnemyBehaviour == EnemyBehaviour.Offensive)
            {
                if (playerIsVisible)
                {
                    enemyBehaviour = EnemyBehaviour.Offensive;
                }
                else
                {

                }

                previousOffensiveBehaviour = offensiveBehaviour;
            }
            else if (previousEnemyBehaviour == EnemyBehaviour.Defensive)
            {
                if (playerIsVisible)
                {
                    // CHECK THE HEALTH AND THRESHOLDS

                    if (botHealth.currentHealth / botHealth.maxHealth < fullEscapeThreshold)
                    {
                        defensiveBehaviour = DefensiveBehaviour.Escape;
                    }
                    else if (botHealth.currentHealth / botHealth.maxHealth < escapeAttackThreshold)
                    {
                        defensiveBehaviour = DefensiveBehaviour.EscapeAttack;
                    }
                    else
                    {
                        enemyBehaviour = EnemyBehaviour.Offensive;
                    }
                }
                else
                {
                    defensiveBehaviour = DefensiveBehaviour.Hide;
                }

                previousDefensiveBehaviour = defensiveBehaviour;
            }
            else
            {

            }

            if (!playerIsVisible)
            {
                // OFFENSIVE: IF WE WERE OFFENSIVE WHEN THE PLAYER WAS LAST SEEN, WE WILL SEARCH IT
                // DEFENSIVE: WE WILL KEEP HIDING 

                if (true)
                {

                }

                enemyBehaviour = EnemyBehaviour.Idle;
                previousPlayerVisible = playerIsVisible;
            }
            else
            {
                if (!previousPlayerVisible)
                {
                    

                    previousEnemyBehaviour = enemyBehaviour;
                }
            }

            previousPlayerVisible = playerIsVisible;
        }

        #endregion

        if (enemyBehaviour == EnemyBehaviour.Idle)
        {
            //bot.AnimatorAssignValues(0f, 0f, false, false);
        }

        if (enemyBehaviour == EnemyBehaviour.Offensive)
        {
            if (Vector3.Distance(transform.position, player.position) < meleeAttackThreshold)
            {
                offensiveBehaviour = OffensiveBehaviour.MeleeAttack;
            }
            else if (Vector3.Distance(transform.position, player.position) < rangedAttackThreshold)
            {
                offensiveBehaviour = OffensiveBehaviour.RangedAttack;
            }
            else
            {
                offensiveBehaviour = OffensiveBehaviour.Chase;
            }

            switch (offensiveBehaviour)
            {
                case OffensiveBehaviour.Patrol:
                    OffensivePatrol();
                    break;

                case OffensiveBehaviour.Chase:
                    OffensiveChase();
                    break;

                case OffensiveBehaviour.RangedAttack:
                    OffensiveRangedAttack();
                    break;

                case OffensiveBehaviour.MeleeAttack:
                    OffensiveRangedAttack();
                    break;
            }
        }

        if (enemyBehaviour == EnemyBehaviour.Defensive)
        {
            switch (defensiveBehaviour)
            {
                case DefensiveBehaviour.Escape:
                    DefensiveEscape();
                    break;

                case DefensiveBehaviour.EscapeAttack:
                    DefensiveEscapeAttack();
                    break;

                case DefensiveBehaviour.Hide:
                    DefensiveHide();
                    break;
            }
        }
    }

    // OFFENSIVE
    private void OffensivePatrol()
    {
        Debug.Log("Idle.");
    }

    private void OffensiveChase()
    {
        Debug.Log("Chasing!");

        //if (calculateTimer < recalculateCooldown)
        //{
        //    calculateTimer += Time.deltaTime;

        //    //Debug.Log("HAVEN'T REACHED THE COOLDOWN YET");
        //}
        //else
        //{
        //    //Debug.Log("REACHED THE COOLDOWN");

        //    if (Vector3.Distance(previousPlayerPosition, player.position) > playerDistanceCheck)
        //    {
        //        calculateTimer = 0f;

        //        RecalculatePath();

        //        bot.Move(0f, 0f);
        //        bot.AnimatorAssignValues(0f, 0f, false);

        //        previousPlayerPosition = player.position;

        //        //Debug.Log("PLAYER HAS MOVED");
        //    }
        //}

        //Vector2 transformPosVec2 = new Vector2(transform.position.x, transform.position.z);
        //Vector2 targetPosVec2 = new Vector2(currentDestination.x, currentDestination.z);
        //float distance = Vector2.Distance(transformPosVec2, targetPosVec2);


        //if (distance < currentDistanceCheck)
        //{
        //    //Debug.Log("AT THE DESTINATION");

        //    if (currentIndex + 1 < path.corners.Length)
        //    {
        //        if (path.corners.Length - 2 > currentIndex)
        //        {
        //            //Debug.Log("NOT THE FINAL DESTINATION");

        //            currentDistanceCheck = normalDistanceCheck;
        //        }
        //        else
        //        {
        //            //Debug.Log("FINAL DESTINATION");


        //            currentDistanceCheck = playerDistanceCheck;
        //        }

        //        RecalculateDestination(false);
        //    }
        //    else
        //    {
        //        //Debug.Log("REACHED THE PLAYER");

        //        bot.Move(0f, 0f);
        //        bot.AnimatorAssignValues(0f, 0f, false);

        //        offensiveBehaviour = OffensiveBehaviour.Attack;
        //    }

        //    ResetTurn();
        //}
        //else
        //{
        //    //Debug.Log("NOT AT THE DESTINATION YET");

        //    if (!completedTurn)
        //    {
        //        if (canTurn)
        //        {
        //            if (currentTurnAction != null)
        //            {
        //                StopCoroutine(currentTurnAction);
        //            }

        //            currentTurnAction = StartCoroutine(TurnToDestination(path.corners[1]));

        //            canTurn = false;

        //            bot.Move(0f, 0f);
        //            bot.AnimatorAssignValues(0f, 0f, false);
        //        }
        //    }
        //    else
        //    {
        //        //Debug.Log("MOVE TO TARGET");
        //        bot.Move(1f, 0f);
        //        bot.AnimatorAssignValues(1f, 0f, false);
        //    }
        //}
    }

    private void OffensiveRangedAttack()
    {
        Debug.Log("Ranged Attacking!");
    }

    private void OffensiveMeleeAttack()
    {
        Debug.Log("Melee Attacking!");
    }

    // DEFENSIVE
    private void DefensiveEscape()
    {
        Debug.Log("Escaping...");
    }

    private void DefensiveEscapeAttack()
    {
        Debug.Log("Attacking while escaping...");
    }

    private void DefensiveHide()
    {
        Debug.Log("Hiding...");
    }

    // GENERAL USE FUNCTIONS
    public IEnumerator TurnToDestination(Vector3 destination)
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


            //Debug.Log("TURN TO TARGET");
        }

        canTurn = false;
        completedTurn = true;
    }

    private void RecalculatePath()
    {
        NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path);

        RecalculateDestination(true);
        ResetTurn();

        ////Debug.Log("CURRENT DESTINATION: " + currentDestination);
    }

    private void RecalculateDestination(bool reset)
    {
        currentIndex = reset ? 0 : currentIndex + 1;
        currentDestination = path.corners[currentIndex];
    }

    private void ResetTurn()
    {
        canTurn = true;
        completedTurn = false;
    }
}