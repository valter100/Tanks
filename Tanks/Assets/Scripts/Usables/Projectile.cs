using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : Usable
{
    [Header("Projectile")]
    [SerializeField] protected AttackPattern attackPattern;
    [SerializeField] protected float damage;
    [SerializeField] protected float timeToLive;
    [SerializeField] protected AudioClip detonationSound;
    [SerializeField] protected AudioSource source;
    [SerializeField] protected ParticleSystem trailParticles;
    [SerializeField] protected ParticleSystem detonationParticles;
    [SerializeField] protected bool canDamageSelf;
    [SerializeField] LayerMask hitableLayers;

    [SerializeField] protected bool hittingGround;
    [SerializeField] protected float distanceToGround;
    protected float oldDistanceToGround;
    float startTime;
    protected Rigidbody rb;
    public Tank ownTank;
    Vector3 oldPosition;

    public AttackPattern GetAttackPattern() => attackPattern;
    public float GetStartTime() => startTime;
    public float GetTimeToLive() => timeToLive;
    public float GetDamage() => damage;
    public string GetDescription() => description;

    public struct PrecomputedResult
    {
        public RaycastHit raycastHit;
        public Tank tank;
        public float timeBeforeHit;
        public float damageDealtToTank;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startTime = timeToLive;
    }

    void OnDestroy()
    {
        // This method must not instantiate any GameObject, play sound, swap player,
        // or affect the scene or game in any other way.

        // This is because precomputed projectiles must be able to be instantiated
        // and destroyed as if nothing ever happened.
    }

    protected virtual void Update()
    {
        // Visualize trail
        //PointVisualizer.AddPoint(transform.position);

        transform.rotation = Quaternion.LookRotation(rb.velocity);

        // Emitt trail particles
        if (trailParticles != null)
            Instantiate(trailParticles, transform.position, Quaternion.identity, null);

        // Update timer
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0.0f)
            Detonate(null);

        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);

        if(Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue, hitableLayers))
        {
            if(hit.collider.gameObject.tag == "Map")
            {
                hittingGround = true;
            }

            if (hittingGround)
            {
                distanceToGround = Vector3.Distance(transform.position, hit.transform.position);
                if (distanceToGround < 5 && oldDistanceToGround < distanceToGround)
                    OnCollision(null);
                oldDistanceToGround = distanceToGround;
            }
        }
        else
        {
            if (hittingGround)
            {
                OnCollision(null);
                //Detonate(null);
            }
        }


        //RaycastHit hit;

        //oldPosition = transform.position;

        //if(Physics.Linecast(transform.position, oldPosition, out hit, hitableLayers))
        //{
        //    if (hit.collider.gameObject.tag == "Water")
        //    {
        //        Destroy(gameObject);
        //        return;
        //    }
        //    else
        //    {
        //        OnCollision(hit.collider);
        //    }

        //}


    }

    /// <summary>
    /// Gets called when this Projectile detects a collision, either through OnCollisionEnter() or OnTriggerEnter().
    /// </summary>
    protected virtual void OnCollision(Collider other)
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Water")
        {
            Destroy(gameObject);
            return;
        }

        OnCollision(other.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            Destroy(gameObject);
            return;
        }

        OnCollision(other);
    }

    /// <summary>
    /// Detonates and destroys this Projectile.
    /// </summary>
    /// <param name="collider">Collider can be null. This must be handled by subclasses if utilized.</param>
    protected virtual void Detonate(Collider collider)
    {
        if (detonationParticles != null)
            Instantiate(detonationParticles, transform.position, Quaternion.identity, null);
        if (detonationSound)
            ownTank.GetComponent<AudioSource>().PlayOneShot(detonationSound);

        Destroy(gameObject);
    }

    /// <summary>
    /// Returns whether or not the provided Tank is a Tank, and that it can be damanged
    /// </summary>
    protected bool CanDamage(Tank tank)
    {
        if (tank == null)
            return false;

        if (!canDamageSelf && tank == ownTank)
            return false;

        return true;
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Fires this Projectile.
    /// </summary>
    public void Fire(Quaternion angle, float power)
    {
        transform.rotation = angle;
        Vector3 force = transform.up * power;

        Vector3 velocity = force / rb.mass;
        rb.velocity = velocity;

        // Equivalent to: rb.AddForce(force, ForceMode.Impulse)
        // AddForce is not used since the Rigidbody requires an update before
        // the applied force affects its velocity
    }

    /// <summary>
    /// Precomputes the aerial trajectory of this Projectile before destorying the GameObject.
    /// </summary>
    public PrecomputedResult? PrecomputeTrajectory(float timeToVisualize = 0.0f)
    {
        RaycastHit raycastHit;

        // For the duration of its life time
        for (float elapsedTime = 0.0f; elapsedTime < timeToLive; elapsedTime += Time.fixedDeltaTime)
        {
            if (timeToVisualize != 0.0f)
                PointVisualizer.AddPoint(rb.position, timeToVisualize);

            // Check collision
            if (rb.SweepTest(rb.velocity, out raycastHit, rb.velocity.magnitude * Time.fixedDeltaTime))
            {
                if (timeToVisualize != 0.0f)
                    PointVisualizer.AddPoint(raycastHit.point, timeToVisualize);

                PrecomputedResult precomputedResult = new PrecomputedResult();
                precomputedResult.raycastHit = raycastHit;
                precomputedResult.tank = raycastHit.transform.GetComponent<Tank>();
                precomputedResult.timeBeforeHit = elapsedTime;
                precomputedResult.damageDealtToTank = damage;

                Destroy(gameObject);
                return precomputedResult;
            }
            
            // Simulate frame
            rb.velocity *= 1.0f - Time.fixedDeltaTime * rb.drag;
            rb.velocity += Physics.gravity * Time.fixedDeltaTime;
            rb.position += rb.velocity * Time.fixedDeltaTime;
        }

        Destroy(gameObject);
        return null;
    }
}
