using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;

    private LayerMask groundLayer;
    public override void DoState(AiManager thisManager,Transform enemyTankPosition, AiTank thisTank)
    {
        if (thisManager.GetDistanceToEnemy() < 30 || thisManager.GetFuel() <= 0)
        {
            thisManager.SetFireState();
            return;
        }

        DoMove(enemyTankPosition, thisTank);
    }

    private void DoMove(Transform enemyTankPosition, AiTank thisTank)
    {
        thisTank.Move(enemyTankPosition);
    }
}
