using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : ActiveState
{
    public override ActiveState DoState(AiManager thisBehaviour, Transform thisTankPosition, Transform enemyTankPosition, Tank thisTank)
    {
        if (thisBehaviour.GetDistanceToEnemy() >= thisBehaviour.GetMaxShootingRange() && thisBehaviour.GetFuel() >= 1f)
        {
            return thisBehaviour.moveState;
        }
        else if (thisBehaviour.GetDistanceToEnemy() < thisBehaviour.GetMaxShootingRange() || thisBehaviour.GetFuel() <= 0)
        {
            return thisBehaviour.shootState;
        }
        else return null;
    }
}
