using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPoint : MonoBehaviour
{
    [SerializeField] Vector3 speed;
    [SerializeField] GameObject? followObject;
    [SerializeField] CameraController cameraController;
    Vector3 velocity;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;
        playerController = GetComponent<PlayerController>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Follow object

        if (followObject != null)
        {
            if (!followObject.activeSelf)
                DropObject();

            else
                transform.position = followObject.transform.position;
        }

        if (cameraController.inTransition || !cameraController.allowTranslation)
            return;

        // Click object

        if (playerController.IsShooting())
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(playerController.GetMousePosition());

            if (Physics.Raycast(ray, out raycastHit, 500.0f))
            {
                if (raycastHit.transform != null && raycastHit.transform.gameObject.tag == "Tank")
                {
                    FollowObject(raycastHit.transform.gameObject);
                    cameraController.Transition(CameraController.View.Side, 0.8f);
                }

                else
                    DropObject();
            }
        }

        // Move position manually

        Rect rect = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
        rect.Inflate(-2.0f, -2.0f);

        if (rect.Contains(playerController.GetMousePosition()))
            return;
        
        velocity = (playerController.GetMousePosition() - rect.center).normalized;
        transform.position += velocity.Mul(speed) * Time.deltaTime;
        followObject = null;
    }

    /// <summary>
    /// Set the position of this FocusPoint, and if following a GameObject, drop it.
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        followObject = null;
        transform.position = position;
    }

    /// <summary>
    /// Set the GameObject which this FocusPoint should follow
    /// </summary>
    /// <returns> Whether or not the provided GameObject was able to be followed.</returns>
    public bool FollowObject(GameObject gameObject)
    {
        if (!gameObject.activeSelf)
            return false;

        followObject = gameObject;
        transform.position = gameObject.transform.position;
        return true;
    }

    /// <summary>
    /// Drop the Gameobject which this FocusPoint is following.
    /// </summary>
    /// <returns> Whether or not there was a GameObject to drop.</returns>
    public bool DropObject()
    {
        bool hasFollowObject = followObject != null;
        followObject = null;
        return hasFollowObject;
    }
}
