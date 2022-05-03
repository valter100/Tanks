using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tanks
{

    public class Simple : AttackPattern
    {
        public override void Fire(Tank tank)
        {
            tank.InstantiateProjectile();
        }
    }
}