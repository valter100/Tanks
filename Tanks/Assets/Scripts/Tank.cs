using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameManager gameManager;
    [SerializeField] CameraController cameraController;
    [SerializeField] int playerIndex;
    [SerializeField] string playerName;
    [SerializeField] Color playerColor;

    [Header("Stats")]
    [SerializeField] float maxHealth;
    [SerializeField] float maxShootForce;
    [SerializeField] float maxFuel;

    [Header("Movement")]
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] ParticleSystem movementEffect;
    [SerializeField] float timeBetweenEffectSpawn;

    [Header("Combat")]
    [SerializeField] List<Projectile> projectiles;
    [SerializeField] List<int> ammo;
    [SerializeField] Projectile currentProjectile;
    [SerializeField] ParticleSystem fireParticles;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem tankDestroyedParticles;

    [Header("Status Effects")]
    [SerializeField] bool isSlowed;

    [Header("References")]
    [SerializeField] GameObject rotatePoint;
    [SerializeField] GameObject cannon;
    [SerializeField] Transform firePoint;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform rumbleSpot;

    [Header("UI")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text fuelText;
    [SerializeField] Slider fuelSlider;
    [SerializeField] Slider shootForceSlider;
    [SerializeField] TMP_Text projectileText;
    [SerializeField] float aimRadius;

    float timeSinceLastEffect;
    int projectileIndex;
    bool hasFired = false;
    float currentFuel;
    float currentHealth;
    float currentShootForce;
    PlayerController playerController;
    Rigidbody rb;
    Ray ray;

    public string GetPlayerName() => playerName;

    public Projectile GetCurrentProjectile() => currentProjectile;

    public float GetFuelPercentage() => currentFuel / maxFuel;

    public float GetHealthPercentage() => currentHealth / maxHealth;

    public GameManager GetGameManager() => gameManager;

    public bool HasAmmo() => ammo[projectileIndex] > 0;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;
        currentFuel = maxFuel;
        currentProjectile = projectiles[0];

        for (int i = 0; i < projectiles.Count; i++)
            ammo.Add(projectiles[i].GetStartAmmoCount());
    }

    void Update()
    {
        if (gameManager.GetCurrentPlayerIndex() != playerIndex || hasFired || currentHealth <= 0)
            return;

        if (playerController.GetMovement() != Vector3.zero && currentFuel > 0)
            Move();

        if (playerController.GetNewWeapon() != 0)
            SwapProjectile();

        Aim();
        CalculateShootForce();
        
        if (Input.GetKeyDown(KeyCode.Space) && CanFire())
            Fire();

        // Fire() must come before PreviewProjectileTrajectory()
        // (For some reason ? )
        PreviewProjectileTrajectory();
    }

    public void Move()
    {
        if (playerController.IsJumping())
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }

        if (playerController.GetMovement().x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (playerController.GetMovement().x < 0)
            transform.rotation = Quaternion.Euler(0, -180, 0);

        Vector3 localDirection = transform.InverseTransformDirection(Vector3.right);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) * 0.66f + localDirection, Vector3.down, out hit, 2, groundLayerMask))
        {
            //Debug.DrawLine(transform.position + new Vector3(0,1,0) * 0.66f + localDirection, hit.point);
            //Debug.Log(hit.normal.y);
            if (hit.normal.y < 0.85f)
                return;
        }

        gameObject.transform.position += playerController.GetMovement() * movementSpeed;
        //rb.AddForce(playerController.GetMovement() * movementSpeed);
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
        fuelSlider.value = currentFuel / maxFuel;
    }

    public void Aim()
    {
        Vector2 cannonScreenPos = Camera.main.WorldToScreenPoint(rotatePoint.transform.position);
        Vector2 lookVector = playerController.GetMousePosition() - cannonScreenPos;

        //Quaternion newRotation = Quaternion.LookRotation(lookVector);
        //rotatePoint.transform.rotation = newRotation;

        float rotationZ = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg - 90;

        if (rotationZ < -90)
            rotationZ = -90;
        else if (rotationZ > 179)
            rotationZ = 179;

        rotatePoint.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    public bool CanFire()
    {
        return HasAmmo()
            && !hasFired
            && gameManager.GetCurrentPlayerIndex() == playerIndex
            && currentHealth > 0.0f;
    }

    private Projectile InstantiateProjectile()
    {
        Projectile projectile = Instantiate(currentProjectile, firePoint);
        projectile.transform.parent = null;
        projectile.ownTank = this;
        projectile.Fire(cannon.transform.rotation, currentShootForce);
        return projectile;
    }

    public void Fire()
    {
        // Precompute projectile

        Projectile precomputedProjectile = InstantiateProjectile();
        Projectile.PrecomputedResult? result = precomputedProjectile.PrecomputeTrajectory();

        // Determine if the projectile hit a Tank which will be destroyed

        bool firstPersonView = result != null
            && result.Value.tank != null
            && result.Value.tank.currentHealth - result.Value.damageDealtToTank <= 0.0f;

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
            cameraController.focusPoint.FollowObject(projectile.gameObject);

        else
        {
            if (result.Value.tank != null)
                cameraController.focusPoint.FollowObject(result.Value.tank.gameObject);
            else
                cameraController.focusPoint.SetPosition(result.Value.raycastHit.point + cameraController.focusPoint.GetDefaultOffset());

            cameraController.Transition(CameraController.View.Side, result.Value.timeBeforeHit);
        }
    }

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("Damaged");
        currentHealth -= damage;

        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            animator.SetTrigger("Destroyed");
        }

        healthSlider.value = currentHealth / maxHealth;
    }

    public void RemoveTank()
    {
        gameManager.RemoveTankFromList(this);
    }

    public void SpawnDestroyedParticles()
    {
        Instantiate(tankDestroyedParticles, transform.position, Quaternion.Euler(-90, 0, 0), null);
    }

    public void AssignPlayer(int newIndex, string newName, Color newColor)
    {
        playerIndex = newIndex;
        playerName = newName;
        playerColor = newColor;

        GetComponent<MeshRenderer>().material.color = newColor;
    }

    public void ReadyTank()
    {
        fuelSlider.gameObject.SetActive(true);
        shootForceSlider.gameObject.SetActive(true);
        lineRenderer.gameObject.SetActive(true);

        currentFuel = maxFuel;
        fuelSlider.value = currentFuel / maxFuel;
        hasFired = false;
        isSlowed = false;
        GetComponent<Renderer>().material.color = playerColor;

        if (cameraController == null)
            cameraController = Camera.main.GetComponent<CameraController>();

        cameraController.focusPoint.FollowObject(gameObject);
        cameraController.Transition(CameraController.View.Side, 1.0f);
    }

    public void UnreadyTank()
    {
        fuelSlider.gameObject.SetActive(false);
        shootForceSlider.gameObject.SetActive(false);
        lineRenderer?.gameObject.SetActive(false);
    }

    public void CalculateShootForce()
    {
        Vector2 cannonScreenPos = Camera.main.WorldToScreenPoint(cannon.transform.position);
        float percentage = Vector2.Distance(cannonScreenPos, playerController.GetMousePosition()) / aimRadius;
        percentage = Mathf.Clamp01(percentage);
        currentShootForce = percentage * maxShootForce;
        shootForceSlider.value = percentage;
    }

    public void SwapProjectile()
    {
        projectileIndex += playerController.GetNewWeapon() + projectiles.Count;
        projectileIndex %= projectiles.Count;
        currentProjectile = projectiles[projectileIndex];
    }

    public void SetIsSlowed(bool state)
    {
        isSlowed = state;
        GetComponent<Renderer>().material.color = Color.blue;
    }

    private void PreviewProjectileTrajectory()
    {
        Projectile projectile = InstantiateProjectile();
        projectile.PrecomputeTrajectory(0.05f);
    }
}
