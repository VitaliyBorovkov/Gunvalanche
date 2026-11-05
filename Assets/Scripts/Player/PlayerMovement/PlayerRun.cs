using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    private CharacterController controller;

    private Vector2 moveInput;

    private bool isRunning;
    private bool isHoldingRunButton;
    private bool wasRunningOnLaunch;
    private bool prevGrounded;

    public bool IsRunning() => isRunning;
    public bool IsHoldingRunButton() => isHoldingRunButton;
    public Vector2 GetMoveInput() => moveInput;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool grounded = controller.isGrounded;

        // Snapshot sprint state the moment the player leaves the ground.
        if (prevGrounded && !grounded)
            wasRunningOnLaunch = isRunning;

        if (grounded)
            isRunning = isHoldingRunButton && moveInput != Vector2.zero;
        else
            // Airborne: preserve whatever sprint state was active at launch — releasing
            // Shift mid-air shouldn't kill the horizontal inertia until landing.
            isRunning = wasRunningOnLaunch;

        prevGrounded = grounded;
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
