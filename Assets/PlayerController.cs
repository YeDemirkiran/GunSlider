using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BotMovement bot;

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            bot.Crouch(new KeyCode[] { KeyCode.LeftControl, KeyCode.RightControl });

            bot.Push(Input.GetKeyDown(KeyCode.Space));

            bot.Jump(Input.GetKey(KeyCode.Space));

            bot.Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

            bot.Gravity();

            bot.AnimatorAssigns(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetKeyDown(KeyCode.LeftControl));
        }
    }
}
