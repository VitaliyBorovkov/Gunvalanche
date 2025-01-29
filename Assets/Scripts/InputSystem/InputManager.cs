using System.Collections.Generic;

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
    private InputAction reloadAction;
    private InputAction switchWeaponByScrollAction;
    private InputAction weaponSlot1Action;
    private InputAction weaponSlot2Action;
    private InputAction weaponSlot3Action;

    private PlayerMovement playerMovement;
    private PlayerLook playerLook;
    private PlayerJump playerJump;
    private PlayerRun playerRun;
    private PlayerShoot playerShoot;
    private PlayerReload playerReload;
    private PlayerSwitchWeapon playerSwitchWeapon;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        try
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerLook = GetComponent<PlayerLook>();
            playerJump = GetComponent<PlayerJump>();
            playerRun = GetComponent<PlayerRun>();
            playerShoot = GetComponent<PlayerShoot>();
            playerReload = GetComponent<PlayerReload>();
            playerSwitchWeapon = GetComponent<PlayerSwitchWeapon>();

            movementAction = playerInput.actions["Movement"];
            jumpAction = playerInput.actions["Jump"];
            lookAction = playerInput.actions["Look"];
            runAction = playerInput.actions["Run"];
            fireAction = playerInput.actions["Fire"];
            //aimAction = playerInput.actions["Aim"];
            reloadAction = playerInput.actions["Reload"];
            switchWeaponByScrollAction = playerInput.actions["SwitchWeaponByScroll"];
            weaponSlot1Action = playerInput.actions["WeaponSlot1"];
            weaponSlot2Action = playerInput.actions["WeaponSlot2"];
            weaponSlot3Action = playerInput.actions["WeaponSlot3"];
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError($"Input Action not found: {e.Message}");
        }
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
        reloadAction.Enable();
        switchWeaponByScrollAction.Enable();
        weaponSlot1Action.Enable();
        weaponSlot2Action.Enable();
        weaponSlot3Action.Enable();

        movementAction.performed += _ => playerMovement.Move(movementAction.ReadValue<Vector2>());
        jumpAction.performed += _ => playerJump.Jump();
        lookAction.performed += _ => playerLook.Look(lookAction.ReadValue<Vector2>());
        runAction.started += _ => playerRun.OnTryRunStart();
        runAction.canceled += _ => playerRun.OnTryRunEnd();
        //fireAction.started += _ => playerShoot.ShootGun();
        fireAction.started += _ => playerShoot.StartFiring();
        fireAction.canceled += _ => playerShoot.StopFiring();
        reloadAction.performed += _ => playerReload.Reload();
        switchWeaponByScrollAction.performed += playerSwitchWeapon.HandleScrollWeapon;
        weaponSlot1Action.performed += _ => playerSwitchWeapon.SwitchWeaponByIndex(0);
        weaponSlot2Action.performed += _ => playerSwitchWeapon.SwitchWeaponByIndex(1);
        weaponSlot3Action.performed += _ => playerSwitchWeapon.SwitchWeaponByIndex(2);
    }
    
    private void OnDisable()
    {
        movementAction.Disable();
        jumpAction.Disable();
        lookAction.Disable();
        fireAction.Disable();
        //aimAction.Disable();
        reloadAction.Disable();
        switchWeaponByScrollAction.Disable();
        weaponSlot1Action.Disable();
        weaponSlot2Action.Disable();
        weaponSlot3Action.Disable();

        movementAction.performed -= _ => playerMovement.Move(movementAction.ReadValue<Vector2>());
        jumpAction.performed -= _ => playerJump.Jump();
        lookAction.performed -= _ => playerLook.Look(lookAction.ReadValue<Vector2>());
        reloadAction.performed -= _ => playerReload.Reload();
        switchWeaponByScrollAction.performed -= playerSwitchWeapon.HandleScrollWeapon;
        weaponSlot1Action.performed -= _ => playerSwitchWeapon.SwitchWeaponByIndex(0);
        weaponSlot2Action.performed -= _ => playerSwitchWeapon.SwitchWeaponByIndex(1);
        weaponSlot3Action.performed -= _ => playerSwitchWeapon.SwitchWeaponByIndex(2);
    }
}