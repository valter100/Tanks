using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerTank : Tank
{
    private static PlayerController playerController;

    protected override void Start()
    {
        base.Start();

        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    public override void ManualUpdate()
    {
        if (playerController.GetMovement() != Vector3.zero && currentFuel > 0)
            Move();

        Aim();
        CalculateShootForce();

        if (playerController.Trigger_Fire() && CanFire())
            Fire();

        // Fire() must come before PreviewProjectileTrajectory()
        // (For unknown reason)
        PreviewProjectileTrajectory();
    }

    public void Move()
    {
        if (playerController.GetMovement().x > 0)
            chassi.transform.rotation = Quaternion.Euler(0, 0, 0);

        else if (playerController.GetMovement().x < 0)
            chassi.transform.rotation = Quaternion.Euler(0, -180, 0);

        Vector3 localDirection = chassi.transform.InverseTransformDirection(Vector3.right);

        RaycastHit hit;
        if (Physics.Raycast(chassi.transform.position + new Vector3(0, 1, 0) * 0.66f + localDirection, Vector3.down, out hit, 2, groundLayerMask))
        {
            if (hit.normal.y < 0.85f)
                return;
        }

        gameObject.transform.position += playerController.GetMovement() * movementSpeed;
        timeSinceLastEffect += Time.deltaTime;
        if (timeSinceLastEffect > timeBetweenEffectSpawn)
        {
            Instantiate(movementEffect, transform.position, Quaternion.identity);
            timeSinceLastEffect = 0;
        }

        if (!isSlowed)
            currentFuel -= playerController.GetMovement().magnitude;
        else
            currentFuel -= playerController.GetMovement().magnitude * 2;
    }

    public void Aim()
    {
        tower.Aim();
        gun.Aim();
    }

    public void CalculateShootForce()
    {
        Vector2 cannonScreenPos = Camera.main.WorldToScreenPoint(gun.transform.position);
        float percentage = Vector2.Distance(cannonScreenPos, playerController.GetMousePosition()) / aimRadius;
        percentage = Mathf.Clamp01(percentage);
        currentShootForce = percentage * maxShootForce;
    }
}
