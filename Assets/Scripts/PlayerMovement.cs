using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController charController;
    [SerializeField] private Vector3 movementSpeed;
    [SerializeField] private float gravity = -9.81f;

    private float currentGravity;

    private float preHorizontalMovement = 0f, preVerticalMovement = 0f;

    private bool canJump = false;

    // Update is called once per frame
    void Update()
    {
        currentGravity += gravity * Time.deltaTime;

        if (canJump && Input.GetKey(KeyCode.Space))
        {
            canJump = false;
            currentGravity += movementSpeed.y;
        }

        //Vector3 upperMovement = (Vector3.up * currentGravity) + (Vector3.up * movementSpeed.y * System.Convert.ToByte(Input.GetKeyDown(KeyCode.Space)));

        float horizontalMovement, verticalMovement;
        
        if (charController.isGrounded)
        {
            currentGravity = 0f;
            canJump = true;

            verticalMovement = Input.GetAxis("Horizontal") * movementSpeed.z;
            horizontalMovement = Input.GetAxis("Vertical") * movementSpeed.x;

            preVerticalMovement = verticalMovement;
            preHorizontalMovement = horizontalMovement;
        }
        else
        {
            canJump = false;

            preVerticalMovement += (Input.GetAxis("Horizontal") / 2f) * movementSpeed.z * Time.deltaTime;
            preHorizontalMovement += (Input.GetAxis("Vertical") / 2f) * movementSpeed.x * Time.deltaTime;

            verticalMovement = preVerticalMovement;
            horizontalMovement = preHorizontalMovement;
            //horizontalMovement = (Input.GetAxis("Horizontal") / 40f) * movementSpeed.z;
        }

        charController.Move(((verticalMovement * transform.right) + (horizontalMovement * transform.forward) + (Vector3.up * currentGravity)) * Time.deltaTime);
    }
}
