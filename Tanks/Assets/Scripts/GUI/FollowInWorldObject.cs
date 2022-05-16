using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowInWorldObject : MonoBehaviour
{
    [SerializeField] public Transform followObject;
    [SerializeField] public Vector3 offset;
    private Camera mainCamera;
    private Vector3 position;

    private void Start()
    {
        mainCamera = Camera.main;
        Update();
    }

    private void Update()
    {
        if (followObject == null)
            return;

        position = mainCamera.WorldToScreenPoint(followObject.position + offset);

        if (transform.position != position)
            transform.position = position;
    }
}
