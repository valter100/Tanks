using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{

    public abstract class AttackPattern : MonoBehaviour
    {
        public virtual void Fire(Tank tank)
        {
            tank.InstantiateProjectile();
        }
    }
}