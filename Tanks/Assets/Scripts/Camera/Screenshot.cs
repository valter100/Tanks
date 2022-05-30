using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    [SerializeField] private string folderPath;

    private void Start()
    {
        folderPath = Application.dataPath;
        folderPath = folderPath.Remove(folderPath.LastIndexOf("Assets"));
        folderPath += "Screenshots/";
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
            {
                string fileName = folderPath + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png";
                Debug.Log("Screenshot taken: " + fileName);
                ScreenCapture.CaptureScreenshot(fileName);
                MessagesManager.AddDelayedMessage(2, "Screenshot taken:\n" + fileName).SetDuration(2f).SetSpeed(0.5f);
            }
        }
    }
}
