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

    protected Rigidbody rb;
    protected Tank ownTank;
    [SerializeField] protected int ammoCount;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject,lifeTime);
    }

    private void OnDestroy()
    {
        ownTank.GetGameManager().NextPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Tank>() != null && other.gameObject.GetComponent<Tank>() == ownTank)
            return;

        Hit(other.gameObject);
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
        return ammoCount;
    }

    public void ChangeAmmoCount(int change)
    {
        ammoCount += change;
    }

    public void ResetAmmo()
    {
        ammoCount = startAmmoCount;
    }

    public bool HasAmmo()
    {
        return ammoCount > 0;
    }

    public abstract void Hit(GameObject other);
    
        
    
}
