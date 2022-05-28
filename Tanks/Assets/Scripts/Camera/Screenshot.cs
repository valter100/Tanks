using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
            {
                ScreenCapture.CaptureScreenshot(Application.dataPath.Replace("Assets", "Screenshots/") + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png");
            }
        }
    }
}
