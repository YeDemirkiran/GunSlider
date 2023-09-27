using UnityEngine;
using static GeneralUtilities;

public class BotMovement : MonoBehaviour
{
    [Header("GENERAL")]
    [SerializeField] private CharacterController charController;

    // SPEEDS
    [SerializeField] private Vector3 standingSpeed, crouchingSpeed;
    private Vector3 movementSpeed;
    private float preVerticalMovement = 0f, preHorizontalMovement = 0f;

    [SerializeField] private float gravity = -9.81f;

    private float currentGravity;
    private bool canJump = false;

    // PUSH
    [SerializeField] private float pushSpeed = 10f, pushSpeedDiminish = 5f;
    private float currentPushSpeed;
    private bool canPush = false, willPush = false, hasPushed = false;
    private float jumpApex = 0f;

    [Header("CROUCH")]
    [SerializeField] private float standingHeight;
    [SerializeField] private float crouchingHeight;
    [SerializeField] private float crouchingTransitionSeconds;
    private bool isCrouching = false;
    private float crouchLerp = 0f;
    private Vector3 meshStandingPosition, meshCrouchingPosition;

    [Header("ANIMATION")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform animatorMesh;


    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] jumpClips, pushClips;
    [SerializeField] private Vector2 pitchRandomness;

    private void Awake()
    {
        movementSpeed = standingSpeed;
        charController.height = standingHeight;
    }

    private void Start()
    {
        meshStandingPosition = animatorMesh.transform.localPosition;
        meshCrouchingPosition = animatorMesh.transform.localPosition + ((standingHeight - crouchingHeight) * Vector3.up / 2f);
    }

    private void Update()
    {
        Gravity();
    }

    // CROUCH

    public void Crouch(KeyCode[] crouchKeys)
    {
        foreach (KeyCode key in crouchKeys)
        {
            if (Input.GetKeyDown(key))
            {
                CrouchInput(true);

                break;
            }

            if (Input.GetKeyUp(key))
            {
                CrouchInput(false);

                break;
            }
        }

        CrouchCheck();
    }
    
    public void Crouch(bool crouch)
    {
        CrouchInput(crouch);
        CrouchCheck();
    }

    private void CrouchInput(bool crouch)
    {
        if (crouch)
        {
            isCrouching = true;

            //charController.height = crouchingHeight;
            //movementSpeed = crouchingSpeed;

            crouchLerp = 0f;
        }

        else
        {
            isCrouching = false;

            //charController.height = standingHeight;
            //movementSpeed = standingSpeed;

            crouchLerp = 1f;
        }
    }

    private void CrouchCheck()
    {
        if (isCrouching)
        {
            //if (charController.height > crouchingHeight)
            if (crouchLerp < 1f)
            {
                crouchLerp += Time.deltaTime / crouchingTransitionSeconds;

                // Move the root downward so the lowest point of the collider never loses touch with the ground
                charController.Move(Vector3.up * -2f * Time.deltaTime);
            }
            //else
            //{
            //    // If for some reason the lerp is 1 but the height and speed are not equal to the intended values

            //    charController.height = crouchingHeight;
            //    animatorMesh.localPosition = meshCrouchingPosition;
            //    movementSpeed = crouchingSpeed;
            //}
        }
        else
        {
            //if (charController.height < standingHeight)
            if (crouchLerp > 0f)
            {
                crouchLerp -= Time.deltaTime / crouchingTransitionSeconds;

                // Move the root upward so the lowest point of the collider never intersects the ground
                charController.Move(Vector3.up * 2f * Time.deltaTime);
            }
            //else
            //{
            //    // If for some reason the lerp is 0 but the height and speed are not equal to the intended values

            //    charController.height = standingHeight;
            //    animatorMesh.localPosition = meshStandingPosition;
            //    movementSpeed = standingSpeed;
            //}
        }

        charController.height = Mathf.Lerp(standingHeight, crouchingHeight, crouchLerp);
        animatorMesh.localPosition = Vector3.Lerp(meshStandingPosition, meshCrouchingPosition, crouchLerp);
        movementSpeed = Vector3.Lerp(standingSpeed, crouchingSpeed, crouchLerp);
    }

    // PUSH
    public void Push(bool push)
    {
        if (!hasPushed)
        {
            //// Calculating the apex
            if (jumpApex < charController.velocity.y && !canJump)
            {
                jumpApex = charController.velocity.y;
            }

            //// If we jumped and reached the apex
            if (!canJump && jumpApex > charController.velocity.y)
            {
                canPush = true;
            }

            //// If on air and jumped again, this makes sure we will push even if we pressed it earlier
            if (jumpApex > 1f && push)
            {
                willPush = true;
            }

            //// Finally push
            if (canPush && willPush)
            {
                currentPushSpeed = pushSpeed;

                willPush = false;
                canPush = false;
                jumpApex = 0f;

                hasPushed = true;

                PlayRandomSound(audioSource, pushClips, pitchRandomness);
            }
        }

        //// Decrease the push over time
        currentPushSpeed -= Time.deltaTime * pushSpeedDiminish;
        currentPushSpeed = Mathf.Clamp(currentPushSpeed, 0f, pushSpeed);
    }

    // JUMP
    public void Jump(bool jump)
    {
        if (canJump && jump)
        {
            canJump = false;
            hasPushed = false;

            currentGravity += movementSpeed.y;

            PlayRandomSound(audioSource, jumpClips, pitchRandomness);
        }
    }

    // GRAVITY
    private void Gravity()
    {
        currentGravity += gravity * Time.deltaTime;

        charController.Move(Vector3.up * currentGravity * Time.deltaTime);
    }

    // MOVEMENT
    public void Move(float verticalInput, float horizontalInput, float airMovementDivider = 2f)
    {
        float verticalMovement = verticalInput * movementSpeed.z;
        float horizontalMovement = horizontalInput * movementSpeed.x;

        if (charController.isGrounded)
        {
            currentGravity = 0f;

            canJump = true;
            canPush = false;
            jumpApex = 0f;

            preVerticalMovement = verticalMovement;
            preHorizontalMovement = horizontalMovement;
        }
        else
        {
            canJump = false;

            preVerticalMovement += verticalMovement / airMovementDivider * Time.deltaTime;
            preHorizontalMovement += horizontalMovement / airMovementDivider * Time.deltaTime;

            verticalMovement = preVerticalMovement;
            horizontalMovement = preHorizontalMovement;
        }

        // Created a Vector3 for readibility
        Vector3 movement = Vector3.zero;

        movement += (verticalMovement + currentPushSpeed) * transform.forward;
        movement += horizontalMovement * transform.right;
        //movement += Vector3.up * currentGravity;

        //charController.Move(((horizontalMovement * transform.right) + ((verticalMovement + currentPushSpeed) * transform.forward) + (Vector3.up * currentGravity)) * Time.deltaTime);
        charController.Move(movement * Time.deltaTime);
    }

    // ANIMATOR
    public void AnimatorAssignValues(float normalizedVerticalSpeed, float normalizedHorizontalSpeed, bool isCrouching)
    {
        animator.SetFloat("Vertical Speed", normalizedVerticalSpeed);
        animator.SetFloat("Horizontal Speed", normalizedHorizontalSpeed);
        animator.SetBool("Crouching", isCrouching);
    }
}