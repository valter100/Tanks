using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrage : AttackPattern
{
    [Header("Barrage specifics")]
    [SerializeField] float timeBetweenBullets;
    [SerializeField] int bulletAmount;
    [SerializeField] float angleSpread;
    float timeSinceLastBullet;

    public override void Fire(Tank tank)
    {
        StartCoroutine(BulletBarrage(tank));
    }

    IEnumerator BulletBarrage(Tank tank)
    {
        for (int i = 0; i < bulletAmount; i++)
        {
            timeSinceLastBullet = 0;

            while (timeSinceLastBullet < timeBetweenBullets)
            {
                timeSinceLastBullet += Time.deltaTime;

                if(timeSinceLastBullet > timeBetweenBullets)
                {
                    tank.InstantiateProjectile();
                    timeSinceLastBullet = 0;
                }
            }
        }

        yield return 0;
    }
}
