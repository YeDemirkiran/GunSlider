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
                if (Input.GetKey(key))
                {
                    isCrouching = true;
                    break;
                }
                else
                {
                    isCrouching = false;
                }
            }

            bot.Crouch(isCrouching);

            bot.Push(Input.GetKeyDown(KeyCode.Space));

            bot.Jump(Input.GetKey(KeyCode.Space));

            bot.Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

            bot.AnimatorAssignValues(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetKeyDown(KeyCode.Space), isCrouching);
        }
    }
}
