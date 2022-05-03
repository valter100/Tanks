using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{

    public class RollBomb : ExplodingProjectile
    {
        protected override void Update()
        {
            base.Update();
        }

        protected override void OnCollision(Collider other)
        {
            if (timeToLive > 2.0f)
                timeToLive = 2.0f;
        }
    }
}