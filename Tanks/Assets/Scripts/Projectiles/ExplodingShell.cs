using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingShell : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Tank>() != null && other.gameObject.GetComponent<Tank>() == ownTank)
            return;

        Hit(other.gameObject);
    }

    public override void Hit(GameObject other)
    {
        Collider[] colliderHits = Physics.OverlapSphere(other.transform.position, explosionRadius);

        foreach (Collider collider in colliderHits)
        {
            if (collider.gameObject.tag == "Tank")
            {
                if (!canDamageSelf && collider.gameObject == ownTank.gameObject)
                {
                    continue;
                }

                collider.gameObject.GetComponent<Tank>().TakeDamage(damage);
            }
        }

        //Vector3 distance = transform.position - other.transform.position;
        //particles.transform.rotation = Quaternion.LookRotation(distance);
        ownTank.GetComponent<AudioSource>().PlayOneShot(clip);
        Destroy(gameObject);
    }
}
