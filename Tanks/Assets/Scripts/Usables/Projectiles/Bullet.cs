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
            PlayerTank tank = other.gameObject.GetComponent<PlayerTank>();

            if (CanDamage(tank))
                tank.TakeDamage(damage);
        }

        base.Detonate(other);
    }
}
