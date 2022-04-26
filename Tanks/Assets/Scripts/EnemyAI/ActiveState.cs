using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class ActiveState : MonoBehaviour
{
    public virtual ActiveState DoState(AiManager thisBehaviour, Transform thisTankPosition,Transform enemyTankPosition, Tank thisTank) { return null; }
}
