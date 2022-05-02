using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackPattern : MonoBehaviour
{
    public virtual void Fire(Tank tank)
    {
        tank.InstantiateProjectile();
    }
}
