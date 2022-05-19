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
        explosion.SetRadius(bouncesLeft + addedRadius);
        if (bouncesLeft <= 0)
            base.OnCollision(other);
        else if (yVelocityBefore < 0)
            Bounce();
    }

    void Bounce()
    {
        explosion.Explode();
        bouncesLeft--;
        
        ownTank.GetComponent<AudioSource>().PlayOneShot(detonationSound);

        float newYVelocity = yVelocityBefore * -1 * elasticity;

        rb.velocity = new Vector3(rb.velocity.x, newYVelocity, 0);

        hittingGround = false;
        distanceToGround = 0;
        oldDistanceToGround = distanceToGround;
        //play bounce animation?
    }
}
