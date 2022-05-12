using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tank : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float maxShootForce;
    [SerializeField] protected float maxFuel;

    [Header("Movement")]
    [SerializeField] protected LayerMask groundLayerMask;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected ParticleSystem movementEffect;
    [SerializeField] protected float timeBetweenEffectSpawn;

    [Header("Combat")]
    [SerializeField] protected ParticleSystem fireParticles;
    [SerializeField] protected Animator animator;
    [SerializeField] protected ParticleSystem damagedParticles;

    [Header("Explosion")]
    [SerializeField] protected Explosion explosion;
    [SerializeField] protected float explosionDamage;

    [Header("Status Effects")]
    [SerializeField] protected bool isSlowed;

    [Header("References")]
    [SerializeField] protected static GameManager gameManager;
    [SerializeField] protected static CameraController cameraController;
    [SerializeField] protected Player player;
    [SerializeField] protected GameObject rotatePoint;
    [SerializeField] protected GameObject[] tankParts;
    [SerializeField] protected GameObject chassi;
    [SerializeField] protected Tower tower;
    [SerializeField] protected Gun gun;
    [SerializeField] protected Transform rumbleSpot;

    [Header("GUI")]
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected Slider healthSlider;
    [SerializeField] protected Slider fuelSlider;
    [SerializeField] protected Slider shootForceSlider;
    [SerializeField] protected float aimRadius;

    [Header("Debug Settings")]
    [SerializeField] protected bool debugTrajectory;

    protected float timeSinceLastEffect;
    protected int projectileIndex;
    protected bool hasFired = false;
    protected float currentFuel;
    protected float currentHealth;
    protected float currentShootForce;
    protected Rigidbody rb;
    protected Ray ray;

    public float GetFuelPercentage() => currentFuel / maxFuel;

    public float GetHealthPercentage() => currentHealth / maxHealth;

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxShootForce() => maxShootForce;

    public float GetMovementSpeed() => movementSpeed;

    public GameManager GetGameManager() => gameManager;

    public bool CanFire() => !hasFired && currentHealth > 0.0f;


    protected virtual void Start()
    {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (cameraController == null)
            cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();

        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        currentFuel = maxFuel;
    }

    public virtual void ManualUpdate()
    {
        
    }

    protected void PreviewProjectileTrajectory()
    {
        Projectile projectile = InstantiateProjectile();
        projectile.PrecomputeTrajectory(0.05f);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Water" && currentHealth > 0)
        {
            TakeDamage(currentHealth);
            gameManager.StartPlayerTransition();
        }
    }

    public Projectile InstantiateProjectile()
    {
        Projectile projectile = Instantiate(player.Inventory.SelectedItem.prefab, gun.GetFirePoint()).GetComponent<Projectile>();
        projectile.ownTank = this;
        projectile.transform.parent = null;
        projectile.Fire(gun.transform.parent.transform.rotation, currentShootForce);
        return projectile;
    }

    protected void Fire()
    {
        Debug.Log("FIRED");
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

        Instantiate(fireParticles, gun.GetFirePoint().position, Quaternion.identity, null);
        player.Inventory.SelectedItem.prefab.GetComponent<Projectile>().GetAttackPattern().Fire(this);

        Projectile projectile = InstantiateProjectile();

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

        gameManager.StartPlayerTransition();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Math.Max(0.0f, currentHealth - damage);

        if (currentHealth > 0.0f)
        {
            animator.SetTrigger("Damaged");
            Instantiate(damagedParticles, transform.position, Quaternion.identity, null);
        }
        else
        {
            animator.SetTrigger("Destroyed");
        }

        healthSlider.value = currentHealth / maxHealth;
    }

    public void LinkPlayer(Player player)
    {
        this.player = player;
        nameText.text = player.Info.name;
        SetColor(player.Info.color);

        if (player.Info.control == Control.Player)
        {
            GetComponent<PlayerTank>().enabled = true;
            GetComponent<PlayerController>().enabled = true;
        }
        else if (player.Info.control == Control.Bot)
        {
            GetComponent<AiTank>().enabled = true;
            GetComponent<AiManager>().enabled = true;
        }
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

    public void SetHasFired(bool state)
    {
        hasFired = state;
    }

    public void SetIsSlowed(bool state)
    {
        isSlowed = state;
        SetColor(Color.cyan);
    }

    public void SetColor(Color color)
    {
        foreach (GameObject tankPart in tankParts)
        {
            tankPart.GetComponent<Renderer>().material.color = color;
        }
    }
}
