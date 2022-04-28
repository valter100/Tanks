using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;
    public override void DoState(AiManager thisBehaviour, Transform thisTankPosition,Transform enemyTankPosition, Tank thisTank)
    {
        if (thisBehaviour.GetDistanceToEnemy() < thisBehaviour.GetMaxShootingRange() || thisBehaviour.GetFuel() <= 0)
        {
            thisBehaviour.SetFireState();
            return;
        }

        DoMove();
    }

    private void DoMove()
    {

    }
}
