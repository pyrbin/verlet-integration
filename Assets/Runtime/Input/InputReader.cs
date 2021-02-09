using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Globals/Input Reader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    public event UnityAction JumpEvent = delegate { };
    public event UnityAction<int> MoveEvent = delegate { };

    private GameInput GameInput;

    private void OnEnable()
    {
        if (GameInput == null)
        {
            GameInput = new GameInput();
            GameInput.Gameplay.SetCallbacks(this);
        }

        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    public void EnableGameplayInput()
    {
        GameInput.Gameplay.Enable();
    }

    public void DisableAllInput()
    {
        GameInput.Gameplay.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent.Invoke((int)context.ReadValue<float>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            JumpEvent.Invoke();
    }

}
