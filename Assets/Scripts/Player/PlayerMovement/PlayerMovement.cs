using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const string LOG_PREFIX = "PlayerMovement";

    private const float gravityPressure = -0.5f;

    [SerializeField] private EntityData entityData;

    private CharacterController controller;
    private PlayerRun playerRun;
    private Vector3 playerVelocity;
    private bool isGrounded;
    [SerializeField] private float gravity = gravityPressure;
    private float walkSpeed;
    private float runSpeed;

    public bool IsGrounded => isGrounded;
    public float Gravity => gravity;

    public void SetVelocityY(float y) => playerVelocity.y = y;
    public void SetWalkSpeed(float speed) => walkSpeed = speed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerRun = GetComponent<PlayerRun>();

        if (controller == null || playerRun == null)
        {
            Debug.Log($"{LOG_PREFIX}: CharacterController or PlayerRun not found");
        }
    }

    private void Start()
    {
        walkSpeed = entityData.WalkSpeed;
        runSpeed = entityData.RunSpeed;
    }

    private void FixedUpdate()
    {
        isGrounded = controller.isGrounded;
    }

    public void Move(Vector2 input)
    {
        playerRun.SetMoveInput(input);
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
