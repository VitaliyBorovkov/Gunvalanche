using System;

using UnityEngine;

public sealed class GameStateContext
{
    public InputManager InputManager;
    public HPAmmoVisibilityController HPAmmoVisibilityController;
    public UIVisibilityBase WeaponIconVisibilityController;

    public readonly InputHandler InputHandler;
    public readonly GameOverUI GameOverUI;
    public readonly PauseUI PauseUI;

    public Action RequestGameplayMap;
    public Action RequestUIMap;

    public GameStateContext(InputManager inputManager, InputHandler inputHandler, GameOverUI gameOverUI,
        PauseUI pauseUI, HPAmmoVisibilityController hpAmmoVisibilityController, UIVisibilityBase weaponIconVisibilityController)
    {
        InputManager = inputManager;
        InputHandler = inputHandler;
        GameOverUI = gameOverUI;
        PauseUI = pauseUI;
        HPAmmoVisibilityController = hpAmmoVisibilityController;
        WeaponIconVisibilityController = weaponIconVisibilityController;
    }

    public void SetCursor(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    public void SetInputManager(InputManager input)
    {
        InputManager = input;
    }

    public void SetHPAmmoVisibilityController(HPAmmoVisibilityController hpAmmoVisibilityController)
    {
        if (hpAmmoVisibilityController != null)
        {
            HPAmmoVisibilityController = hpAmmoVisibilityController;
        }
    }

    public void SetWeaponIconVisibilityController(UIVisibilityBase weaponIconVisibilityController)
    {
        if (weaponIconVisibilityController != null)
        {
            WeaponIconVisibilityController = weaponIconVisibilityController;
        }
    }

    public HPAmmoVisibilityController GetOrResolveHPAmmoVisibilityController()
    {
        if (HPAmmoVisibilityController != null)
        {
            return HPAmmoVisibilityController;
        }

        HPAmmoVisibilityController = GameObject.FindObjectOfType<HPAmmoVisibilityController>();
        return HPAmmoVisibilityController;
    }

    public UIVisibilityBase GetOrResolveWeaponIconVisibilityController()
    {
        if (WeaponIconVisibilityController != null)
        {
            return WeaponIconVisibilityController;
        }
        WeaponIconVisibilityController = GameObject.FindObjectOfType<UIVisibilityBase>();
        return WeaponIconVisibilityController;
    }
}
