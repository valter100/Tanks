using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBomb : ExplodingProjectile
{
    [Header("Bounce Bomb Specifics")]
    [SerializeField] int bouncesLeft;
    [SerializeField] float elasticity;
    [SerializeField] float addedRadius;

    float yVelocityBefore;

    protected override void Update()
    {
        yVelocityBefore = rb.velocity.y;

        base.Update();
    }

    protected override void OnCollision(Collider other)
    {
        if (bouncesLeft <= 0)
            base.OnCollision(other);
        else if (yVelocityBefore < 0)
            Bounce();
    }

    void Bounce()
    {
        explosion.SetRadius(bouncesLeft + addedRadius);
        explosion.Explode();

        bouncesLeft--;

        float newYVelocity = yVelocityBefore * -1 * elasticity;

        rb.velocity = new Vector3(rb.velocity.x, newYVelocity, 0);

        //play bounce animation?
    }
}
