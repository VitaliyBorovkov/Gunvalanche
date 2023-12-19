using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))] 
public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction movementAction; 
    private InputAction jumpAction;
    private InputAction lookAction;
    private InputAction runAction;
    private InputAction fireAction;
    //private InputAction aimAction;
    //private InputAction reloadAction;

    private PlayerMovement playerMovement;
    private PlayerLook playerLook;
    private PlayerJump playerJump;
    private PlayerRun playerRun;
    private PlayerShoot playerShoot;
    //private PlayerReload playerReload;



    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerLook = GetComponent<PlayerLook>();
        playerJump = GetComponent<PlayerJump>();
        playerRun = GetComponent<PlayerRun>();
        playerShoot = GetComponent<PlayerShoot>();
        //playerReload = GetComponent<PlayerReload>();



        movementAction = playerInput.actions["Movement"];
        jumpAction = playerInput.actions["Jump"];
        lookAction = playerInput.actions["Look"];
        runAction = playerInput.actions["Run"];
        fireAction = playerInput.actions["Fire"];
        //aimAction = playerInput.actions["Aim"];
        //reloadAction = playerInput.actions["Reload"];

    }

    private void FixedUpdate()
    {
        Vector2 moveInput = movementAction.ReadValue<Vector2>();
        playerMovement.Move(moveInput);
        playerJump.Initialize(playerMovement);
        playerRun.SetMoveInput(moveInput);
    }

    private void LateUpdate()
    {
        playerLook.Look(lookAction.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        movementAction.Enable();
        jumpAction.Enable();
        lookAction.Enable();
        runAction.Enable();
        fireAction.Enable();
        //aimAction.Enable();
        //reloadAction.Enable();


        movementAction.performed += _ => playerMovement.Move(movementAction.ReadValue<Vector2>());
        jumpAction.performed += _ => playerJump.Jump();
        lookAction.performed += _ => playerLook.Look(lookAction.ReadValue<Vector2>());
        runAction.started += _ => playerRun.OnTryRunStart();
        runAction.canceled += _ => playerRun.OnTryRunEnd();
        //fireAction.started += _ => playerShoot.ShootGun();
        fireAction.started += _ => playerShoot.StartFiring();
        fireAction.canceled -= _ => playerShoot.StopFiring();
        //reloadAction.performed += _ => playerReload.Reload();

    }
    
    private void OnDisable()
    {
        movementAction.Disable();
        jumpAction.Disable();
        lookAction.Disable();
        fireAction.Disable();
        //aimAction.Disable();
        //reloadAction.Disable();


        movementAction.performed -= _ => playerMovement.Move(movementAction.ReadValue<Vector2>());
        jumpAction.performed -= _ => playerJump.Jump();
        lookAction.performed -= _ => playerLook.Look(lookAction.ReadValue<Vector2>());
       
        //fireAction.canceled -= _ => playerShoot.ShootGun();
        
        //reloadAction.performed -= _ => playerReload.Reload();

    }
}
