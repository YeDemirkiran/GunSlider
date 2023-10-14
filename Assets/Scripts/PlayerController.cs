using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private BotMovement bot;

    [SerializeField] KeyCode[] crouchKeys;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            foreach (KeyCode key in crouchKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    bot.Crouch();

                    Debug.Log("Crouched");
                    break;
                }
                else if (Input.GetKeyUp(key))
                {
                    bot.StandUp();

                    Debug.Log("Stood up");
                    break;
                }
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                bot.Punch();
            }

            if (Input.GetKey(KeyCode.Space)) bot.Jump();
            if (Input.GetKeyDown(KeyCode.Space)) bot.Push();

            Vector2 movementInput = playerInput.Default.Move.ReadValue<Vector2>();

            if (Mathf.Abs(movementInput.x) + Mathf.Abs(movementInput.y) > 0.01f)
            {
                bot.Move(movementInput.y, movementInput.x);
            }
            

            bot.RotateSpine(Input.GetAxis("Mouse X") * 100f, Input.GetAxis("Mouse Y") * 100f, true);

            //bot.AnimatorAssignValues(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetKeyDown(KeyCode.Space), isCrouching);
        }
    }
}
