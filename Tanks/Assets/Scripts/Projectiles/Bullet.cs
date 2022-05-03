using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
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
        if (other != null)
        {
            Tank tank = other.gameObject.GetComponent<Tank>();

            if (CanDamage(tank))
                tank.TakeDamage(damage);
        }

        base.Detonate(other);
    }
}
