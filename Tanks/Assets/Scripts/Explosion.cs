using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] ParticleSystem explosionEffect;

    float damage;
    Tanks.MapDestroyingExplosive mapDestruction;

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void SetRadius(float newRadius)
    {
        radius = newRadius;

        ParticleSystem.ShapeModule shape = explosionEffect.shape;
        ParticleSystem.EmissionModule emission = explosionEffect.emission;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        burst.count = radius * 30;

        emission.SetBurst(0, burst);

        shape.radius = radius;
    }

    public void SetParticles(ParticleSystem newSystem)
    {
        explosionEffect = newSystem;
    }

    public ParticleSystem getParticles()
    {
        return explosionEffect;
    }

    public void Explode()
    {
        mapDestruction = gameObject.AddComponent<Tanks.MapDestroyingExplosive>();
        mapDestruction.Explode(radius);

        Instantiate(explosionEffect, transform.position, Quaternion.Euler(-90, 0, 0));
        transform.parent = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag != "Tank")
                continue;


            try
            {
                if (collider.gameObject.GetComponentInParent<Player>().Tank)
                {
                    Tank tank = collider.gameObject.GetComponentInParent<Player>().Tank;

                    if (tank)
                        tank.TakeDamage(damage);

                }
            }
            catch
            {
                continue;
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
