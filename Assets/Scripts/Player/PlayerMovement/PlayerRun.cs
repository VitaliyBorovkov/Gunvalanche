using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    private CharacterController controller;

    public Vector2 moveInput;

    public bool isRunning;
    public bool isHoldingRunButton;

    public bool IsRunning() => isRunning;
    public bool IsHoldingRunButton() => isHoldingRunButton;
    public Vector2 GetMoveInput() => moveInput;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isRunning = isHoldingRunButton && CanRun();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public bool CanRun()
    {
        return moveInput != Vector2.zero && isHoldingRunButton && controller.isGrounded;
    }
    
    public void OnTryRunStart()
    {
        isHoldingRunButton = true;
    }

    public void OnTryRunEnd()
    {
        isHoldingRunButton = false;
    }
}
