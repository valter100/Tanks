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
    [Header("Max Stats")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float maxFuel;
    [SerializeField] protected float maxShootForce;

    //[Header("Current Stats")]
    protected float currentHealth;
    protected float currentFuel;
    protected float currentShootForce;

    [Header("Movement")]
    [SerializeField] protected LayerMask groundLayerMask;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected ParticleSystem movementEffect;
    [SerializeField] protected float timeBetweenEffectSpawn;
    [SerializeField] protected float rightRotation;
    [SerializeField] protected float leftRotation;

    [Header("Combat")]
    [SerializeField] protected ParticleSystem fireParticles;
    [SerializeField] protected AudioClip fireSound;
    protected Animator animator;
    [SerializeField] protected ParticleSystem damagedParticles;
    [SerializeField] protected float aimRadius;

    [Header("Explosion")]
    protected Explosion explosion;
    [SerializeField] protected AudioClip explosionSound;
    [SerializeField] protected float explosionDamage;

    //[Header("Status Effects")]
    protected bool isSlowed;

    [Header("References")]
    protected static GameManager gameManager;
    protected static CameraController cameraController;
    protected Player player;
    //[SerializeField] protected GameObject rotatePoint;
    [SerializeField] protected GameObject[] customColoredParts;
    [SerializeField] protected GameObject[] baseColoredParts;
    [SerializeField] protected GameObject chassi;
    [SerializeField] protected Transform rumbleSpot;
    protected Tower tower;
    protected Gun gun;

    [Header("Other")]
    [SerializeField] protected bool debugTrajectory;

    protected bool preview;
    protected bool facingRight;
    static protected Color baseColor = new Color(0.22f, 0.22f, 0.22f);

    protected float timeSinceLastEffect;
    protected bool hasFired = false;
    protected Rigidbody rb;
    protected Ray ray;

    public Player GetPlayer() => player;

    public float GetFuelPercentage() => currentFuel / maxFuel;

    public float GetHealthPercentage() => currentHealth / maxHealth;

    public float GetPowerPercentage() => currentShootForce / maxShootForce;

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxShootForce() => maxShootForce;

    public float GetMovementSpeed() => movementSpeed;

    public GameManager GetGameManager() => gameManager;

    public bool CanFire() => !hasFired && currentHealth > 0.0f;

    public bool Destroyed() => currentHealth <= 0.0f;

    protected virtual void Awake()
    {
        foreach (GameObject baseColorPart in baseColoredParts)
            baseColorPart.GetComponent<Renderer>().material.color = baseColor;
    }

    protected virtual void Start()
    {
        if (!preview)
        {
            if (gameManager == null)
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            if (cameraController == null)
                cameraController = CameraController.Instance;
        }

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        explosion = GetComponent<Explosion>();
        tower = GetComponentInChildren<Tower>();
        gun = GetComponentInChildren<Gun>();

        currentHealth = maxHealth;
        currentFuel = maxFuel;
    }

    public abstract void ManualUpdate();

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
        Projectile projectile = Instantiate(player.Inventory.SelectedItem.usable.gameObject, gun.GetFirePoint()).GetComponent<Projectile>();
        projectile.ownTank = this;
        projectile.transform.parent = null;
        projectile.Fire(gun.transform.parent.transform.rotation, currentShootForce);
        return projectile;
    }

    protected void Fire()
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
        Instantiate(fireParticles, gun.GetFirePoint().position, Quaternion.identity, null);
        GetComponent<AudioSource>().PlayOneShot(fireSound);
        Projectile projectile = InstantiateProjectile();

        if (animator)
            animator.SetTrigger("Fire");

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
        if (currentHealth <= 0)
            return;

        MessagesManager.AddMessage((-damage).ToString("0.0") + " HP").SetColor(Color.red).SetWorldPosition(transform.position);
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
    }

    public void Explode()
    {
        explosion.SetDamage(explosionDamage);
        GetComponent<AudioSource>().PlayOneShot(explosionSound);
        explosion.Explode();
    }

    public virtual void LinkPlayer(Player player)
    {
        this.player = player;
        SetColor(player.Info.color);
    }

    public virtual void Ready()
    {
        currentFuel = maxFuel;
        hasFired = false;

        if (cameraController == null)
            cameraController = Camera.main.GetComponent<CameraController>();

        cameraController.focusPoint.FollowObject(gameObject);
        cameraController.Transition(CameraController.View.Side, 1.0f);

        MessagesManager.AddMessage("Ready!").SetWorldPosition(transform.position);
    }

    public void Unready()
    {
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
        foreach (GameObject customColorPart in customColoredParts)
        {
            customColorPart.GetComponent<Renderer>().material.color = color;
        }
    }

    public void DestroyTank()
    {
        gameObject.SetActive(false);
    }

    public void AddHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        MessagesManager.AddMessage("+" + amount.ToString("0.0") + " HP").SetColor(Color.green).SetWorldPosition(transform.position);
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
        MessagesManager.AddMessage("+" + amount.ToString("0.0" + " Fuel")).SetColor(new Color(0f, 190f/255f, 1f)).SetWorldPosition(transform.position);
    }

}
