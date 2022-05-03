using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameManager gameManager;
    [SerializeField] CameraController cameraController;
    [SerializeField] bool isActive = false;
    [SerializeField] string playerName;
    [SerializeField] public Color playerColor;

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
    //[SerializeField] List<AttackPattern> attackPatterns;
    [SerializeField] List<int> ammo;
    [SerializeField] Projectile currentProjectile;
    [SerializeField] ParticleSystem fireParticles;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem tankDamagedParticles;

    [Header("Explosion")]
    [SerializeField] Explosion explosion;
    [SerializeField] float explosionDamage;

    [Header("Status Effects")]
    [SerializeField] bool isSlowed;

    [Header("References")]
    [SerializeField] GameObject rotatePoint;
    [SerializeField] GameObject[] tankParts;
    [SerializeField] GameObject cannon;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform rumbleSpot;

    [Header("UI")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider fuelSlider;
    [SerializeField] Slider shootForceSlider;
    [SerializeField] float aimRadius;

    float timeSinceLastEffect;
    int projectileIndex;
    bool hasFired = false;
    float currentFuel;
    float currentHealth;
    float currentShootForce;
    PlayerController playerController;
    GameObject followProjectile;
    Rigidbody rb;
    Ray ray;
    TMP_Text projectileTMP;

    public string GetPlayerName() => playerName;

    public Projectile GetCurrentProjectile() => currentProjectile;

    public float GetFuelPercentage() => currentFuel / maxFuel;

    public float GetHealthPercentage() => currentHealth / maxHealth;

    public GameManager GetGameManager() => gameManager;

    public bool HasAmmo() => ammo[projectileIndex] > 0;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        projectileTMP = GameObject.Find("GUI").GetComponentsInChildren<TextMeshProUGUI>().ToList().Find(item => item.name == "Current projectile");
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentFuel = maxFuel;
        currentProjectile = projectiles[0];

        for (int i = 0; i < projectiles.Count; i++)
            ammo.Add(projectiles[i].GetStartAmmoCount());
    }

    void Update()
    {
        if (!isActive)
            return;

        if (playerController.GetMovement() != Vector3.zero && currentFuel > 0)
            Move();

        if (playerController.GetNewWeapon() != 0)
            SwapProjectile();

        Aim();
        CalculateShootForce();

        if (playerController.IsShooting() && CanFire())
            Fire();

        // Fire() must come before PreviewProjectileTrajectory()
        // (For some reason ? )
        PreviewProjectileTrajectory();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Water" && currentHealth > 0)
        {
            TakeDamage(currentHealth);
            gameManager.StartPlayerTransition();
        }
    }

    public void Move()
    {
        if (playerController.GetMovement().x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (playerController.GetMovement().x < 0)
            transform.rotation = Quaternion.Euler(0, -180, 0);

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
        fuelSlider.value = currentFuel / maxFuel;
    }

    public void Aim()
    {
        Vector2 cannonScreenPos = Camera.main.WorldToScreenPoint(rotatePoint.transform.position);
        Vector2 lookVector = playerController.GetMousePosition() - cannonScreenPos;

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
            && isActive
            && currentHealth > 0.0f;
    }

    public Projectile InstantiateProjectile()
    {
        Projectile projectile = Instantiate(currentProjectile, firePoint);
        projectile.ownTank = this;
        projectile.transform.parent = null;
        projectile.Fire(cannon.transform.rotation, currentShootForce);
        followProjectile = projectile.gameObject;
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

        if (animator)
            animator.SetTrigger("Fire");

        //foreach (AttackPattern attackPattern in attackPatterns)
        //{
        //    if (attackPattern == currentProjectile.GetAttackPattern())
        //    {
        //        attackPattern.Fire(this);
        //    }
        //}

        Instantiate(fireParticles, firePoint.position, Quaternion.identity, null);
        currentProjectile.GetAttackPattern().Fire(this);

        //Projectile projectile = InstantiateProjectile();


        // Update camera

        if (firstPersonView)
            StartCoroutine(cameraController.Coroutine_KillCamSequence(result.Value, followProjectile));

        else if (result == null)
            StartCoroutine(cameraController.focusPoint.Coroutine_DelayedFollowObject(followProjectile, 0.2f));

        else
        {
            if (result.Value.tank != null)
                cameraController.focusPoint.FollowObject(result.Value.tank.gameObject);
            else
                cameraController.focusPoint.SetPosition(result.Value.raycastHit.point + cameraController.focusPoint.GetDefaultOffset());

            cameraController.Transition(CameraController.View.Side, result.Value.timeBeforeHit);
        }

        if (ammo[projectileIndex] <= 0)
        {
            projectiles.RemoveAt(projectileIndex);
            ammo.RemoveAt(projectileIndex);
            SwapProjectile();
        }
        isActive = false;
        gameManager.StartPlayerTransition();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth > 0.0f)
        {
            animator.SetTrigger("Damaged");
            Instantiate(tankDamagedParticles, transform.position, Quaternion.identity, null);
        }
        else
        {
            animator.SetTrigger("Destroyed");
        }

        healthSlider.value = currentHealth / maxHealth;
    }

    public void DeactivateTank()
    {
        gameObject.SetActive(false);
    }

    public void AssignPlayer()
    {
        playerName = gameManager.AssignName();
        playerColor = gameManager.AssignColor();


        foreach (GameObject go in tankParts)
        {
            go.GetComponent<Renderer>().material.color = playerColor;
        }

        nameText.text = playerName;
    }

    public void ReadyTank()
    {
        fuelSlider.gameObject.SetActive(true);
        shootForceSlider.gameObject.SetActive(true);

        currentFuel = maxFuel;
        fuelSlider.value = currentFuel / maxFuel;
        isActive = true;
        hasFired = false;

        if (cameraController == null)
            cameraController = Camera.main.GetComponent<CameraController>();

        cameraController.focusPoint.FollowObject(gameObject);
        cameraController.Transition(CameraController.View.Side, 1.0f);

        if (projectileTMP)
            projectileTMP.text = "Current projectile: " + currentProjectile.name;
    }

    public void UnreadyTank()
    {
        fuelSlider.gameObject.SetActive(false);
        shootForceSlider.gameObject.SetActive(false);

        foreach (GameObject go in tankParts)
        {
            go.GetComponent<Renderer>().material.color = playerColor;
        }
        isSlowed = false;
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

        if (projectileTMP)
            projectileTMP.text = "Current projectile: " + currentProjectile.name;
    }

    public void SetHasFired(bool state)
    {
        hasFired = state;
    }

    public void SetIsSlowed(bool state)
    {
        isSlowed = state;

        foreach (GameObject go in tankParts)
        {
            go.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
    }

    private void PreviewProjectileTrajectory()
    {
        Projectile projectile = InstantiateProjectile();
        projectile.PrecomputeTrajectory(0.05f);
    }

    public void Explode()
    {
        explosion.SetDamage(explosionDamage);
        explosion.Explode();
    }

    public void SetFollowProjectile(GameObject projectile)
    {
        followProjectile = projectile;
    }
}
