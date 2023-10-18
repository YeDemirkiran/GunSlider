using System.Collections;
using UnityEngine;
using static AudioUtilities;
using static AnimationUtilities;

public class BotMovement : MonoBehaviour
{
    public enum AnimationType { Melee, Rifle }

    [Header("GENERAL")]
    [SerializeField] private CharacterController charController;
    [SerializeField] private float gravity = -9.81f;

    [Header("MOVEMENT")]
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private Vector3 standingSpeed, crouchingSpeed;
    private Vector3 movementSpeed;

    private float movementLerpX, movementLerpY;
    public int xDirection, yDirection;


    private float verticalInput, horizontalInput;
    private float preVerticalMovement = 0f, preHorizontalMovement = 0f;


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
    [SerializeField] private float crouchTransitionDuration;
    private LerpFloat crouchLerp = new LerpFloat(0f);
    private Vector3 meshStandingPosition, meshCrouchingPosition;
    [HideInInspector] public bool isCrouching = false;

    [Header("GENERAL ANIMATION")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform animatorMesh;
    private Coroutine currentJumpLerp;
    private bool jumpLerpStarted;

    [Header("ANIMATION MODES")]
    [SerializeField] private AnimationType currentAnimationType = AnimationType.Melee;
    [SerializeField] private int meleeIndex, rangedIndex;

    [Header("Melee Attack Mode")]
    private LerpFloat meleeLayerWeight = new LerpFloat(0f);
    private sbyte currentPunchIndex = -1; 


    [Header("Ranged Weapon Mode")]
    [SerializeField] private Transform spineBone;
    [SerializeField] private Vector3 spineSensitivity, spineClampX, spineClampY;
    [SerializeField] private bool invertSpineX, invertSpineY;
    private Vector3 spineRotation;


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

        if (currentAnimationType == AnimationType.Rifle)
        {
            spineRotation = spineBone.localEulerAngles;
        }
    }

    private void Update()
    {
        Gravity();
        PushInternal();

        // ANIMATOR MODE
        switch (currentAnimationType)
        {
            case AnimationType.Melee:
                AnimatorStateInfo meleeInfo = animator.GetCurrentAnimatorStateInfo(meleeIndex);

                if (meleeInfo.IsTag("Punch")
                    && meleeInfo.normalizedTime >= 0.75f && !meleeLayerWeight.isLerping)
                {
                    meleeLayerWeight.Lerp(this, 0f, 0.25f);
                    currentPunchIndex = -1;
                }

                animator.SetLayerWeight(meleeIndex, meleeLayerWeight);

                break;
            case AnimationType.Rifle:
                //animator.SetLayerWeight(rangedIndex, 1f);
                break;
        }
    }

    private void LateUpdate()
    {
        // We need to animate the bone at the late update
        if (currentAnimationType == AnimationType.Rifle)
        {
            spineBone.localEulerAngles = spineRotation;
        }

        charController.height = Mathf.Lerp(standingHeight, crouchingHeight, crouchLerp);
        animatorMesh.localPosition = Vector3.Lerp(meshStandingPosition, meshCrouchingPosition, crouchLerp);
        movementSpeed = Vector3.Lerp(standingSpeed, crouchingSpeed, crouchLerp);

        MoveInternal();

        AnimatorAssignValues(yDirection * movementLerpY, xDirection * movementLerpX, false, crouchLerp);
    }

    // Ranged Mode 
    public void RotateSpine(float deltaX, float deltaY, bool invertY)
    {
        if (currentAnimationType == AnimationType.Rifle)
        {
            spineRotation.x += (invertY ? -1f : 1f) * deltaY * Time.deltaTime;
            spineRotation.x = Mathf.Clamp(spineRotation.x, spineClampX.x, spineClampX.y);

            spineRotation.y += deltaX * Time.deltaTime;
        }
    }

    // Melee Mode
    public void Punch()
    {
        if (meleeLayerWeight < 1f && !meleeLayerWeight.isLerping)
        {
            animator.Play("None", meleeIndex, 0f);
            meleeLayerWeight.Lerp(this, 1f, 0.1f);
            animator.SetTrigger("Jab");
            currentPunchIndex = 0;
        }
    }

    #region PUBLIC

    public void Move(float verticalInput, float horizontalInput)
    {
        this.verticalInput = verticalInput;
        this.horizontalInput = horizontalInput;     
    }

    public void MoveInternal()
    {
        // FUCK UNITY AND THE NEW INPUT SYSTEM IT DOESN'T HAVE SMOOTHING
        // YOU LAZY FUCKS
        // SO WE HAVE TO DO IT

        // HORIZONTAL MOVEMENT WITH SMOOTHING
        if (horizontalInput != 0)
        {
            if (xDirection != System.Math.Sign(horizontalInput))
            {
                if (movementLerpX > 0f)
                {
                    movementLerpX -= Time.deltaTime / accelerationSpeed * 2f;
                }
                else
                {
                    xDirection = System.Math.Sign(horizontalInput);
                }
            }
            else
            {
                movementLerpX += Time.deltaTime / accelerationSpeed;
            }    
        }
        else
        {
            movementLerpX -= Time.deltaTime / accelerationSpeed;
        }
        movementLerpX = Mathf.Clamp(movementLerpX, 0f, 1f);

        // VERTICAL MOVEMENT WITH SMOOTHING
        if (verticalInput != 0)
        {
            if (yDirection != System.Math.Sign(verticalInput))
            {
                if (movementLerpY > 0f)
                {
                    movementLerpY -= Time.deltaTime / accelerationSpeed * 2f;
                }
                else
                {
                    yDirection = System.Math.Sign(verticalInput);
                }
            }
            else
            {
                movementLerpY += Time.deltaTime / accelerationSpeed;
            }
        }
        else
        {
            movementLerpY -= Time.deltaTime / accelerationSpeed;
        }
        movementLerpY = Mathf.Clamp(movementLerpY, 0f, 1f);

        //Debug.Log("X BEFORE: " + movementLerpX);
        //Debug.Log("X AFTER: " + movementLerpX);
        //Debug.Log("LERP Y: " + movementLerpY);
        //Debug.Log("LERP X: " + movementLerpX);

        //Debug.Log("ABS Y: " + Mathf.Abs(verticalInput));
        //Debug.Log("ABS X: " + Mathf.Abs(horizontalInput));

        //Debug.Log("SIGN Y: " + System.Math.Sign(verticalInput));
        //Debug.Log("SIGN X: " + System.Math.Sign(horizontalInput));

        float verticalMovement = yDirection * movementLerpY * movementSpeed.z;
        float horizontalMovement = xDirection * movementLerpX * movementSpeed.x;

        if (charController.isGrounded)
        {
            preVerticalMovement = verticalMovement;
            preHorizontalMovement = horizontalMovement;
        }
        else
        {
            preVerticalMovement += verticalMovement / 2f * Time.deltaTime;
            preHorizontalMovement += horizontalMovement / 2f * Time.deltaTime;

            verticalMovement = preVerticalMovement;
            horizontalMovement = preHorizontalMovement;
        }

        // Created a Vector3 for readibility
        Vector3 movement = Vector3.zero;

        movement += (verticalMovement + currentPushSpeed) * transform.forward;
        movement += horizontalMovement * transform.right;

        charController.Move(movement * Time.deltaTime);
    }

    public void Jump()
    {
        if (canJump)
        {
            canJump = false;
            hasPushed = false;

            currentGravity += movementSpeed.y;

            PlayRandomSound(audioSource, jumpClips, pitchRandomness);
        }
    }

    public void Push()
    {
        if (!hasPushed)
        {
            //// If on air and jumped again, this makes sure we will push even if we pressed it earlier
            if (jumpApex > 1f)
            {
                willPush = true;
            }
        }
    }

    public void Crouch()
    {
        if (!isCrouching)
        {
            crouchLerp.Lerp(this, 1f, crouchTransitionDuration);
            isCrouching = true;
        }

        //        charController.Move(Vector3.up * direction * 2f * Time.deltaTime);
    }

    public void StandUp()
    {
        crouchLerp.Lerp(this, 0f, crouchTransitionDuration);
        isCrouching = false;
    }

    #endregion

    #region INTERNAL

    #region General

    private void Gravity()
    {
        currentGravity += gravity * Time.deltaTime;
        charController.Move(Vector3.up * currentGravity * Time.deltaTime);

        if (charController.isGrounded)
        {
            currentGravity = 0f;

            if (charController.velocity.y <= jumpApex)
            {
                canJump = true;
                canPush = false;
                jumpApex = 0f;
            }
        }
        else
        {
            canJump = false;
        }
    }

    private void PushInternal()
    {
        if (!hasPushed)
        {
            // If we jumped
            if (!canJump)
            {
                // Calculating the apex
                if (jumpApex < charController.velocity.y)
                {
                    jumpApex = charController.velocity.y;
                    canPush = false;
                }

                //// If we jumped and reached the apex
                else
                {
                    canPush = true;
                }
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

    #endregion

    #region Animator
    private void AnimatorAssignValues(float normalizedVerticalSpeed, float normalizedHorizontalSpeed, bool hasJumped, float crouchingValue)
    {
        animator.SetFloat("Vertical Speed", normalizedVerticalSpeed);
        animator.SetFloat("Horizontal Speed", normalizedHorizontalSpeed);

        if (hasJumped)
        {
            if (animator.GetFloat("JumpingPose") < 1f)
            {
                if (!jumpLerpStarted)
                {
                    if (currentJumpLerp != null)
                    {
                        StopCoroutine(currentJumpLerp);
                    }

                    currentJumpLerp = StartCoroutine(LerpJumpingFloat(1f, 0.1f, "JumpingPose"));
                }
            }       
        }

        else
        {
            if (animator.GetFloat("JumpingPose") > 0f)
            {
                if (!jumpLerpStarted)
                {
                    if (currentJumpLerp != null)
                    {
                        StopCoroutine(currentJumpLerp);
                    }

                    currentJumpLerp = StartCoroutine(LerpJumpingFloat(0f, 0.1f, "JumpingPose"));
                    //StopCoroutine(currentCrouchLerp);
                }
            }

            animator.SetFloat("JumpingPose", 0f);
        }

        if (canJump && hasJumped)
        {
            animator.SetBool("Has Jumped", hasJumped);
        }

        animator.SetFloat("CrouchingPose", crouchingValue);
        //animator.SetBool("Is Crouching", isCrouching);
    }

    private IEnumerator LerpJumpingFloat(float target, float duration, string variableName)
    {
        float timer = 0f;
        float original = animator.GetFloat(variableName);
        jumpLerpStarted = true;

        while (timer < 1)
        {
            timer += Time.deltaTime / duration;

            animator.SetFloat(variableName, Mathf.Lerp(original, target, timer));

            yield return null;
        }

        jumpLerpStarted = false;
    }
    #endregion

    #endregion
}