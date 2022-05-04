using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpread : Bullet
{
    [SerializeField] float spreadTimer;
    [SerializeField] int spreadCount;
    [SerializeField] float spreadAngleRange;
    [SerializeField] Projectile bullet;

    bool hasSpread = false;
    float timeSinceSpawn;

    private void Update()
    {
        timeSinceSpawn += Time.deltaTime;
        if (timeSinceSpawn > spreadTimer && !hasSpread)
        {
            Spread();
            hasSpread = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerTank>() != null && other.gameObject.GetComponent<PlayerTank>() == ownTank)
            return;

        Detonate(other);
    }

    public void Spread()
    {
        for(int i = 0; i < spreadCount; i++)
        {
            float randomAngle = Random.Range(-spreadAngleRange, spreadAngleRange);

            //GameObject spreadBullet = Instantiate(bullet.gameObject, transform.position, transform.rotation );
            //spreadBullet.GetComponent<Projectile>().SetDamage(damage);
            //spreadBullet.GetComponent<Projectile>().SetOwnTank(ownTank);
            //spreadBullet.transform.Rotate(0, 0, randomAngle);
            //spreadBullet.GetComponent<Rigidbody>().AddForce(transform.right * GetComponent<Rigidbody>().velocity.magnitude * 15);
            //spreadBullet.GetComponent<Projectile>().SetEndsRound(false);
        }
    }
}
