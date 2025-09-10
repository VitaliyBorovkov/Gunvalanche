using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private const string GameplayMapName = "GameplayActionMap";
    private const string UIMapName = "UIActionMap";

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
    public InputAction Pause => playerInput.actions["Pause"];

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

    public void SwitchToGameplayActionMap()
    {
        TrySwitchActionMap(GameplayMapName);
    }

    public void SwitchToUIActionMap()
    {
        TrySwitchActionMap(UIMapName);
    }

    private void TrySwitchActionMap(string mapName)
    {
        if (playerInput == null)
        {
            return;
        }

        if (playerInput.actions == null)
        {
            Debug.LogWarning($"InputManager: PlayerInput.actions is null, cannot switch to '{mapName}'.");
            return;
        }

        var map = playerInput.actions.FindActionMap(mapName, throwIfNotFound: false);
        if (map == null)
        {
            Debug.LogWarning($"InputManager: action map '{mapName}' not found in actions '{playerInput.actions.name}'. Available maps:");
            foreach (var m in playerInput.actions.actionMaps)
                Debug.Log($"  - {m.name}");
            return;
        }

        playerInput.SwitchCurrentActionMap(mapName);
        //Debug.Log($"InputManager: Switched to action map '{mapName}'.");
    }
}