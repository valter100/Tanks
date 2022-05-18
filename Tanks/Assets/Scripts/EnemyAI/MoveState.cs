using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    public override void DoState(AiManager thisManager,Transform enemyTankPosition, AiTank thisTank)
    {
        if (thisManager.GetDistanceToEnemy() < thisManager.GetMaxShootingRange() || thisManager.GetFuel() <= 0)
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
