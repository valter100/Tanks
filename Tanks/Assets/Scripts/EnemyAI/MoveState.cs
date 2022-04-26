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
        distanceToEnemy = Vector3.Distance(thisTankPosition.position, enemyTankPosition.position);
        fuel = thisTank.GetFuelPercentage();
        maxShootingRange = thisTank.GetMaxShootForce();

        if (distanceToEnemy < maxShootingRange || fuel < 1f)
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
