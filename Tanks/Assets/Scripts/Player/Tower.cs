using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] Transform rotatePoint;
    [SerializeField] PlayerController controller;

    [SerializeField] float leftRotation;
    [SerializeField] float rightRotation;

    Camera cam;

    bool facingRight;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    //private void Update()
    //{
    //    Aim();
    //}

    public void Aim()
    {
        Vector2 mousePosition = controller.GetMousePosition();

        float xValue = cam.WorldToScreenPoint(rotatePoint.transform.position).x - mousePosition.x;

        if (xValue < 0 && !facingRight)
        {
            rotatePoint.transform.rotation = Quaternion.Euler(0, leftRotation, 0);
            facingRight = true;
        }
        else if (xValue > 0 && facingRight)
        {
            rotatePoint.transform.rotation = Quaternion.Euler(0, rightRotation, 0);
            facingRight = false;
        }
    }
}
