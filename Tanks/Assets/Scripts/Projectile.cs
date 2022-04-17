using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected AudioClip clip;
    [SerializeField] protected int damage;
    [SerializeField] protected int lifeTime;
    [SerializeField] protected ParticleSystem particles;
    [SerializeField] protected int startAmmoCount;
    [SerializeField] protected bool canDamageSelf;
    [SerializeField] protected Terrain terrain;
    [SerializeField] protected float explosionRadius;

    protected Rigidbody rb;
    protected Tank ownTank;
    protected bool copy = false;

    public int getLifeTime => lifeTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        terrain = Terrain.activeTerrain;
        Destroy(gameObject,lifeTime);
    }

    protected virtual void OnDestroy()
    {
        Instantiate(particles, transform.position, Quaternion.identity, null);
        
        if(GetComponent<DestructableTerrain>())
            GetComponent<DestructableTerrain>().Terraform(this);

        //Vector3 distance = transform.position - other.transform.position;
        //particles.transform.rotation = Quaternion.LookRotation(distance);
        ownTank.GetComponent<AudioSource>().PlayOneShot(clip);
        ownTank.GetGameManager().NextPlayer();
    }

    public void Fire(Quaternion angle, float force)
    {
        transform.rotation = angle;
        Vector3 shootVector = gameObject.transform.up * force;

        rb.AddForce(shootVector, ForceMode.Impulse);
    }

    public void SetOwnTank(Tank tank)
    {
        ownTank = tank;
    }

    public int GetAmmoCount()
    {
        return startAmmoCount;
    }

    public float GetExplosionRadius()
    {
        return explosionRadius;
    }
    public abstract void Hit(GameObject other);
    
    public Vector3? PrecomputeTrajectory()
    {
        RaycastHit raycastHit;

        Projectile copy = Instantiate(gameObject, transform).GetComponent<Projectile>();
        copy.transform.parent = null;

        Rigidbody rigidbodyCopy = copy.GetComponent<Rigidbody>();

        // For the duration of the copy's maximum life time
        for (float elapsedTime = 0.0f; elapsedTime < copy.getLifeTime; elapsedTime += Time.fixedDeltaTime)
        {
            // Check collision
            if (rigidbodyCopy.SweepTest(rigidbodyCopy.velocity, out raycastHit, rigidbodyCopy.velocity.magnitude * Time.fixedDeltaTime))
            {
                Destroy(copy.gameObject);
                if (raycastHit.transform.gameObject.tag == "Tank")
                    return raycastHit.transform.position;

                return null;
            }

            // Simulate frame
            rigidbodyCopy.velocity *= 1.0f - Time.fixedDeltaTime * rigidbodyCopy.drag;
            rigidbodyCopy.velocity += Physics.gravity * Time.fixedDeltaTime;
            rigidbodyCopy.position += rigidbodyCopy.velocity * Time.fixedDeltaTime;
        }

        return null;
    }
    
}
