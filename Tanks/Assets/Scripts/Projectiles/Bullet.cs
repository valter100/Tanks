using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    public override void Hit(GameObject other)
    {
        if (other.GetComponent<Tank>())
        {
            other.GetComponent<Tank>().TakeDamage(damage);
        }

        Instantiate(particles, transform.position, Quaternion.Euler(-90, 0, 0), null);
        Vector3 distance = transform.position - other.transform.position;
        particles.transform.rotation = Quaternion.LookRotation(distance);
        ownTank.GetComponent<AudioSource>().PlayOneShot(clip);
        Destroy(gameObject);
    }

}
