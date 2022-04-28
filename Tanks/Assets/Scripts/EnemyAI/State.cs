using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class State : MonoBehaviour
{
    public virtual void DoState(AiManager thisBehaviour, Transform thisTankPosition, Transform enemyTankPosition, Tank thisTank) { }
}
