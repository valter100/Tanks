using System.Linq;
using TMPro;
using UnityEngine;


public class AiTank : Tank
{
    AiManager aiManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        projectileTMP = GameObject.Find("GUI").GetComponentsInChildren<TextMeshProUGUI>().ToList().Find(item => item.name == "Current projectile");
        rb = GetComponent<Rigidbody>();
        aiManager = GetComponent<AiManager>();

        currentHealth = maxHealth;
        currentFuel = maxFuel;
        currentProjectile = projectiles[0];

        for (int i = 0; i < projectiles.Count; i++)
            ammo.Add(projectiles[i].GetStartAmmoCount());
    }

    void Update()
    {
        aiManager.Update();
    }

    public void Move(Transform enemyTankPosition)
    {
        if (enemyTankPosition.position.x > gameObject.transform.position.x)
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (enemyTankPosition.position.x < gameObject.transform.position.x)
            gameObject.transform.rotation = Quaternion.Euler(0, -180, 0);

        Vector3 localDirection = gameObject.transform.InverseTransformDirection(Vector3.right);

        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position + new Vector3(0, 1, 0) * 0.66f + localDirection, Vector3.down, out hit, 2, groundLayerMask))
        {
            if (hit.normal.y < 0.85f)
                return;
        }

        if (enemyTankPosition.position.x > gameObject.transform.position.x)
            gameObject.transform.position += Vector3.right * movementSpeed * Time.deltaTime;
        else if (enemyTankPosition.position.x < gameObject.transform.position.x)
            gameObject.transform.position -= Vector3.right * movementSpeed * Time.deltaTime;

        
        timeSinceLastEffect += Time.deltaTime;
        if (timeSinceLastEffect > timeBetweenEffectSpawn)
        {
            Instantiate(movementEffect, transform.position, Quaternion.identity);
            timeSinceLastEffect = 0;
        }

        if (!isSlowed)
            currentFuel -= 0.05f; // 1 should be changed for the correct value;
        else
            currentFuel -= 0.05f * 2;
        fuelSlider.value = currentFuel / maxFuel;
    }

    public void Aim()
    {
        //Vector2 cannonScreenPos = Camera.main.WorldToScreenPoint(rotatePoint.transform.position);
        //Vector2 lookVector = playerController.GetMousePosition() - cannonScreenPos;

        ////Quaternion newRotation = Quaternion.LookRotation(lookVector);
        ////rotatePoint.transform.rotation = newRotation;

        //float rotationZ = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg - 90;

        //if (rotationZ < -90)
        //    rotationZ = -90;
        //else if (rotationZ > 179)
        //    rotationZ = 179;

        //rotatePoint.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    public override void Fire()
    {
        // Precompute projectile

        Projectile precomputedProjectile = InstantiateProjectile();
        Projectile.PrecomputedResult? result = precomputedProjectile.PrecomputeTrajectory();

        // Determine if the projectile hit a Tank which will be destroyed

        bool firstPersonView = result != null
            && result.Value.tank != null
            && result.Value.tank.GetCurrentHealth() - result.Value.damageDealtToTank <= 0.0f;

        // Fire projectile

        ammo[projectileIndex] -= 1;
        hasFired = true;
        animator.SetTrigger("Fire");
        Instantiate(fireParticles, firePoint.position, Quaternion.identity, null);

        Projectile projectile = InstantiateProjectile();

        if (ammo[projectileIndex] <= 0)
        {
            projectiles.RemoveAt(projectileIndex);
            ammo.RemoveAt(projectileIndex);
            SwapProjectile();
        }

        // Update camera

        if (firstPersonView)
            StartCoroutine(cameraController.Coroutine_KillCamSequence(result.Value, projectile.gameObject));

        else if (result == null)
            StartCoroutine(cameraController.focusPoint.Coroutine_DelayedFollowObject(projectile.gameObject, 0.2f));

        else
        {
            if (result.Value.tank != null)
                cameraController.focusPoint.FollowObject(result.Value.tank.gameObject);
            else
                cameraController.focusPoint.SetPosition(result.Value.raycastHit.point + cameraController.focusPoint.GetDefaultOffset());

            cameraController.Transition(CameraController.View.Side, result.Value.timeBeforeHit);
        }

        isActive = false;
        gameManager.StartPlayerTransition();
    }

    public void CalculateShootForce()
    {
        //Vector2 cannonScreenPos = Camera.main.WorldToScreenPoint(cannon.transform.position);
        //float percentage = Vector2.Distance(cannonScreenPos, playerController.GetMousePosition()) / aimRadius;
        //percentage = Mathf.Clamp01(percentage);
        //currentShootForce = percentage * maxShootForce;
        //shootForceSlider.value = percentage;
    }

    public void SwapProjectile()
    {
        //projectileIndex += playerController.GetNewWeapon() + projectiles.Count;
        //projectileIndex %= projectiles.Count;
        //currentProjectile = projectiles[projectileIndex];

        //if (projectileTMP)
        //    projectileTMP.text = "Current projectile: " + currentProjectile.name;
    }

    private void PreviewProjectileTrajectory()
    {
        Projectile projectile = InstantiateProjectile();
        projectile.PrecomputeTrajectory(0.05f);
    }
}
