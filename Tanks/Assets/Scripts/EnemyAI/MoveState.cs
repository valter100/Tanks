using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : ActiveState
{
    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;
    public override ActiveState DoState(AiManager thisBehaviour, Transform thisTankPosition,Transform enemyTankPosition, Tank thisTank)
    {
        if (thisBehaviour.GetDistanceToEnemy() < thisBehaviour.GetMaxShootingRange() || thisBehaviour.GetFuel() <= 0)
        {
            return thisBehaviour.shootState;
        }


        DoMove();

        return null;
    }

    private void DoMove()
    {

    }
}
