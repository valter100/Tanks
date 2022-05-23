using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerControls playerControls;
    Vector3 move;

    void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        move = playerControls.Tank.Move.ReadValue<Vector3>();
    }

    // Gets

    public Vector3 GetMovement()
    {
        return move;
    }

    public Vector2 GetMousePosition()
    {
        return playerControls.Tank.Aim.ReadValue<Vector2>();
    }

    // Triggers

    public bool Trigger_Fire()
    {
        return playerControls.Tank.Shoot.triggered;
    }

    public bool Trigger_Focus()
    {
        return playerControls.Tank.FocusCamera.triggered;
    }

    public bool Trigger_AutoFocus()
    {
        return playerControls.Tank.AutoFocusCamera.triggered;
    }

    public bool Trigger_PreviousItem()
    {
        return playerControls.Tank.PreviousItem.triggered;
    }

    public bool Trigger_NextItem()
    {
        return playerControls.Tank.NextItem.triggered;
    }

    public bool Trigger_Inventory()
    {
        return playerControls.Tank.Inventory.triggered;
    }

    public bool Trigger_Back()
    {
        return playerControls.Tank.Back.triggered;
    }

}
