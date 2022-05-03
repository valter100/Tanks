using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{

    public class DropBomb : ExplodingProjectile
    {
        [Header("Drop Bomb Specifics")]
        [SerializeField] float dropTime;

        bool hasDropped = false;

        protected override void Update()
        {
            base.Update();
            if (!hasDropped && GetStartTime() - GetTimeToLive() > dropTime)
            {
                float xVelocity = 0;
                float yVelocity = -5;

                rb.velocity = new Vector3(xVelocity, yVelocity, 0);
                hasDropped = true;
            }
        }
    }
}