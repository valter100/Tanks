using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingShell : Projectile
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollision(Collider other)
    {
        base.OnCollision(other);
        Detonate(other);
    }

    protected override void Detonate(Collider other)
    {
        Collider[] colliderHits = Physics.OverlapSphere(transform.position, explosionRadius);

        Tank tank;
        foreach (Collider colliderHit in colliderHits)
        {
            tank = colliderHit.gameObject.GetComponent<Tank>();

            if (CanDamage(tank))
                tank.TakeDamage(damage);
        }

        base.Detonate(other);
    }
}
