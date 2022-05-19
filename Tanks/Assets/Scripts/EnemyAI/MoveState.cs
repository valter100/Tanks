using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    
    public override void DoState(AiManager thisManager,Transform enemyTankPosition, AiTank thisTank)
    {
        if (thisManager.GetDistanceToEnemy() < thisManager.GetMaxShootingRange() && thisManager.GetDistanceToEnemy()  > thisManager.GetMinShootingRange() || thisManager.GetFuel() <= 0)
        {
            thisManager.SetFireState();
            return;
        }

        DoMove(enemyTankPosition, thisTank, thisManager);

    }

    private void DoMove(Transform enemyTankPosition, AiTank thisTank, AiManager thisManager)
    {
        if (thisManager.GetDistanceToEnemy() < thisManager.GetMaxShootingRange())
            thisTank.MoveCloser(enemyTankPosition);
        else if (thisManager.GetDistanceToEnemy() < thisManager.GetMaxShootingRange())
            thisTank.MoveAway(enemyTankPosition);
        
    }
}
