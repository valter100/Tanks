using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseMover : StandaloneInputModule
{
    Mouse mouse;

    Vector2 mousePos;

    [SerializeField] List<KeyCode> up;
    [SerializeField] List<KeyCode> left;
    [SerializeField] List<KeyCode> down;
    [SerializeField] List<KeyCode> right;
    [SerializeField] List<KeyCode> leftClickButtons;
    [SerializeField] float mouseSpeed;

    void Start()
    {
        DontDestroyOnLoad(this); //Don't include if mouse movement is specific to a single scene
        mouse = Mouse.current;
        mousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    { 
        // For every keybind we need to connect both of the joysticks, one for WASD and one for the Arrow keys.
        // The script checks both of the joysticks every update, which means that both players can steer the mouse
        // at the same time. This can be further customized with weaving in turn based limitations and the likes.

        foreach(KeyCode key in up)
        {
            if (Input.GetKey(key))
            {
                mousePos += new Vector2(0, 1) * mouseSpeed;
                break;
            }
        }        
        foreach(KeyCode key in left)
        {
            if (Input.GetKey(key))
            {
                mousePos += new Vector2(-1, 0) * mouseSpeed;
                break;
            }
        }        
        foreach(KeyCode key in down)
        {
            if (Input.GetKey(key))
            {
                mousePos += new Vector2(0, -1) * mouseSpeed;
                break;
            }
        }        
        foreach(KeyCode key in right)
        {
            if (Input.GetKey(key))
            {
                mousePos += new Vector2(1, 0) * mouseSpeed;
                break;
            }
        }
        // This method places the mouse cursor onto a specific screen coordinate. We use Mouse Pos here to make the cursor follow our inputs.
        mouse.WarpCursorPosition(mousePos); 

        // Method that simulates mouse clicks. NUM3 has to be hardcoded since Num dont have keycode with the old input system.
        if (Input.GetKeyDown("[3]"))
        {
            ClickAt(mousePos, true);
        }
        else if (Input.GetKeyUp("[3]"))
        {
            ClickAt(mousePos, false);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ClickAt(mousePos, true);
        }
        else if (Input.GetKeyUp(KeyCode.M))
        {
            ClickAt(mousePos, false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ClickAt(Vector2 pos, bool pressed)
    {
        Input.simulateMouseWithTouches = true;
        var pointerData = GetTouchPointerEventData(new Touch()
        {
            position = mousePos,
        }, out bool b, out bool bb);

        ProcessTouchPress(pointerData, pressed, !pressed);
    }
}
