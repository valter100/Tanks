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
    [Header("Player")]
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected CameraController cameraController;
    [SerializeField] protected bool isActive;
    //[SerializeField] int playerIndex;
    [SerializeField] protected string playerName;
    [SerializeField] protected Color playerColor;

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
    [SerializeField] protected List<Projectile> projectiles;
    [SerializeField] protected List<int> ammo;
    [SerializeField] protected Projectile currentProjectile;
    [SerializeField] protected ParticleSystem fireParticles;
    [SerializeField] protected Animator animator;
    [SerializeField] protected ParticleSystem tankDamagedParticles;
    [SerializeField] protected ParticleSystem tankDestroyedParticles;

    [Header("Status Effects")]
    [SerializeField] protected bool isSlowed;

    [Header("References")]
    [SerializeField] protected GameObject rotatePoint;
    [SerializeField] protected GameObject[] tankParts;
    [SerializeField] protected GameObject cannon;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected Transform rumbleSpot;

    [Header("UI")]
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected TMP_Text healthText;
    [SerializeField] protected Slider healthSlider;
    [SerializeField] protected TMP_Text fuelText;
    [SerializeField] protected Slider fuelSlider;
    [SerializeField] protected Slider shootForceSlider;
    [SerializeField] protected TMP_Text projectileText;
    [SerializeField] protected float aimRadius;

    protected float timeSinceLastEffect;
    protected int projectileIndex;
    protected bool hasFired = false;
    protected float currentFuel;
    protected float currentHealth;
    protected float currentShootForce;
    protected Rigidbody rb;
    protected Ray ray;
    protected TMP_Text projectileTMP;

    public string GetPlayerName() => playerName;

    public Projectile GetCurrentProjectile() => currentProjectile;

    public float GetFuelPercentage() => currentFuel / maxFuel;

    public float GetHealthPercentage() => currentHealth / maxHealth;

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxShootForce() => maxShootForce;

    public float GetMovementSpeed() => movementSpeed;

    public GameManager GetGameManager() => gameManager;

    public bool HasAmmo() => ammo[projectileIndex] > 0;

    public void AssignPlayer(int newIndex, string newName, Color newColor)
    {
        playerName = newName;
        playerColor = newColor;

        foreach (GameObject go in tankParts)
        {
            go.GetComponent<MeshRenderer>().material.color = newColor;
        }

        if (nameText)
            nameText.text = playerName;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Water" && currentHealth > 0)
        {
            TakeDamage(currentHealth);
            gameManager.StartPlayerTransition();
        }
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

    public void SpawnDestroyedParticles()
    {
        Instantiate(tankDestroyedParticles, transform.position, Quaternion.Euler(-90, 0, 0), null);
    }

    protected Projectile InstantiateProjectile()
    {
        Projectile projectile = Instantiate(currentProjectile, firePoint);
        projectile.ownTank = this;
        projectile.transform.parent = null;
        projectile.Fire(cannon.transform.rotation, currentShootForce);
        return projectile;
    }

    public bool CanFire()
    {
        return HasAmmo()
            && !hasFired
            && isActive
            && currentHealth > 0.0f;
    }

    public virtual void Fire() { }

}
