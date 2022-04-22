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

    private void Start()
    {
        
    }

    void Update()
    {
        move = playerControls.Tank.Move.ReadValue<Vector3>();
        //Debug.Log(move);
    }

    public Vector3 GetMovement()
    {
        return move;
    }

    public bool IsShooting()
    {
        return playerControls.Tank.Shoot.triggered;
    }

    public bool IsFocusing()
    {
        return playerControls.Tank.FocusCamera.triggered;
    }

    public Vector2 GetMousePosition()
    {
        return playerControls.Tank.Aim.ReadValue<Vector2>();
    }

    public int GetNewWeapon()
    {
        if (playerControls.Tank.NextProjectile.triggered)
            return 1;
        else if (playerControls.Tank.PreviousProjectile.triggered)
            return -1;

        else return 0;
    }
}
