using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputManager))]
public class InputHandler : MonoBehaviour
{
    private InputManager input;

    private PlayerMovement playerMovement;
    private PlayerLook playerLook;
    private PlayerJump playerJump;
    private PlayerRun playerRun;
    private PlayerShoot playerShoot;
    private PlayerReload playerReload;
    private PlayerSwitchWeapon playerSwitchWeapon;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool isEnabled = true;

    public bool IsEnabled => isEnabled;

    private void Awake()
    {
        input = GetComponent<InputManager>();

        playerMovement = GetComponent<PlayerMovement>();
        playerLook = GetComponent<PlayerLook>();
        playerJump = GetComponent<PlayerJump>();
        playerRun = GetComponent<PlayerRun>();
        playerShoot = GetComponent<PlayerShoot>();
        playerReload = GetComponent<PlayerReload>();
        playerSwitchWeapon = GetComponent<PlayerSwitchWeapon>();

        if (playerJump != null && playerMovement != null)
        {
            playerJump.Initialize(playerMovement);
        }
        else
        {
            Debug.LogWarning("InputHandler: PlayerJump or PlayerMovement is missing on Player.");
        }
    }

    private void OnEnable()
    {
        if (isEnabled)
        {
            input.SetActionsEnabled(true);
        }

        input.Movement.performed += OnMovePerformed;
        input.Movement.canceled += OnMoveCanceled;

        input.Look.performed += OnLookPerformed;
        input.Look.canceled += OnLookCanceled;

        input.Jump.performed += OnJumpPerformed;

        input.Run.started += OnRunStarted;
        input.Run.canceled += OnRunCanceled;

        input.Fire.started += OnFireStarted;
        input.Fire.canceled += OnFireCanceled;

        input.Reload.performed += OnReloadPerformed;

        input.SwitchWeaponByScroll.performed += OnSwitchScrollPerformed;
        input.WeaponSlot1.performed += OnWeaponSlot1Performed;
        input.WeaponSlot2.performed += OnWeaponSlot2Performed;
        input.WeaponSlot3.performed += OnWeaponSlot3Performed;
    }

    private void OnDisable()
    {
        input.Movement.performed -= OnMovePerformed;
        input.Movement.canceled -= OnMoveCanceled;

        input.Look.performed -= OnLookPerformed;
        input.Look.canceled -= OnLookCanceled;

        input.Jump.performed -= OnJumpPerformed;

        input.Run.started -= OnRunStarted;
        input.Run.canceled -= OnRunCanceled;

        input.Fire.started -= OnFireStarted;
        input.Fire.canceled -= OnFireCanceled;

        input.Reload.performed -= OnReloadPerformed;

        input.SwitchWeaponByScroll.performed -= OnSwitchScrollPerformed;
        input.WeaponSlot1.performed -= OnWeaponSlot1Performed;
        input.WeaponSlot2.performed -= OnWeaponSlot2Performed;
        input.WeaponSlot3.performed -= OnWeaponSlot3Performed;

        input.SetActionsEnabled(false);

        moveInput = Vector2.zero;
        lookInput = Vector2.zero;
    }

    private void Update()
    {
        if (!isEnabled)
        {
            return;
        }

        if (playerMovement != null)
        {
            playerMovement.Move(moveInput);
            playerRun?.SetMoveInput(moveInput);
        }
    }

    private void LateUpdate()
    {
        if (!isEnabled)
        {
            return;
        }

        if (playerLook != null && playerLook.playerCamera != null)
        {
            playerLook.Look(lookInput);
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        moveInput = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        lookInput = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerJump?.Jump();
    }

    private void OnRunStarted(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerRun?.OnTryRunStart();
    }

    private void OnRunCanceled(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerRun?.OnTryRunEnd();
    }

    private void OnFireStarted(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerShoot?.StartFiring();
    }

    private void OnFireCanceled(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerShoot?.StopFiring();
    }

    private void OnReloadPerformed(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerReload?.Reload();
    }

    private void OnSwitchScrollPerformed(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerSwitchWeapon?.HandleScrollWeapon(ctx);
    }

    private void OnWeaponSlot1Performed(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerSwitchWeapon?.SwitchWeaponByIndex(0);
    }

    private void OnWeaponSlot2Performed(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerSwitchWeapon?.SwitchWeaponByIndex(1);
    }

    private void OnWeaponSlot3Performed(InputAction.CallbackContext ctx)
    {
        if (!isEnabled)
        {
            return;
        }

        playerSwitchWeapon?.SwitchWeaponByIndex(2);
    }

    public void SetEnabled(bool value)
    {
        if (isEnabled == value)
        {
            return;
        }

        isEnabled = value;
        if (!isEnabled)
        {
            playerShoot?.StopFiring();
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
        }

        if (input != null)
        {
            input.SetActionsEnabled(isEnabled);
        }
        Debug.Log($"InputHandler: Input is now {(isEnabled ? "enabled" : "disabled")}");
    }
}