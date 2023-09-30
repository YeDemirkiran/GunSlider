using System.Collections;
using UnityEngine;
using static AudioUtilities;
using static AnimationUtilities;

public class BotMovement : MonoBehaviour
{
    public enum AnimationType { Melee, Rifle }

    [Header("GENERAL")]
    [SerializeField] private CharacterController charController;

    // SPEEDS
    [SerializeField] private Vector3 standingSpeed, crouchingSpeed;
    private Vector3 movementSpeed;

    private float verticalInput, horizontalInput;
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
    [SerializeField] private float crouchTransitionDuration;
    private LerpFloat crouchLerp = new LerpFloat(0f);
    private Vector3 meshStandingPosition, meshCrouchingPosition;

    [Header("GENERAL ANIMATION")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform animatorMesh;
    private Coroutine currentCrouchLerp, currentJumpLerp;
    private bool crouchLerpStarted, jumpLerpStarted;

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
    private Vector3 spineeRotation;


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

        spineeRotation = spineBone.localEulerAngles;
    }

    private void Update()
    {
        Gravity();

        // ANIMATOR MODE
        switch (currentAnimationType)
        {
            case AnimationType.Melee:
                AnimatorStateInfo meleeInfo = animator.GetCurrentAnimatorStateInfo(meleeIndex);

                //Debug.Log("BANAN: " + meleeInfo[0].clip.name);
                //Debug.Log("BANAN: " + animator.GetCurrentAnimatorClipInfo(meleeIndex)[0].clip.name);

                if (meleeInfo.IsTag("Punch"))
                {
                    //Debug.Log(meleeInfo.normalizedTime);
                }

                if (meleeInfo.IsTag("Punch")
                    && meleeInfo.normalizedTime >= 0.75f && !meleeLayerWeight.isLerping)
                {

                     Debug.Log("Woo");
                //    animator.Play("None", meleeIndex, 0f);
                    meleeLayerWeight.Lerp(this, 0f, 0.25f);
                    currentPunchIndex = -1;
                }

                animator.SetLayerWeight(meleeIndex, meleeLayerWeight);

                break;
            case AnimationType.Rifle:
                //animator.SetLayerWeight(rangedIndex, 1f);
                break;
        }

        //Debug.Log("CURRENT MELEE WEIGHT: " + animator.GetLayerWeight(meleeIndex));
    }

    private void LateUpdate()
    {
        if (currentAnimationType == AnimationType.Rifle)
        {
            spineBone.localEulerAngles = spineeRotation;
        }

        charController.height = Mathf.Lerp(standingHeight, crouchingHeight, crouchLerp);
        animatorMesh.localPosition = Vector3.Lerp(meshStandingPosition, meshCrouchingPosition, crouchLerp);
        movementSpeed = Vector3.Lerp(standingSpeed, crouchingSpeed, crouchLerp);

        AnimatorAssignValues(verticalInput, horizontalInput, false, crouchLerp);
    }

    // CROUCH

    //public void Crouch(KeyCode[] crouchKeys)
    //{
    //    foreach (KeyCode key in crouchKeys)
    //    {
    //        if (Input.GetKeyDown(key))
    //        {
    //            CrouchInput(true);

    //            break;
    //        }

    //        if (Input.GetKeyUp(key))
    //        {
    //            CrouchInput(false);

    //            break;
    //        }
    //    }

    //    CrouchCheck();
    //}

    // GENERAL MOVEMENT

    // Movement
    public void Move(float verticalInput, float horizontalInput, float airMovementDivider = 2f)
    {
        this.verticalInput = verticalInput;
        this.horizontalInput = horizontalInput;

        float verticalMovement = verticalInput * movementSpeed.z;
        float horizontalMovement = horizontalInput * movementSpeed.x;

        if (charController.isGrounded)
        {
            currentGravity = 0f;

            if (charController.velocity.y <= jumpApex)
            {
                canJump = true;
                canPush = false;
                jumpApex = 0f;
            }

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

    //// GRAVITY
    private void Gravity()
    {
        currentGravity += gravity * Time.deltaTime;

        charController.Move(Vector3.up * currentGravity * Time.deltaTime);
    }

    //// Jump
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

    //// Push
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

    //// Crouch
    public void Crouch(bool crouch)
    {
        crouchLerp.Lerp(this, crouch ? 1f : 0f, crouchTransitionDuration);

        //if (currentCrouchLerp != null)
        //{
        //    StopCoroutine(currentCrouchLerp);
        //}

        //currentCrouchLerp = StartCoroutine(CrouchLerp(crouch ? 1f : 0f, crouchTransitionDuration));
    }

    //private IEnumerator CrouchLerp(float target, float duration)
    //{
    //    float original = crouchLerp;
    //    float timer = 0f;
    //    float direction = Mathf.Sign(original - target);

    //    while (timer < 1.1f)
    //    {
    //        timer  += Time.deltaTime / duration;

    //        crouchLerp = Mathf.Lerp(original, target, timer);
    //        charController.Move(Vector3.up * direction * 2f * Time.deltaTime);

    //        //Debug.Log("LERP: " + crouchLerp);

    //        yield return null;
    //    }

    //    crouchLerp = target;
    //}
    

    // ANIMATOR
    public void AnimatorAssignValues(float normalizedVerticalSpeed, float normalizedHorizontalSpeed, bool hasJumped, float crouchingValue)
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
                    //StopCoroutine(currentCrouchLerp);
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

    

    // Ranged Mode 
    public void RotateSpine(float deltaX, float deltaY, bool invertY)
    {
        if (currentAnimationType == AnimationType.Rifle)
        {
            spineeRotation.x += (invertY ? -1f : 1f) * deltaY * Time.deltaTime;
            spineeRotation.x = Mathf.Clamp(spineeRotation.x, spineClampX.x, spineClampX.y);

            spineeRotation.y += deltaX * Time.deltaTime;

            Debug.Log("ROTATING");
        }
    }

    // Melee Mode
    public void Punch()
    {
        if (meleeLayerWeight < 1f && !meleeLayerWeight.isLerping)
        {
            animator.Play("None", meleeIndex, 0f);
            //animator.Play("None", meleeIndex, 0f);
            meleeLayerWeight.Lerp(this, 1f, 0.1f);
            animator.SetTrigger("Jab");
            currentPunchIndex = 0;

            Debug.Log("JABBBBBB");
        }
    }
}