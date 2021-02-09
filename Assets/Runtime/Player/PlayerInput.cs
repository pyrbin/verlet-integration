using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    private CharacterController Controller;

    [SerializeField]
    private InputReader InputReader;

    void Start()
    {
        Controller = GetComponent<CharacterController>();

        InputReader.JumpEvent += () =>
        {
            Controller.Jump();
        };

        InputReader.MoveEvent += (input) =>
        {
            Controller.MovementInput = input;
        };

        InputReader.EnableGameplayInput();
    }
}
