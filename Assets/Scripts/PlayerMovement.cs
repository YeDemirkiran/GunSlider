using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController charController;
    [SerializeField] private Vector3 movementSpeed;
    [SerializeField] private float gravity = -9.81f;

    private float currentGravity;

    // Update is called once per frame
    void Update()
    {
        currentGravity += (gravity + (movementSpeed.y * System.Convert.ToByte(Input.GetKeyDown(KeyCode.Space)))) * Time.deltaTime;
        //Vector3 upperMovement = (Vector3.up * currentGravity) + (Vector3.up * movementSpeed.y * System.Convert.ToByte(Input.GetKeyDown(KeyCode.Space)));

        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed.z;

        charController.Move(((horizontalMovement * transform.forward) + (Vector3.up * currentGravity)) * Time.deltaTime);

        if (charController.isGrounded)
        {
            currentGravity = 0f;
        }
    }
}
