using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireState :  State
{
    public override void DoState(AiManager thisManager, Transform enemyTankPosition, AiTank thisTank)
    {
        if (thisManager.GetDistanceToEnemy() >= thisManager.GetMaxShootingRange() && thisManager.GetFuel() > 0)
        {
           thisManager.SetMoveState();
            return;
        }

        DoShoot(thisTank);
        thisManager.activeTargetPosition = null;
    }

    private void DoShoot(AiTank thisTank)
    {
        //If we are in fireing state, calculate the correct aim to hit target, then randomize a bias and add to aim to make sure not every shot is a direct hit.
        //thisTank.Fire();
        Debug.Log("PewPew");
    }
}
