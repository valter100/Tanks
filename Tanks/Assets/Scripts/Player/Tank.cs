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
    [SerializeField] ParticleSystem fireParticles;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem tankDamagedParticles;

    [Header("Explosion")]
    [SerializeField] Explosion explosion;
    [SerializeField] float explosionDamage;

    [Header("Status Effects")]
    [SerializeField] bool isSlowed;

    [Header("References")]
    [SerializeField] static GameManager gameManager;
    [SerializeField] static CameraController cameraController;
    [SerializeField] Player player;
    [SerializeField] GameObject rotatePoint;
    [SerializeField] GameObject[] tankParts;
    [SerializeField] GameObject cannon;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform rumbleSpot;

    [Header("GUI")]
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

    public float GetFuelPercentage() => currentFuel / maxFuel;

    public float GetHealthPercentage() => currentHealth / maxHealth;

    public GameManager GetGameManager() => gameManager;

    public bool CanFire() => !hasFired && currentHealth > 0.0f;


    private void Awake()
    {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (cameraController == null)
            cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();

        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentFuel = maxFuel;
    }

    public void ManualUpdate()
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

        //Vector3 localDirection = transform.InverseTransformDirection(Vector3.right);

        //RaycastHit hit;
        //if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) * 0.66f + localDirection, Vector3.down, out hit, 2, groundLayerMask))
        //{
        //    //Debug.DrawLine(transform.position + new Vector3(0,1,0) * 0.66f + localDirection, hit.point);
        //    //Debug.Log(hit.normal.y);
        //    if (hit.normal.y < 0.85f)
        //        return;
        //}

        gameObject.transform.position += playerController.GetMovement() * movementSpeed;

        timeSinceLastEffect += Time.deltaTime;
        if (timeSinceLastEffect > timeBetweenEffectSpawn)
        {
            Instantiate(movementEffect, transform.position, Quaternion.identity);
            timeSinceLastEffect = 0;
        }

        currentFuel += playerController.GetMovement().magnitude * (isSlowed ? 2.0f : 1.0f);
        currentFuel = Math.Max(0.0f, currentFuel);

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

    public Projectile InstantiateProjectile()
    {
        Projectile projectile = Instantiate(player.Inventory.SelectedItem.prefab, firePoint).GetComponent<Projectile>();
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

        player.Inventory.DecreaseAmountOfSelectedItem();
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
        player.Inventory.SelectedItem.prefab.GetComponent<Projectile>().GetAttackPattern().Fire(this);

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

        gameManager.StartPlayerTransition();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Math.Max(0.0f, currentHealth - damage);

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

    public void LinkPlayer(Player player)
    {
        this.player = player;
        nameText.text = player.Info.name;
        SetColor(player.Info.color);
    }

    public void Ready()
    {
        fuelSlider.gameObject.SetActive(true);
        shootForceSlider.gameObject.SetActive(true);

        currentFuel = maxFuel;
        fuelSlider.value = currentFuel / maxFuel;
        hasFired = false;

        if (cameraController == null)
            cameraController = Camera.main.GetComponent<CameraController>();

        cameraController.focusPoint.FollowObject(gameObject);
        cameraController.Transition(CameraController.View.Side, 1.0f);
    }

    public void Unready()
    {
        fuelSlider.gameObject.SetActive(false);
        shootForceSlider.gameObject.SetActive(false);
        isSlowed = false;
        SetColor(player.Info.color);
    }

    public void CalculateShootForce()
    {
        Vector2 cannonScreenPos = Camera.main.WorldToScreenPoint(cannon.transform.position);
        float percentage = Vector2.Distance(cannonScreenPos, playerController.GetMousePosition()) / aimRadius;
        percentage = Mathf.Clamp01(percentage);
        currentShootForce = percentage * maxShootForce;
        shootForceSlider.value = percentage;
    }

    public void SetHasFired(bool state)
    {
        hasFired = state;
    }

    public void SetIsSlowed(bool state)
    {
        isSlowed = state;
        SetColor(Color.cyan);
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

    public void SetColor(Color color)
    {
        foreach (GameObject tankPart in tankParts)
        {
            tankPart.GetComponent<Renderer>().material.color = color;
        }
    }
}
