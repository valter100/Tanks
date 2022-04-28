using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : State
{
    public override void DoState(AiManager thisBehaviour, Transform thisTankPosition, Transform enemyTankPosition, Tank thisTank)
    {
        if (thisBehaviour.GetDistanceToEnemy() >= thisBehaviour.GetMaxShootingRange() && thisBehaviour.GetFuel() >= 1f)
        {
           thisBehaviour.SetMoveState();
           return;
        }
        else if (thisBehaviour.GetDistanceToEnemy() < thisBehaviour.GetMaxShootingRange() || thisBehaviour.GetFuel() <= 0)
        {
           thisBehaviour.SetFireState();
           return;
        }
    }
}
