using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float gravityPressure = -0.5f;

    public CharacterController controller;

    public EntityData entityData;
    public Vector3 playerVelocity;
    
    private PlayerRun playerRun;
    
    public bool isGrounded;

    public float gravity = gravityPressure;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float runSpeed;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerRun = GetComponent<PlayerRun>();
    }
    
    private void FixedUpdate()
    {
        walkSpeed = entityData.WalkSpeed;
        runSpeed = entityData.RunSpeed;
        isGrounded = controller.isGrounded;
    }

    public void Move(Vector2 input)
    {
        playerRun.moveInput = input;
        Vector3 moveDirection = transform.TransformDirection(new Vector3(input.x, 0f, input.y));
        float targetSpeed = playerRun.IsRunning() ? runSpeed : walkSpeed;
        controller.Move(targetSpeed * Time.deltaTime * moveDirection);

        if (controller.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = gravity * Time.deltaTime;
        else
            playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }
}
