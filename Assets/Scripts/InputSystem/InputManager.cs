using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;

    public InputAction Movement => playerInput.actions["Movement"];
    public InputAction Jump => playerInput.actions["Jump"];
    public InputAction Look => playerInput.actions["Look"];
    public InputAction Run => playerInput.actions["Run"];
    public InputAction Fire => playerInput.actions["Fire"];
    public InputAction Reload => playerInput.actions["Reload"];
    public InputAction SwitchWeaponByScroll => playerInput.actions["SwitchWeaponByScroll"];
    public InputAction WeaponSlot1 => playerInput.actions["WeaponSlot1"];
    public InputAction WeaponSlot2 => playerInput.actions["WeaponSlot2"];
    public InputAction WeaponSlot3 => playerInput.actions["WeaponSlot3"];

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void SetActionsEnabled(bool enable)
    {
        if (enable)
        {
            Movement.Enable();
            Jump.Enable();
            Look.Enable();
            Run.Enable();
            Fire.Enable();
            Reload.Enable();
            SwitchWeaponByScroll.Enable();
            WeaponSlot1.Enable();
            WeaponSlot2.Enable();
            WeaponSlot3.Enable();
        }
        else
        {
            Movement.Disable();
            Jump.Disable();
            Look.Disable();
            Run.Disable();
            Fire.Disable();
            Reload.Disable();
            SwitchWeaponByScroll.Disable();
            WeaponSlot1.Disable();
            WeaponSlot2.Disable();
            WeaponSlot3.Disable();
        }
    }
}