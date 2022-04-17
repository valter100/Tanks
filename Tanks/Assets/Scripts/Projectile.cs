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

    public void Shoot(Quaternion angle, float force)
    {
        transform.rotation = angle;
        Vector3 shootVector = gameObject.transform.up * force;

        rb.AddForce(shootVector);
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
    
        
    
}
