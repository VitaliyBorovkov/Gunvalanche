using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private bool isJumping;

    public float jumpHeight = 3f;
    public float jumpPower = -3.0f;

    public bool IsJumping() => isJumping;
    public void Initialize(PlayerMovement movement)
    {
        playerMovement = movement;
    }

    public void Jump()
    {
        if (playerMovement.IsGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(playerMovement.Gravity) * jumpHeight);
            playerMovement.SetVelocityY(jumpVelocity);
        }
    }
}
