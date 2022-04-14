using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingShell : Projectile
{
    [Header("Exploding Shell")]
    [SerializeField] float explosionRadius;
        
    public override void Hit(GameObject other)
    {
        Collider[] colliderHits = Physics.OverlapSphere(other.transform.position, explosionRadius);

        foreach (Collider collider in colliderHits)
        {
            if(collider.gameObject.tag == "Tank"/* && collider.gameObject != ownTank.gameObject*/)
            {
                collider.gameObject.GetComponent<Tank>().TakeDamage(damage);
            }
        }

        Instantiate(particles, transform.position, Quaternion.Euler(-90, 0, 0), null);
        Vector3 distance = transform.position - other.transform.position;
        particles.transform.rotation = Quaternion.LookRotation(distance);
        ownTank.GetComponent<AudioSource>().PlayOneShot(clip);
        Destroy(gameObject);
    }
}
