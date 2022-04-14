using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Projectile")]
    [SerializeField] Projectile[] projectiles;
    [SerializeField] Projectile currentProjectile;
    [SerializeField] ParticleSystem fireParticles;

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

    float timeSinceLastEffect;
    int projectileIndex;
    bool hasShot = false;
    float currentFuel;
    float currentHealth;
    [SerializeField] float currentShootForce;
    Camera mainCamera;
    PlayerController playerController;
    Rigidbody rb;
    Ray ray;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;
        currentFuel = maxFuel;
        currentProjectile = projectiles[0];

        foreach (Projectile projectile in projectiles)
            projectile.ResetAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetCurrentPlayerIndex() != playerIndex || hasShot)
            return;


        Move();
        Aim();
        SwapProjectile();
        CalculateShootForce();
        Shoot();
    }

    public void Move()
    {
        if (playerController.IsJumping())
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }

        if (playerController.GetMovement() == Vector3.zero || currentFuel <= 0)
            return;
        

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
        timeSinceLastEffect += Time.deltaTime;
        if(timeSinceLastEffect > timeBetweenEffectSpawn)
        {
            Instantiate(movementEffect, transform.position, Quaternion.identity);
            timeSinceLastEffect = 0;
        }

        currentFuel -= playerController.GetMovement().magnitude;

        fuelSlider.value = currentFuel / maxFuel;
    }

    public void Aim()
    {
        Vector2 cannonScreenPos = mainCamera.WorldToScreenPoint(rotatePoint.transform.position);
        Vector2 lookVector = playerController.GetMousePosition() - cannonScreenPos;

        float rotationZ = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg;
        rotatePoint.transform.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(rotationZ, 0, 180));
        rotatePoint.transform.Rotate(0, 0, -90);

    }

    public void Shoot()
    {
        if (!playerController.IsShooting() || !currentProjectile.HasAmmo())
            return;

        currentProjectile.ChangeAmmoCount(-1);
        Instantiate(fireParticles, firePoint.position, Quaternion.identity, null);
        Projectile projectile = Instantiate(currentProjectile.gameObject, firePoint).GetComponent<Projectile>();

        projectile.transform.parent = null;
        projectile.SetOwnTank(this);
        projectile.Shoot(cannon.transform.rotation, currentShootForce);

        hasShot = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            gameManager.RemoveTankFromList(this);
        }
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
        hasShot = false;
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

    public void CalculateShootForce()
    {
        Vector2 cannonScreenPos = mainCamera.WorldToScreenPoint(cannon.transform.position);
        currentShootForce = Math.Min(Vector3.Distance(cannonScreenPos, playerController.GetMousePosition()), maxShootForce);

        shootForceSlider.value = currentShootForce / maxShootForce;
    }

    public void SwapProjectile()
    {
        if (playerController.GetNewWeapon() == 0)
            return;

        projectileIndex += playerController.GetNewWeapon();

        if (projectileIndex >= projectiles.Length)
            projectileIndex = 0;
        else if (projectileIndex < 0)
            projectileIndex = projectiles.Length - 1;

        currentProjectile = projectiles[projectileIndex];
    }
}
