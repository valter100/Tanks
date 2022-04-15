using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBomb : Projectile
{
    [SerializeField] float explosionRadius;

    public override void Hit(GameObject other)
    {
        other.GetComponent<Tank>().TakeDamage(damage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Collider[] colliderHits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliderHits)
        {
            if (collider.gameObject.tag == "Tank")
            {
                if (!canDamageSelf && collider.gameObject == ownTank.gameObject)
                {
                    continue;
                }

                Hit(collider.gameObject);
            }
        }
    }
}
