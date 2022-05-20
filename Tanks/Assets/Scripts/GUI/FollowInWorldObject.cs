using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowInWorldObject : MonoBehaviour
{
    [SerializeField] public Transform followObject;
    [SerializeField] public Vector3 offset;

    private Canvas canvas;
    private RectTransform rect;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rect = GetComponent<RectTransform>();
        Update();
    }

    private void Update()
    {
        if (followObject == null)
            return;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(followObject.position + offset);

            if (transform.position != screenPoint)
                transform.position = screenPoint;
        }

        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector2 viewpointPoint = mainCamera.WorldToViewportPoint(followObject.position + offset);

            if (rect.anchorMin != viewpointPoint)
            {
                rect.anchorMin = viewpointPoint;
                rect.anchorMax = viewpointPoint;
            }
        }
    }

}
