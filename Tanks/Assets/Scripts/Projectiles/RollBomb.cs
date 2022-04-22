using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBomb : Projectile
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollision(Collider other)
    {
        base.OnCollision(other);

        if (timeToLive > 2.0f)
            timeToLive = 2.0f;
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
