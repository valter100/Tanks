using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPoint : MonoBehaviour
{
#pragma warning disable CS8632

    [Header("Adjustable")]
    [SerializeField] Vector3 speed;
    [SerializeField] Vector3 defaultOffset;

    [Header("Information")]
    [SerializeField] Vector3 offset;
    [SerializeField] GameObject? followObject;
    [SerializeField] CameraController cameraController;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameManager gameManager;
    Vector3 velocity;

    public GameObject GetObject() => followObject;
    public Vector3 GetDefaultOffset() => defaultOffset;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;
        playerController = GetComponent<PlayerController>();
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
                GoToObject();
        }

        if (cameraController.inTransition || !cameraController.allowTranslation)
            return;

        // Click object

        if (playerController.Trigger_Focus())
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(playerController.GetMousePosition());

            if (Physics.Raycast(ray, out raycastHit, 500.0f))
            {
                if (raycastHit.transform != null && raycastHit.transform.gameObject.tag == "Tank")
                {
                    FollowObject(raycastHit.transform.gameObject);
                    cameraController.Transition(CameraController.View.Side, 0.6f);
                    return;
                }

                else
                    DropObject();
            }
        }

        else if (playerController.Trigger_AutoFocus())
        {
            FollowObject(gameManager.CurrentPlayer.Tank.gameObject);
            cameraController.Transition(CameraController.View.Side, 0.5f);
            return;
        }

        // Move position manually

        Rect rect = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
        rect.Inflate(-2.0f, -2.0f);

        if (rect.Contains(playerController.GetMousePosition()))
            return;
        
        velocity = (playerController.GetMousePosition() - rect.center).normalized;
        transform.position += velocity.Mul(speed) * Time.deltaTime;
        followObject = null;
        offset = Vector3.zero;
    }

    /// <summary>
    /// Set the position of this FocusPoint, and if following a GameObject, drop it.
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        followObject = null;
        transform.position = position;
        offset = Vector3.zero;
    }

    /// <summary>
    /// Set the GameObject which this FocusPoint should follow
    /// </summary>
    /// <returns> Whether or not the provided GameObject was able to be followed.</returns>
    public bool FollowObject(GameObject gameObject)
    {
        if (!gameObject.activeSelf)
            return false;

        FocusPointOffset focusPointOffset = gameObject.GetComponent<FocusPointOffset>();
        offset = focusPointOffset != null ? focusPointOffset.offset : Vector3.zero;
        followObject = gameObject;
        GoToObject();
        return true;
    }

    /// <summary>
    /// Set the GameObject which this FocusPoint should follow, after a delay.
    /// </summary>
    public IEnumerator Coroutine_DelayedFollowObject(GameObject gameObject, float delay)
    {
        if (!gameObject.activeSelf)
            yield break;

        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        FocusPointOffset focusPointOffset = gameObject.GetComponent<FocusPointOffset>();
        offset = focusPointOffset != null ? focusPointOffset.offset : Vector3.zero;
        followObject = gameObject;
        GoToObject();
        yield return 0;
    }

    /// <summary>
    /// Drop the Gameobject which this FocusPoint is following.
    /// </summary>
    /// <returns> Whether or not there was a GameObject to drop.</returns>
    public bool DropObject()
    {
        bool hasFollowObject = followObject != null;
        followObject = null;
        offset = Vector3.zero;
        return hasFollowObject;
    }

    private void GoToObject()
    {
        transform.position = followObject.transform.position + offset;
    }
}
