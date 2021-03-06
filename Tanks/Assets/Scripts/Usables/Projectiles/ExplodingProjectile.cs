using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingProjectile : Projectile
{
    [Header("Exploding Projectile Specifics")]
    [SerializeField] protected Explosion explosion;

    public float Radius => explosion.Radius;

    private void Start()
    {
        explosion.SetDamage(damage);
    }

    // Update is called once per frame
    override protected void Update()
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
        explosion.Explode();
        base.Detonate(other);
    }
}
