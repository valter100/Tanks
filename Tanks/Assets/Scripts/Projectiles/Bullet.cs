using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Tank>() != null && other.gameObject.GetComponent<Tank>() == ownTank)
            return;

        Hit(other.gameObject);
    }

    public override void Hit(GameObject other)
    {
        if (other.GetComponent<Tank>())
        {
            other.GetComponent<Tank>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }

}
