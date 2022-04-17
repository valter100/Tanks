using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public enum View { Side, Focus, FirstPerson };

    [SerializeField] View view;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float targetFOV;
    [SerializeField] public FocusPoint focusPoint;
    [SerializeField] GameObject? followObject;
    [SerializeField] bool transitioning;

    Dictionary<View, ViewSettings> viewSettings;
    PlayerController playerController;
    Camera mainCamera;

    public bool allowTranslation => viewSettings[view].allowTranslation;
    public bool inTransition => transitioning;

    private class ViewSettings
    {
        public Vector3 position, rotation;
        public float FOV, minFOV, maxFOV;
        public bool allowTranslation, allowZoom;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        playerController = GetComponent<PlayerController>();
        transform.localScale = Vector3.one;
        transitioning = false;

        ViewSettings sideView = new ViewSettings();
        sideView.position = new Vector3(0.0f, 7.0f, -50.0f);
        sideView.rotation = Vector3.zero;
        sideView.FOV = 25.0f;
        sideView.minFOV = 15.0f;
        sideView.maxFOV = 50.0f;
        sideView.allowTranslation = true;
        sideView.allowZoom = true;

        ViewSettings focusView = new ViewSettings();
        focusView.position = new Vector3(0.0f, 7.0f, -100.0f);
        focusView.rotation = Vector3.zero;
        focusView.FOV = 5.0f;
        focusView.minFOV = 3.0f;
        focusView.maxFOV = 7.0f;
        focusView.allowTranslation = false;
        focusView.allowZoom = true;

        ViewSettings firstPersonView = new ViewSettings();
        firstPersonView.FOV = 60.0f;
        firstPersonView.allowTranslation = false;
        firstPersonView.allowZoom = false;

        viewSettings = new Dictionary<View, ViewSettings>(){
            { View.Side, sideView },
            { View.Focus, focusView },
            { View.FirstPerson, firstPersonView }};

        // Transition instantly to side view
        Transition(View.Side, null);
    }

    // Update is called once per frame
    void Update()
    {
        if (transitioning)
            return;

        if (followObject != null && !followObject.activeSelf)
            followObject = null;

        // Target FOV

        if (viewSettings[view].allowZoom)
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;

            if (scrollValue != 0.0f)
            {
                targetFOV -= scrollValue * 0.005f * (viewSettings[view].maxFOV - viewSettings[view].minFOV);
                targetFOV = Mathf.Clamp(targetFOV, viewSettings[view].minFOV, viewSettings[view].maxFOV);
            }
        }

        // Target position and rotation

        if (view == View.Side || view == View.Focus)
        {
            targetPosition = new Vector3(focusPoint.transform.position.x, focusPoint.transform.position.y, transform.position.z);
        }

        else if (view == View.FirstPerson)
        {
            if (followObject != null)
                transform.position = followObject.transform.position;
            transform.rotation = Quaternion.Euler(focusPoint.transform.position - transform.position);
        }

        // Approach target values

        transform.position = ApproachTargetValue(transform.position, targetPosition, 5.0f);
        mainCamera.fieldOfView = ApproachTargetValue(mainCamera.fieldOfView, targetFOV, 10.0f);
    }

    /// <summary>
    /// Starts a coroutine transition to the provided View. This call will be ignored if another transition is already happening.
    /// </summary>
    /// <param name="view">View to transition to.</param>
    /// <param name="duration">The amount of seconds the transition takes. Enter null for instant transition.</param>
    public void Transition(View view, float? duration)
    {
        if (transitioning)
            return;

        this.view = view;
        transitioning = true;

        Quaternion targetEuler;
        targetFOV = viewSettings[view].FOV;

        if (view == View.FirstPerson)
        {
            targetPosition = followObject != null ? followObject.transform.position : transform.position;
            targetEuler = Quaternion.LookRotation(focusPoint.transform.position - targetPosition);
        }

        else
        {
            followObject = null;
            targetPosition = new Vector3(focusPoint.transform.position.x, focusPoint.transform.position.y, viewSettings[view].position.z);
            targetEuler = Quaternion.Euler(viewSettings[view].rotation);
        }

        if (duration != null)
            StartCoroutine(Transition(duration.Value, targetPosition, targetEuler, targetFOV));

        else
        {
            transform.position = targetPosition;
            transform.rotation = targetEuler;
            mainCamera.fieldOfView = targetFOV;
            transitioning = false;
        }  
    }

    /// <summary>
    /// Set the GameObject which this Camera's position should follow
    /// </summary>
    public void FollowObject(GameObject gameObject)
    {
        if (!gameObject.activeSelf)
            return;

        followObject = gameObject;
        transform.position = gameObject.transform.position;
    }

    private Vector3 ApproachTargetValue(Vector3 vector, Vector3 target, float speed)
    {
        Vector3 delta = target - vector;
        Vector3 change = delta * Time.deltaTime * speed;

        if (change.magnitude >= delta.magnitude || delta.magnitude < 0.001f)
            return target;

        return vector + change;
    }

    private float ApproachTargetValue(float value, float target, float speed)
    {
        float delta = target - value;
        float change = delta * Time.deltaTime * speed;

        if (Math.Abs(change) >= Math.Abs(delta) || Math.Abs(delta) < 0.001f)
            return target;

        return value + change;
    }

    private IEnumerator Transition(float duration, Vector3 targetPosition, Quaternion targetEuler, float targetFOV)
    {
        float lerpValue;
        float elapsedTime = 0.0f;

        Vector3 startPosition = transform.position;
        Quaternion startEuler = transform.rotation;
        float startFOV = mainCamera.fieldOfView;

        while (elapsedTime < duration)
        {
            lerpValue = (elapsedTime / duration).LerpValueSigmoidCurve();

            transform.position = Vector3.Lerp(startPosition, targetPosition, lerpValue);
            transform.rotation = Quaternion.Lerp(startEuler, targetEuler, lerpValue);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, lerpValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetEuler;
        mainCamera.fieldOfView = targetFOV;
        transitioning = false;
    }
}
