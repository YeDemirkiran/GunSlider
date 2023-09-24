using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeneralUtilities;

public class PlayerMovement : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private CharacterController charController;

    [SerializeField] private float gravity = -9.81f;
    private float currentGravity;
    private bool canJump = false;

    [SerializeField] private Vector3 standingSpeed, crouchingSpeed;
    private Vector3 movementSpeed;
    private float preVerticalMovement = 0f, preHorizontalMovement = 0f;

    [SerializeField] private float pushSpeed = 10f, pushSpeedDiminish = 5f;
    private float currentPushSpeed;
    private bool canPush = false, willPush = false, hasPushed = false;
    private float jumpApex = 0f;


    [Header("CROUCH")]
    [SerializeField] private float standingHeight, crouchingHeight;


    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] jumpClips, pushClips;
    [SerializeField] private Vector2 pitchRandomness;

    private void Awake()
    {
        movementSpeed = standingSpeed;
        charController.height = standingHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            // CROUCH //
            if (Input.GetKey(KeyCode.LeftControl))
            {
                charController.height = crouchingHeight;
                movementSpeed = crouchingSpeed;
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                charController.height = standingHeight;
                movementSpeed = standingSpeed;
            }

            // PUSH //

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
                if (jumpApex > 1f && Input.GetKeyDown(KeyCode.Space))
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


            // GRAVITY
            currentGravity += gravity * Time.deltaTime;

            //// Jump
            if (canJump && Input.GetKey(KeyCode.Space))
            {
                canJump = false;
                hasPushed = false;

                currentGravity += movementSpeed.y;

                PlayRandomSound(audioSource, jumpClips, pitchRandomness);
            }

            float verticalMovement, horizontalMovement;

            if (charController.isGrounded)
            {
                currentGravity = 0f;

                canJump = true;
                canPush = false;
                jumpApex = 0f;

                horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed.x;
                verticalMovement = Input.GetAxis("Vertical") * movementSpeed.z;

                preHorizontalMovement = horizontalMovement;
                preVerticalMovement = verticalMovement;
            }
            else
            {
                canJump = false;

                preHorizontalMovement += (Input.GetAxis("Horizontal") / 2f) * movementSpeed.x * Time.deltaTime;
                preVerticalMovement += (Input.GetAxis("Vertical") / 2f) * movementSpeed.z * Time.deltaTime;

                horizontalMovement = preHorizontalMovement;
                verticalMovement = preVerticalMovement;
                //horizontalMovement = (Input.GetAxis("Horizontal") / 40f) * movementSpeed.z;
            }

            charController.Move(((horizontalMovement * transform.right) + ((verticalMovement + currentPushSpeed) * transform.forward) + (Vector3.up * currentGravity)) * Time.deltaTime);
        }    
    }
}