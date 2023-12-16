using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;

    [SerializeField] private BotMovement bot;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GunController gunController;

    bool isPressingJump = false;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.Default.PauseGame.performed += ctx => PauseKey();

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

        //playerInput.Default.Punch.performed += ctx => bot.Punch();
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.Default.PauseGame.performed -= ctx => PauseKey();

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            Vector2 movementInput = playerInput.Default.Move.ReadValue<Vector2>();
            bot.Move(movementInput.y, movementInput.x);

            cameraController.MoveCamera(playerInput.Default.MoveCamera.ReadValue<Vector2>().x);

            if (isPressingJump) { bot.Jump(); }

            Vector2 weaponInput = playerInput.Default.MoveWeapon.ReadValue<Vector2>();
            gunController.Rotate(weaponInput.y, weaponInput.x, true);
        }
    }

    void PauseKey()
    {
        if (!GameManager.isPaused)
        {
            GameManager.Instance.Pause();
        }
        else
        {
            GameManager.Instance.Resume();
        }
    }
}