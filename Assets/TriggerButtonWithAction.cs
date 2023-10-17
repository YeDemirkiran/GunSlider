using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TriggerButtonWithAction : MonoBehaviour
{
    [SerializeField] private InputActionReference buttonInput;
    [SerializeField] private Button button;

    // Start is called before the first frame update
    void OnEnable()
    {
        buttonInput.action.Enable();
        buttonInput.action.performed += ctx => button.onClick.Invoke();
    }

    private void OnDisable()
    {
        buttonInput.action.Disable();
        buttonInput.action.performed -= ctx => button.onClick.Invoke();
    }
}
