using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BotMovement bot;

    [SerializeField] KeyCode[] crouchKeys;

    private bool isCrouching = false;

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            foreach (KeyCode key in crouchKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    bot.Crouch(true);
                    isCrouching = true;

                    Debug.Log("Crouched");
                    break;
                }
                else if (Input.GetKeyUp(key))
                {
                    bot.Crouch(false);
                    isCrouching = false;

                    Debug.Log("Stood up");
                    break;
                }
            }

            


            if (Input.GetKey(KeyCode.Mouse1))
            {
                bot.Punch();
            }


            bot.Push(Input.GetKeyDown(KeyCode.Space));

            bot.Jump(Input.GetKey(KeyCode.Space));

            bot.Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            

            bot.RotateSpine(Input.GetAxis("Mouse X") * 100f, Input.GetAxis("Mouse Y") * 100f, true);

            //bot.AnimatorAssignValues(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetKeyDown(KeyCode.Space), isCrouching);
        }
    }
}
