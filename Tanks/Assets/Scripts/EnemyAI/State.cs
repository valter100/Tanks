using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class State
{
    public virtual void DoState(AiManager thisBehaviour, Transform enemyTankPosition, AiTank thisTank) { }

    public virtual void Initialize() { }
}
