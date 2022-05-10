using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform rotatePoint;
    [SerializeField] PlayerController controller;
    Camera cam;
    [SerializeField] Transform firePoint;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void Aim()
    {
        Vector2 cannonScreenPos = cam.WorldToScreenPoint(rotatePoint.transform.position);
        Vector2 lookVector = controller.GetMousePosition() - cannonScreenPos;

        rotatePoint.transform.rotation = Quaternion.LookRotation(lookVector);
        rotatePoint.transform.Rotate(90, 0, 0);

        //float rotationZ = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg - 90;

        //if (rotationZ < 0)
        //    rotationZ = 0;
        //else if (rotationZ > 180)
        //    rotationZ = 180;

        //rotatePoint.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    public Transform GetFirePoint()
    {
        return firePoint;
    }
}
