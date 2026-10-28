using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    private CharacterController controller;

    private Vector2 moveInput;

    private bool isRunning;
    private bool isHoldingRunButton;

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
        if (controller.isGrounded)
        {
            return moveInput != Vector2.zero && isHoldingRunButton;
        }

        // Airborne: don't require isGrounded here too, or a jump instantly cancels the sprint
        // speed it was launched with (PlayerMovement recomputes horizontal speed from IsRunning()
        // every frame — there's no separate momentum to preserve, so the run flag itself has to
        // survive the jump). Only keeps sprinting if it was already running before leaving the
        // ground — holding the run button while airborne can't start a sprint that wasn't there.
        return isRunning && isHoldingRunButton;
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
