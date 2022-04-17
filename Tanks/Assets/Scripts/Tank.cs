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
    [SerializeField] float currentShootForce;
    CameraController cameraController;
    PlayerController playerController;
    Rigidbody rb;
    Ray ray;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;
        currentFuel = maxFuel;
        currentProjectile = projectiles[0];

        for (int i = 0; i < projectiles.Count; i++)
        {
            ammo.Add(projectiles[i].GetAmmoCount());
        }
    }

    void Update()
    {
        if (gameManager.GetCurrentPlayerIndex() != playerIndex || hasFired || currentHealth <= 0)
            return;

        if (playerController.GetMovement() != Vector3.zero && currentFuel > 0)
            Move();

        Aim();

        if (playerController.GetNewWeapon() == 1 || playerController.GetNewWeapon() == -1)
            SwapProjectile();

        CalculateShootForce();
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

    public void Fire()
    {
        ammo[projectileIndex] -= 1;
        Instantiate(fireParticles, firePoint.position, Quaternion.identity, null);
        Projectile projectile = Instantiate(currentProjectile.gameObject, firePoint).GetComponent<Projectile>();

        projectile.transform.parent = null;
        projectile.SetOwnTank(this);
        projectile.Fire(cannon.transform.rotation, currentShootForce);

        hasFired = true;

        animator.SetTrigger("Fire");

        if (ammo[projectileIndex] <= 0)
        {
            projectiles.RemoveAt(projectileIndex);
            ammo.RemoveAt(projectileIndex);

            SwapProjectile();
        }

        // Precompute point of collision

        Vector3? collisionPoint = projectile.PrecomputeTrajectory();

        // Update camera

        if (collisionPoint != null)
        {
            cameraController.focusPoint.SetPosition(collisionPoint.Value);
            cameraController.FollowObject(projectile.gameObject);
            cameraController.Transition(CameraController.View.FirstPerson, 0.6f);
        }

        else
        {
            cameraController.focusPoint.FollowObject(projectile.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth / maxHealth;

        animator.SetTrigger("Damaged");

        if (currentHealth <= 0)
        {
            animator.SetTrigger("Destroyed");
        }
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

    public GameManager GetGameManager()
    {
        return gameManager;
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
    }

    public void UnreadyTank()
    {
        fuelSlider.gameObject.SetActive(false);
        shootForceSlider.gameObject.SetActive(false);
        lineRenderer?.gameObject.SetActive(false);
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public Projectile GetCurrentProjectile()
    {
        return currentProjectile;
    }

    public float GetFuelPercentage()
    {
        return currentFuel / maxFuel;
    }

    public float GetHealthPercentage()
    {
        return (currentHealth / maxHealth);
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
        projectileIndex += playerController.GetNewWeapon();

        if (projectileIndex >= projectiles.Count)
            projectileIndex = 0;
        else if (projectileIndex < 0)
            projectileIndex = projectiles.Count - 1;

        currentProjectile = projectiles[projectileIndex];
    }

    public bool HasAmmo()
    {
        return ammo[projectileIndex] > 0;
    }

    public void SetIsSlowed(bool state)
    {
        isSlowed = state;
        GetComponent<Renderer>().material.color = Color.blue;
    }
}
