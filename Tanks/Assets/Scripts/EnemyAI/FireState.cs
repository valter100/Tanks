using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireState :  State
{
    public override void DoState(AiManager thisManager, Transform enemyTankPosition, AiTank thisTank)
    {
        if (thisManager.GetDistanceToEnemy() > thisManager.GetMaxShootingRange() && thisManager.GetFuel() > 0 || thisManager.GetDistanceToEnemy() <= thisManager.GetMinShootingRange() && thisManager.GetFuel() > 0)
        {
           thisManager.SetMoveState();
            return;
        }

        DoShoot(thisTank, enemyTankPosition);
        thisManager.activeTargetPosition = null;
        thisManager.SetStateToStartState();
    }

    private void DoShoot(AiTank thisTank, Transform enemyTankPosition)
    {
        thisTank.Aim(enemyTankPosition); 
    }
}
