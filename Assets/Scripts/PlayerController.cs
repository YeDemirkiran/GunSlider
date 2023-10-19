using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private BotMovement bot;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GunController gunController;

    [SerializeField] KeyCode[] crouchKeys;

    private bool isPressingJump = false;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.Default.PauseGame.performed += ctx => GameManager.Instance.PauseGame();

        playerInput.Default.Crouch.performed += ctx => 
        {
            if (!bot.isCrouching) bot.Crouch();
            else bot.StandUp();
        };

        playerInput.Default.Jump.started += ctx => isPressingJump = true;
        playerInput.Default.Jump.canceled += ctx => isPressingJump = false;

        playerInput.Default.Push.performed += ctx => bot.Push();

        playerInput.Default.Shoot.performed += ctx => gunController.Shoot();
        playerInput.Default.Reload.performed += ctx => gunController.Reload();

        playerInput.Default.Punch.performed += ctx => bot.Punch();
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.Default.PauseGame.performed -= ctx => GameManager.Instance.PauseGame();

        playerInput.Default.Crouch.performed -= ctx =>
        {
            if (!bot.isCrouching) bot.Crouch();
            else bot.StandUp();
        };

        playerInput.Default.Jump.started -= ctx => isPressingJump = true;
        playerInput.Default.Jump.canceled -= ctx => isPressingJump = false;

        playerInput.Default.Push.performed -= ctx => bot.Push();

        playerInput.Default.Shoot.performed -= ctx => gunController.Shoot();
        playerInput.Default.Reload.performed -= ctx => gunController.Reload();

        playerInput.Default.Punch.performed -= ctx => bot.Punch();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            //foreach (KeyCode key in crouchKeys)
            //{
            //    if (Input.GetKeyDown(key))
            //    {
            //        bot.Crouch();
                    

            //        Debug.Log("Crouched");
            //        break;
            //    }
            //    else if (Input.GetKeyUp(key))
            //    {
            //        bot.StandUp();

            //        Debug.Log("Stood up");
            //        break;
            //    }
            //}

            //if (Input.GetKey(KeyCode.Space)) bot.Jump();
            //if (Input.GetKeyDown(KeyCode.Space)) bot.Push();


            Vector2 movementInput = playerInput.Default.Move.ReadValue<Vector2>();
            bot.Move(movementInput.y, movementInput.x);

            cameraController.MoveCamera(playerInput.Default.MoveCamera.ReadValue<Vector2>().x);

            if (isPressingJump) { bot.Jump(); }

            Vector2 weaponInput = playerInput.Default.MoveWeapon.ReadValue<Vector2>();
            gunController.Rotate(weaponInput.y, weaponInput.x, true);

            bot.RotateSpine(Input.GetAxis("Mouse X") * 100f, Input.GetAxis("Mouse Y") * 100f, true);
        }
    }
}