using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
#pragma warning disable CS8632

    public enum View { Side, Focus, FirstPerson };

    [Header("References")]
    [SerializeField] private Transform parentTransform;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] public FocusPoint focusPoint;
    [SerializeField] private GameObject? followObject;

    [Header("Values")]
    [SerializeField] private View view;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Quaternion targetRotation;
    [SerializeField] private float targetFOV;
    [SerializeField] private bool transitioning;
    [SerializeField] private Vector3 translationalApproachSpeed;
    [SerializeField] private float FOVApproachSpeed;

    private Dictionary<View, ViewSettings> viewSettings;
    private Camera mainCamera;
    private float maxDistanceToTarget;

    public bool allowTranslation => viewSettings[view].allowTranslation;
    public bool inTransition => transitioning;

    public static CameraController Instance { get; private set; }

    private class ViewSettings
    {
        public Vector3 position, rotation;
        public float FOV, minFOV, maxFOV;
        public bool allowTranslation, allowZoom;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        Instance = this;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        transitioning = false;
        maxDistanceToTarget = Screen.currentResolution.height * 0.25f;

        ViewSettings sideView = new ViewSettings();
        sideView.position = new Vector3(0, 17, -25);
        sideView.rotation = new Vector3(12, 0, 0);
        sideView.minFOV = 20;
        sideView.FOV = 45;
        sideView.maxFOV = 80;
        sideView.allowTranslation = true;
        sideView.allowZoom = true;

        ViewSettings focusView = new ViewSettings();
        focusView.position = new Vector3(0, 7, -15);
        focusView.rotation = Vector3.zero;
        focusView.FOV = 60;
        focusView.allowTranslation = false;
        focusView.allowZoom = false;

        ViewSettings firstPersonView = new ViewSettings();
        firstPersonView.FOV = 60f;
        firstPersonView.allowTranslation = false;
        firstPersonView.allowZoom = false;

        viewSettings = new Dictionary<View, ViewSettings>(){
            { View.Side, sideView },
            { View.Focus, focusView },
            { View.FirstPerson, firstPersonView }};

        // Transition instantly to side view
        Transition(View.Side, null);
        mainCamera.fieldOfView = targetFOV = viewSettings[view].maxFOV;
    }

    private void Update()
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
                targetFOV -= scrollValue * 0.004f * (viewSettings[view].maxFOV - viewSettings[view].minFOV);
                targetFOV = Mathf.Clamp(targetFOV, viewSettings[view].minFOV, viewSettings[view].maxFOV);
            }
        }

        // Target position and rotation

        if (view == View.Side || view == View.Focus)
        {
            targetPosition = new Vector3(focusPoint.transform.position.x, focusPoint.transform.position.y, parentTransform.position.z);
        }

        else if (view == View.FirstPerson)
        {
            if (followObject != null)
                parentTransform.position = targetPosition = followObject.transform.position;
            parentTransform.LookAt(focusPoint.transform.position);
        }

        // Approach target values

        if (parentTransform.position != targetPosition)
            ApproachTargetPosition();

        if (mainCamera.fieldOfView != targetFOV)
            ApproachTargetFOV();

        // Apply trauma (camera shake)

        transform.localPosition = new Vector3(cameraShake.Offset.x, cameraShake.Offset.y, 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, cameraShake.Rotation);
    }

    private void ApproachTargetPosition()
    {
        Vector3 delta, change;

        /*float percentageOfMaxDistance = Vector2.Distance(
            mainCamera.WorldToScreenPoint(focusPoint.transform.position).ToV2(),
            new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) * 0.5f)
            / maxDistanceToTarget;

        if (percentageOfMaxDistance > 1.0f)
        {
            delta = targetPosition - transform.position;
            transform.position += delta * (percentageOfMaxDistance - 1.0f);
            return;
        }*/

        delta = targetPosition - parentTransform.position;
        change = delta.Mul(translationalApproachSpeed) * Time.deltaTime;

        if (change.magnitude >= delta.magnitude || delta.magnitude < 0.001f)
            parentTransform.position = targetPosition;
        else
            parentTransform.position += change;
    }

    private void ApproachTargetFOV()
    {
        float delta = targetFOV - mainCamera.fieldOfView;
        float change = delta * Time.deltaTime * FOVApproachSpeed;

        if (Math.Abs(change) >= Math.Abs(delta) || Math.Abs(delta) < 0.001f)
            mainCamera.fieldOfView = targetFOV;
        else
            mainCamera.fieldOfView += change;
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

        bool lookAtFocusPoint = this.view == View.FirstPerson && view != View.FirstPerson;
        this.view = view;

        // Determine targets

        targetFOV = viewSettings[view].FOV;

        if (view == View.FirstPerson)
        {
            targetPosition = followObject != null ? followObject.transform.position : parentTransform.position;

            Quaternion rotation = parentTransform.rotation;
            parentTransform.LookAt(focusPoint.transform.position);
            targetRotation = parentTransform.rotation;
            parentTransform.rotation = rotation;
        }

        else
        {
            followObject = null;
            targetPosition = new Vector3(focusPoint.transform.position.x, focusPoint.transform.position.y, viewSettings[view].position.z);
            targetRotation = Quaternion.Euler(viewSettings[view].rotation);
        }

        // Transition

        if (duration != null)
            StartCoroutine(Coroutine_Transition(duration.Value, lookAtFocusPoint));
        else
            InstantTransition();
    }

    /// <summary>
    /// Will after a set amount of time call Transition().
    /// </summary>
    /// <param name="view">View to transition to.</param>
    /// <param name="duration">The amount of seconds the transition takes. Enter null for instant transition.</param>
    public IEnumerator Coroutine_DelayedTransition(View view, float? duration, float delay)
    {
        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        Transition(view, duration);
        yield return 0;
    }

    /// <summary>
    /// Starts a sequence of transitions.
    /// </summary>
    public IEnumerator Coroutine_KillCamSequence(Projectile.PrecomputedResult result, GameObject projectile)
    {
        /*
        focusPoint.SetPosition(result.raycastHit.point);
        FollowObject(projectile);
        Transition(View.FirstPerson, null);

        float duration = result.timeBeforeHit;
        float elapsedTime = 0.0f;

        while (elapsedTime / duration < 0.7f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        focusPoint.FollowObject(projectile);
        transform.position = targetPosition = result.tank.transform.position + new Vector3(0, 1, 0);
        followObject = null;

        while (elapsedTime / duration < 0.9f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        focusPoint.SetPosition(result.tank.transform.position);
        Transition(View.Focus, 0.6f);
        yield return 0; 
        */

        focusPoint.SetPosition(result.raycastHit.point);
        FollowObject(projectile);
        Transition(View.FirstPerson, null);

        float duration = result.timeBeforeHit;
        float elapsedTime = 0.0f;

        while (elapsedTime / duration < 0.95f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        focusPoint.SetPosition(result.tank.transform.position);
        Transition(View.Focus, 0.2f);
        yield return 0;
    }

    private IEnumerator Coroutine_Transition(float duration, bool lookAtFocusPoint)
    {
        transitioning = true;
        float lerpValue;
        float elapsedTime = 0.0f;

        Vector3 startPosition = parentTransform.position;
        Quaternion startRotation = parentTransform.rotation;
        float startFOV = mainCamera.fieldOfView;

        while (elapsedTime < duration)
        {
            lerpValue = (elapsedTime / duration).LerpValueSmoothstep();

            parentTransform.position = Vector3.Lerp(startPosition, targetPosition, lerpValue);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, lerpValue);

            if (lookAtFocusPoint)
                parentTransform.LookAt(focusPoint.transform.position);
            else
                parentTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, lerpValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        InstantTransition();
        transitioning = false;
    }

    private void InstantTransition()
    {
        parentTransform.position = targetPosition;
        parentTransform.rotation = targetRotation;
        mainCamera.fieldOfView = targetFOV;
    }

    /// <summary>
    /// Set the GameObject which this Camera's position should follow
    /// </summary>
    public void FollowObject(GameObject gameObject)
    {
        if (!gameObject.activeSelf)
            return;

        followObject = gameObject;
        parentTransform.position = gameObject.transform.position;
    }

    public void Restart()
    {
        StopAllCoroutines();
        Start();
    }

}
