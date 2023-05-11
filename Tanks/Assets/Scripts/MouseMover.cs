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
        mouse = Mouse.current;
        mousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    { 
        foreach(KeyCode key in up)
        {
            if (Input.GetKey(key))
            {
                mousePos += new Vector2(0, 1) * mouseSpeed;
            }
        }        
        foreach(KeyCode key in left)
        {
            if (Input.GetKey(key))
            {
                mousePos += new Vector2(-1, 0) * mouseSpeed;
            }
        }        
        foreach(KeyCode key in down)
        {
            if (Input.GetKey(key))
            {
                mousePos += new Vector2(0, -1) * mouseSpeed;
            }
        }        
        foreach(KeyCode key in right)
        {
            if (Input.GetKey(key))
            {
                mousePos += new Vector2(1, 0) * mouseSpeed;
            }
        }
       
        mouse.WarpCursorPosition(mousePos);

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

        //foreach (KeyCode key in leftClickButtons)
        //{
        //    try
        //    {
        //        if (Input.GetKeyDown("[" + key  + "]"))
        //        {
        //            ClickAt(mousePos, true);
        //        }
        //        else if(Input.GetKeyUp("[" + key + "]"))
        //        {
        //            ClickAt(mousePos, false);
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        if (Input.GetKeyDown(key))
        //        {
        //            ClickAt(mousePos, true);
        //        }
        //        else if (Input.GetKeyUp(key))
        //        {
        //            ClickAt(mousePos, false);
        //        }
        //    }
        //}

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

        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed!");
        }
    }
}
